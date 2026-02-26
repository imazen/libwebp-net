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

        /// <summary>
        /// Creates an animation decoder from WebP data.
        /// </summary>
        /// <param name="webpData">Complete animated WebP file data.</param>
        /// <param name="useThreads">Use multi-threaded decoding.</param>
        public AnimDecoder(byte[] webpData, bool useThreads = false)
        {
            if (webpData == null) throw new ArgumentNullException(nameof(webpData));

            _dataHandle = GCHandle.Alloc(webpData, GCHandleType.Pinned);
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
                throw new Exception("Failed to create animation decoder. Data may not be a valid animated WebP.");

            var animInfo = new WebPAnimInfo();
            int infoResult = NativeLibraryLoader.FixDllNotFoundException("webpdemux",
                () => NativeMethods.WebPAnimDecoderGetInfo(_decoder, ref animInfo));

            if (infoResult == 0)
                throw new Exception("Failed to get animation info");

            _info = new AnimInfo(
                (int)animInfo.canvas_width,
                (int)animInfo.canvas_height,
                (int)animInfo.frame_count,
                (int)animInfo.loop_count,
                animInfo.bgcolor);
        }

        /// <summary>
        /// Creates an animation decoder from a stream.
        /// </summary>
        public AnimDecoder(Stream stream, bool useThreads = false)
            : this(ReadStreamFully(stream), useThreads)
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
            int canvasSize = _info.Width * _info.Height * 4;

            while (HasMoreFrames())
            {
                IntPtr buf = IntPtr.Zero;
                int timestamp = 0;

                int result = NativeLibraryLoader.FixDllNotFoundException("webpdemux",
                    () => NativeMethods.WebPAnimDecoderGetNext(_decoder, ref buf, ref timestamp));

                if (result == 0)
                    throw new Exception("Failed to decode animation frame");

                byte[] pixels = new byte[canvasSize];
                Marshal.Copy(buf, pixels, 0, canvasSize);

                var frame = new AnimFrame(pixels, _info.Width, _info.Height, timestamp);
                frames.Add(frame);
            }

            // Compute durations from timestamps
            for (int i = 0; i < frames.Count - 1; i++)
                frames[i].DurationMs = frames[i + 1].TimestampMs - frames[i].TimestampMs;

            if (frames.Count > 0)
                frames[frames.Count - 1].DurationMs = 0;

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
            int timestamp = 0;

            int result = NativeLibraryLoader.FixDllNotFoundException("webpdemux",
                () => NativeMethods.WebPAnimDecoderGetNext(_decoder, ref buf, ref timestamp));

            if (result == 0) return null;

            int canvasSize = _info.Width * _info.Height * 4;
            byte[] pixels = new byte[canvasSize];
            Marshal.Copy(buf, pixels, 0, canvasSize);

            return new AnimFrame(pixels, _info.Width, _info.Height, timestamp);
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
        }

        private void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(AnimDecoder));
        }

        private static byte[] ReadStreamFully(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (stream is MemoryStream ms && ms.Position == 0)
                return ms.ToArray();
            using (var output = new MemoryStream())
            {
                byte[] buffer = new byte[8192];
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                    output.Write(buffer, 0, read);
                return output.ToArray();
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_decoder != IntPtr.Zero)
                {
                    NativeLibraryLoader.FixDllNotFoundException("webpdemux", () =>
                    {
                        NativeMethods.WebPAnimDecoderDelete(_decoder);
                        return 0;
                    });
                    _decoder = IntPtr.Zero;
                }
                if (_dataHandle.IsAllocated)
                    _dataHandle.Free();

                _disposed = true;
            }
        }
    }
}
