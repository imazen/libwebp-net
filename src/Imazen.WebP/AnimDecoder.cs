using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Imazen.WebP.Extern;

namespace Imazen.WebP
{
    /// <summary>
    /// Information about an animated WebP image.
    /// </summary>
    public class AnimInfo
    {
        /// <summary>Canvas width in pixels.</summary>
        public int Width { get; }
        /// <summary>Canvas height in pixels.</summary>
        public int Height { get; }
        /// <summary>Total number of frames.</summary>
        public int FrameCount { get; }
        /// <summary>Number of loop iterations (0 = infinite).</summary>
        public int LoopCount { get; }
        /// <summary>Background color (BGRA packed in uint32).</summary>
        public uint BackgroundColor { get; }

        internal AnimInfo(int width, int height, int frameCount, int loopCount, uint bgColor)
        {
            Width = width;
            Height = height;
            FrameCount = frameCount;
            LoopCount = loopCount;
            BackgroundColor = bgColor;
        }
    }

    /// <summary>
    /// Decodes animated WebP images into individual frames.
    /// Uses libwebpdemux's WebPAnimDecoder internally.
    /// </summary>
    public class AnimDecoder : IDisposable
    {
        private IntPtr _decoder;
        private GCHandle _dataHandle;
        private bool _disposed;
        private readonly AnimInfo _info;
        private int _prevEndTimestamp;

        /// <summary>
        /// Creates an animation decoder from WebP data.
        /// </summary>
        /// <param name="webpData">Complete animated WebP file data.</param>
        /// <param name="useThreads">Use multi-threaded decoding.</param>
        public AnimDecoder(byte[] webpData, bool useThreads = false)
        {
            if (webpData == null) throw new ArgumentNullException(nameof(webpData));

            _dataHandle = GCHandle.Alloc(webpData, GCHandleType.Pinned);
            try
            {
                var data = new WebPData
                {
                    bytes = _dataHandle.AddrOfPinnedObject(),
                    size = (UIntPtr)webpData.Length
                };

                var options = new WebPAnimDecoderOptions();
                NativeLibraryLoader.FixDllNotFoundException("webpdemux", () =>
                {
                    if (NativeMethods.WebPAnimDecoderOptionsInit(ref options) == 0)
                        throw new Exception("Failed to initialize animation decoder options (version mismatch)");
                    return 0;
                });
                options.color_mode = WEBP_CSP_MODE.MODE_BGRA;
                options.use_threads = useThreads ? 1 : 0;

                _decoder = NativeLibraryLoader.FixDllNotFoundException("webpdemux",
                    () => NativeMethods.WebPAnimDecoderNew(ref data, ref options));

                if (_decoder == IntPtr.Zero)
                    throw new InvalidDataException("Failed to create animation decoder. Data may not be a valid animated WebP.");

                var animInfo = new WebPAnimInfo();
                int infoResult = NativeLibraryLoader.FixDllNotFoundException("webpdemux",
                    () => NativeMethods.WebPAnimDecoderGetInfo(_decoder, ref animInfo));

                if (infoResult == 0)
                    throw new InvalidDataException("Failed to get animation info");

                // libwebp's canvas_{width,height} are uint32. Reject anything
                // outside the spec cap so subsequent `width * height * 4`
                // multiplications cannot overflow int32 silently.
                if (animInfo.canvas_width == 0 || animInfo.canvas_height == 0 ||
                    animInfo.canvas_width > NativeMethods.WEBP_MAX_DIMENSION ||
                    animInfo.canvas_height > NativeMethods.WEBP_MAX_DIMENSION)
                {
                    throw new InvalidDataException(
                        $"Animation canvas dimensions out of range: {animInfo.canvas_width}x{animInfo.canvas_height}");
                }

                _info = new AnimInfo(
                    (int)animInfo.canvas_width,
                    (int)animInfo.canvas_height,
                    (int)animInfo.frame_count,
                    (int)animInfo.loop_count,
                    animInfo.bgcolor);
            }
            catch
            {
                // Constructor exception: clean up native + pinned-handle
                // resources before propagating, so the GC won't pin webpData
                // forever and the native decoder won't leak.
                if (_decoder != IntPtr.Zero)
                {
                    try
                    {
                        NativeMethods.WebPAnimDecoderDelete(_decoder);
                    }
                    catch
                    {
                        // best-effort
                    }
                    _decoder = IntPtr.Zero;
                }
                if (_dataHandle.IsAllocated)
                    _dataHandle.Free();
                throw;
            }
        }

        /// <summary>
        /// Creates an animation decoder from a stream. Reads the entire stream
        /// into memory first, capped by
        /// <see cref="WebPLimits.MaxDecodeStreamBytes"/>.
        /// </summary>
        public AnimDecoder(Stream stream, bool useThreads = false)
            : this(ReadStreamFully(stream, WebPLimits.MaxDecodeStreamBytes), useThreads)
        {
        }

        /// <summary>
        /// Creates an animation decoder from a stream with a caller-supplied
        /// buffered-data cap.
        /// </summary>
        public AnimDecoder(Stream stream, long maxBytes, bool useThreads = false)
            : this(ReadStreamFully(stream, maxBytes), useThreads)
        {
        }

        /// <summary>
        /// Gets information about the animation (dimensions, frame count, loop count).
        /// </summary>
        public AnimInfo Info => _info;

        /// <summary>
        /// Decodes all frames and returns them as a list.
        /// Each frame's Pixels array contains BGRA data for the full canvas.
        /// </summary>
        public List<AnimFrame> DecodeAllFrames()
        {
            ThrowIfDisposed();
            Reset();

            var frames = new List<AnimFrame>();
            AnimFrame? frame;
            while ((frame = GetNextFrame()) != null)
            {
                frames.Add(frame);
            }
            return frames;
        }

        /// <summary>
        /// Gets the next decoded frame, or null if no more frames.
        /// The returned pixel data is BGRA for the full canvas.
        /// </summary>
        public AnimFrame? GetNextFrame()
        {
            ThrowIfDisposed();

            if (!HasMoreFrames()) return null;

            IntPtr buf = IntPtr.Zero;
            int endTimestamp = 0;

            int result = NativeLibraryLoader.FixDllNotFoundException("webpdemux",
                () => NativeMethods.WebPAnimDecoderGetNext(_decoder, ref buf, ref endTimestamp));

            if (result == 0) return null;

            // Width/Height are validated in the constructor to be in
            // (0, WEBP_MAX_DIMENSION], so canvasSize <= 16383*16383*4 fits
            // in int32 by an order of magnitude. The long widening here is
            // defense-in-depth in case the constraint is ever relaxed.
            long canvasSizeL = (long)_info.Width * _info.Height * 4;
            if (canvasSizeL <= 0 || canvasSizeL > int.MaxValue)
                throw new InvalidDataException($"Frame size overflow: {canvasSizeL}");
            int canvasSize = (int)canvasSizeL;
            byte[] pixels = new byte[canvasSize];
            Marshal.Copy(buf, pixels, 0, canvasSize);

            // WebPAnimDecoderGetNext returns the frame's END timestamp.
            // Convert to start timestamp for consistency with AnimEncoder.AddFrame.
            int startTimestamp = _prevEndTimestamp;
            var frame = new AnimFrame(pixels, _info.Width, _info.Height, startTimestamp);
            frame.DurationMs = endTimestamp - startTimestamp;
            _prevEndTimestamp = endTimestamp;

            return frame;
        }

        /// <summary>
        /// Checks if there are more frames to decode.
        /// </summary>
        public bool HasMoreFrames()
        {
            ThrowIfDisposed();
            return NativeLibraryLoader.FixDllNotFoundException("webpdemux",
                () => NativeMethods.WebPAnimDecoderHasMoreFrames(_decoder)) != 0;
        }

        /// <summary>
        /// Resets the decoder to the first frame.
        /// </summary>
        public void Reset()
        {
            ThrowIfDisposed();
            NativeLibraryLoader.FixDllNotFoundException("webpdemux", () =>
            {
                NativeMethods.WebPAnimDecoderReset(_decoder);
                return 0;
            });
            _prevEndTimestamp = 0;
        }

        private void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(AnimDecoder));
        }

        private static byte[] ReadStreamFully(Stream stream, long maxBytes)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (maxBytes <= 0) throw new ArgumentOutOfRangeException(nameof(maxBytes));
            if (stream is MemoryStream ms && ms.Position == 0)
            {
                if (ms.Length > maxBytes)
                    throw new InvalidDataException(
                        $"Encoded WebP data {ms.Length} bytes exceeds cap of {maxBytes} bytes.");
                return ms.ToArray();
            }
            using (var output = new MemoryStream())
            {
                byte[] buffer = new byte[8192];
                long total = 0;
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    total += read;
                    if (total > maxBytes)
                        throw new InvalidDataException(
                            $"Encoded WebP data exceeds cap of {maxBytes} bytes.");
                    output.Write(buffer, 0, read);
                }
                return output.ToArray();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~AnimDecoder()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (_decoder != IntPtr.Zero)
            {
                // Finalizer path: don't go through FixDllNotFoundException —
                // it can take locks and call back into managed code, which
                // is unsafe from a finalizer thread. The native library is
                // already loaded by this point (the decoder handle came
                // from it), so a direct call is fine.
                try
                {
                    NativeMethods.WebPAnimDecoderDelete(_decoder);
                }
                catch
                {
                    // best-effort
                }
                _decoder = IntPtr.Zero;
            }
            if (_dataHandle.IsAllocated)
                _dataHandle.Free();

            _disposed = true;
        }
    }
}
