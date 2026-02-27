using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Imazen.WebP.Extern;

namespace Imazen.WebP
{
    /// <summary>
    /// Encodes multiple frames into an animated WebP image.
    /// Uses libwebpmux's WebPAnimEncoder internally.
    /// </summary>
    public class AnimEncoder : IDisposable
    {
        private IntPtr _encoder;
        private readonly int _width;
        private readonly int _height;
        private bool _disposed;
        private int _lastTimestamp;
        private int _lastDuration = 100; // default duration for the last frame
        private bool _hasFrames;

        /// <summary>
        /// Creates an animation encoder with default options.
        /// </summary>
        /// <param name="width">Canvas width in pixels.</param>
        /// <param name="height">Canvas height in pixels.</param>
        public AnimEncoder(int width, int height)
        {
            if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
            if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));

            _width = width;
            _height = height;

            var options = new WebPAnimEncoderOptions();
            NativeLibraryLoader.FixDllNotFoundException("webpmux", () =>
            {
                if (NativeMethods.WebPAnimEncoderOptionsInit(ref options) == 0)
                    throw new Exception("Failed to initialize animation encoder options (version mismatch)");
                return 0;
            });

            _encoder = NativeLibraryLoader.FixDllNotFoundException("webpmux",
                () => NativeMethods.WebPAnimEncoderNew(width, height, ref options));

            if (_encoder == IntPtr.Zero)
                throw new Exception("Failed to create animation encoder");
        }

        /// <summary>
        /// Creates an animation encoder with custom options.
        /// </summary>
        /// <param name="width">Canvas width in pixels.</param>
        /// <param name="height">Canvas height in pixels.</param>
        /// <param name="loopCount">Number of loop iterations (0 = infinite).</param>
        /// <param name="backgroundColor">Background color as BGRA packed uint32.</param>
        /// <param name="allowMixed">Allow mixing lossy and lossless frames.</param>
        /// <param name="minimizeSize">Minimize output size (slower).</param>
        public AnimEncoder(int width, int height, int loopCount = 0, uint backgroundColor = 0,
            bool allowMixed = false, bool minimizeSize = false)
        {
            if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
            if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));

            _width = width;
            _height = height;

            var options = new WebPAnimEncoderOptions();
            NativeLibraryLoader.FixDllNotFoundException("webpmux", () =>
            {
                if (NativeMethods.WebPAnimEncoderOptionsInit(ref options) == 0)
                    throw new Exception("Failed to initialize animation encoder options (version mismatch)");
                return 0;
            });

            options.anim_params.loop_count = loopCount;
            options.anim_params.bgcolor = backgroundColor;
            options.allow_mixed = allowMixed ? 1 : 0;
            options.minimize_size = minimizeSize ? 1 : 0;

            _encoder = NativeLibraryLoader.FixDllNotFoundException("webpmux",
                () => NativeMethods.WebPAnimEncoderNew(width, height, ref options));

            if (_encoder == IntPtr.Zero)
                throw new Exception("Failed to create animation encoder");
        }

        /// <summary>
        /// Adds a frame of raw BGRA pixel data at the given timestamp.
        /// </summary>
        /// <param name="bgraPixels">BGRA pixel data (width * height * 4 bytes).</param>
        /// <param name="timestampMs">Timestamp in milliseconds for this frame.</param>
        /// <param name="quality">0-100 for lossy, -1 for lossless.</param>
        public void AddFrame(byte[] bgraPixels, int timestampMs, float quality = -1)
        {
            if (bgraPixels == null) throw new ArgumentNullException(nameof(bgraPixels));
            AddFrameInternal(bgraPixels, _width * 4, WebPPixelFormat.Bgra, timestampMs, quality);
        }

        /// <summary>
        /// Adds a frame of raw pixel data at the given timestamp.
        /// </summary>
        /// <param name="pixels">Raw pixel data.</param>
        /// <param name="stride">Byte stride per row.</param>
        /// <param name="format">Pixel format of the input data.</param>
        /// <param name="timestampMs">Timestamp in milliseconds for this frame.</param>
        /// <param name="quality">0-100 for lossy, -1 for lossless.</param>
        public void AddFrame(byte[] pixels, int stride, WebPPixelFormat format, int timestampMs, float quality = -1)
        {
            if (pixels == null) throw new ArgumentNullException(nameof(pixels));
            AddFrameInternal(pixels, stride, format, timestampMs, quality);
        }

        /// <summary>
        /// Adds a frame using an advanced encoder config.
        /// </summary>
        public void AddFrame(byte[] pixels, int stride, WebPPixelFormat format, int timestampMs, WebPEncoderConfig config)
        {
            if (pixels == null) throw new ArgumentNullException(nameof(pixels));
            if (config == null) throw new ArgumentNullException(nameof(config));
            ThrowIfDisposed();

            if (!config.Validate())
                throw new ArgumentException("Invalid encoder configuration", nameof(config));

            var nativeConfig = config.GetNativeConfig();
            AddFrameWithConfig(pixels, stride, format, timestampMs, ref nativeConfig);
        }

        private void AddFrameInternal(byte[] pixels, int stride, WebPPixelFormat format, int timestampMs, float quality)
        {
            ThrowIfDisposed();

            var config = new WebPConfig();
            NativeLibraryLoader.FixDllNotFoundException("webp", () =>
                NativeMethods.WebPConfigInitInternal(ref config, WebPPreset.WEBP_PRESET_DEFAULT,
                    quality >= 0 ? quality : 75f, NativeMethods.WEBP_ENCODER_ABI_VERSION));

            if (quality < 0)
            {
                config.lossless = 1;
                config.quality = 75;
            }
            else
            {
                config.lossless = 0;
                config.quality = Math.Max(0, Math.Min(100, quality));
            }

            AddFrameWithConfig(pixels, stride, format, timestampMs, ref config);
        }

        private void AddFrameWithConfig(byte[] pixels, int stride, WebPPixelFormat format, int timestampMs, ref WebPConfig config)
        {
            // Local copy to avoid ref-in-lambda issue (CS1628)
            var localConfig = config;

            var picture = new WebPPicture();
            NativeLibraryLoader.FixDllNotFoundException("webp", () =>
            {
                if (NativeMethods.WebPPictureInitInternal(ref picture, NativeMethods.WEBP_ENCODER_ABI_VERSION) == 0)
                    throw new Exception("Failed to initialize WebPPicture (version mismatch)");
                return 0;
            });

            picture.width = _width;
            picture.height = _height;
            picture.use_argb = 1;

            var pixelHandle = GCHandle.Alloc(pixels, GCHandleType.Pinned);
            try
            {
                IntPtr pixelPtr = pixelHandle.AddrOfPinnedObject();

                int importResult;
                switch (format)
                {
                    case WebPPixelFormat.Bgra:
                        importResult = NativeLibraryLoader.FixDllNotFoundException("webp",
                            () => NativeMethods.WebPPictureImportBGRA(ref picture, pixelPtr, stride));
                        break;
                    case WebPPixelFormat.Rgba:
                        importResult = NativeLibraryLoader.FixDllNotFoundException("webp",
                            () => NativeMethods.WebPPictureImportRGBA(ref picture, pixelPtr, stride));
                        break;
                    case WebPPixelFormat.Bgr:
                        importResult = NativeLibraryLoader.FixDllNotFoundException("webp",
                            () => NativeMethods.WebPPictureImportBGR(ref picture, pixelPtr, stride));
                        break;
                    case WebPPixelFormat.Rgb:
                        importResult = NativeLibraryLoader.FixDllNotFoundException("webp",
                            () => NativeMethods.WebPPictureImportRGB(ref picture, pixelPtr, stride));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(format));
                }
                if (importResult == 0)
                    throw new Exception("Failed to import pixel data into WebPPicture");

                int addResult = NativeLibraryLoader.FixDllNotFoundException("webpmux",
                    () => NativeMethods.WebPAnimEncoderAdd(_encoder, ref picture, timestampMs, ref localConfig));

                if (addResult == 0)
                {
                    IntPtr errPtr = NativeMethods.WebPAnimEncoderGetError(_encoder);
                    string error = errPtr != IntPtr.Zero ? Marshal.PtrToStringAnsi(errPtr) ?? "Unknown error" : "Unknown error";
                    throw new Exception($"Failed to add animation frame: {error}");
                }

                if (_hasFrames)
                    _lastDuration = Math.Max(timestampMs - _lastTimestamp, 1);
                _lastTimestamp = timestampMs;
                _hasFrames = true;
            }
            finally
            {
                NativeLibraryLoader.FixDllNotFoundException("webp", () =>
                {
                    NativeMethods.WebPPictureFree(ref picture);
                    return 0;
                });
                pixelHandle.Free();
            }
        }

        /// <summary>
        /// Assembles all added frames into the final animated WebP data.
        /// </summary>
        /// <returns>Encoded animated WebP bytes.</returns>
        public byte[] Assemble()
        {
            ThrowIfDisposed();

            // Signal end of frames. The timestamp here is the end time of the last frame.
            // We add at least 1ms beyond _lastTimestamp to give the last frame a non-zero
            // duration, preventing the encoder from dropping it.
            int endTimestamp = _lastTimestamp + Math.Max(_lastDuration, 1);
            NativeLibraryLoader.FixDllNotFoundException("webpmux", () =>
                NativeMethods.WebPAnimEncoderAddNull(_encoder, IntPtr.Zero, endTimestamp, IntPtr.Zero));

            var webpData = new WebPData();
            int result = NativeLibraryLoader.FixDllNotFoundException("webpmux",
                () => NativeMethods.WebPAnimEncoderAssemble(_encoder, ref webpData));

            if (result == 0)
            {
                IntPtr errPtr = NativeMethods.WebPAnimEncoderGetError(_encoder);
                string error = errPtr != IntPtr.Zero ? Marshal.PtrToStringAnsi(errPtr) ?? "Unknown error" : "Unknown error";
                throw new Exception($"Failed to assemble animation: {error}");
            }

            // Copy to managed array (data is owned by encoder, freed on delete)
            int size = (int)(ulong)webpData.size;
            byte[] output = new byte[size];
            Marshal.Copy(webpData.bytes, output, 0, size);

            return output;
        }

        /// <summary>
        /// Assembles and writes to a stream.
        /// </summary>
        public void Assemble(Stream outputStream)
        {
            if (outputStream == null) throw new ArgumentNullException(nameof(outputStream));
            byte[] data = Assemble();
            outputStream.Write(data, 0, data.Length);
        }

        private void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(AnimEncoder));
        }

        public void Dispose()
        {
            if (!_disposed && _encoder != IntPtr.Zero)
            {
                NativeLibraryLoader.FixDllNotFoundException("webpmux", () =>
                {
                    NativeMethods.WebPAnimEncoderDelete(_encoder);
                    return 0;
                });
                _encoder = IntPtr.Zero;
                _disposed = true;
            }
        }
    }
}
