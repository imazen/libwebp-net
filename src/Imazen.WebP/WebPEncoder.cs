using System;
using System.IO;
using System.Runtime.InteropServices;
using Imazen.WebP.Extern;

namespace Imazen.WebP
{
    /// <summary>
    /// Raw buffer WebP encoder. No System.Drawing dependency — works on all platforms.
    /// </summary>
    public static class WebPEncoder
    {
        // Prevent delegate from being GC'd during encoding
        private static readonly WebPWriterFunction _writerDelegate = ManagedWriter;

        private sealed class EncodeOutput(Stream stream)
        {
            public readonly Stream Stream = stream;
        }

        private static int ManagedWriter(IntPtr data, UIntPtr dataSize, ref WebPPicture picture)
        {
            int size = (int)(uint)dataSize;
            if (size <= 0) return 1;
            var handle = GCHandle.FromIntPtr(picture.custom_ptr);
            var ctx = (EncodeOutput)handle.Target!;
#if NETCOREAPP
            unsafe {
                ctx.Stream.Write(new ReadOnlySpan<byte>((void*)data, size));
            }
#else
            byte[] buffer = new byte[size];
            Marshal.Copy(data, buffer, 0, size);
            ctx.Stream.Write(buffer, 0, size);
#endif
            return 1;
        }

        /// <summary>
        /// Encodes raw pixel data to WebP format. Returns the encoded bytes.
        /// </summary>
        /// <param name="pixels">Raw pixel data</param>
        /// <param name="width">Image width in pixels</param>
        /// <param name="height">Image height in pixels</param>
        /// <param name="stride">Byte stride (bytes per row)</param>
        /// <param name="format">Pixel format of the input data</param>
        /// <param name="quality">0-100 for lossy, -1 for lossless</param>
        /// <returns>Encoded WebP data</returns>
        public static byte[] Encode(byte[] pixels, int width, int height, int stride,
            WebPPixelFormat format, float quality)
        {
            if (pixels == null) throw new ArgumentNullException(nameof(pixels));
            if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
            if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));
            ValidatePixelBuffer(pixels, width, height, stride, format);

            var handle = GCHandle.Alloc(pixels, GCHandleType.Pinned);
            try
            {
                IntPtr dataPtr = handle.AddrOfPinnedObject();
                IntPtr result = IntPtr.Zero;
                UIntPtr length;

                if (quality < 0)
                {
                    length = EncodeLossless(dataPtr, width, height, stride, format, ref result);
                }
                else
                {
                    if (quality > 100) quality = 100;
                    length = EncodeLossy(dataPtr, width, height, stride, format, quality, ref result);
                }

                if ((ulong)length == 0 || result == IntPtr.Zero)
                    throw new Exception("WebP encode failed!");

                try
                {
                    ulong len = (ulong)length;
                    if (len > int.MaxValue)
                        throw new InvalidOperationException(
                            $"libwebp produced {len} bytes — exceeds int.MaxValue and cannot be returned as a byte[].");
                    byte[] output = new byte[(int)len];
                    Marshal.Copy(result, output, 0, output.Length);
                    return output;
                }
                finally
                {
                    NativeMethods.WebPSafeFree(result);
                }
            }
            finally
            {
                handle.Free();
            }
        }

        /// <summary>
        /// Encodes raw pixel data to WebP format and writes to a stream.
        /// </summary>
        public static void Encode(byte[] pixels, int width, int height, int stride,
            WebPPixelFormat format, float quality, Stream output)
        {
            if (output == null) throw new ArgumentNullException(nameof(output));
            byte[] encoded = Encode(pixels, width, height, stride, format, quality);
            output.Write(encoded, 0, encoded.Length);
        }

        private static UIntPtr EncodeLossy(IntPtr data, int width, int height, int stride,
            WebPPixelFormat format, float quality, ref IntPtr result)
        {
            // We can't pass ref through a lambda, so we do a local copy pattern
            IntPtr res = result;
            UIntPtr length;
            switch (format)
            {
                case WebPPixelFormat.Bgra:
                    length = NativeLibraryLoader.FixDllNotFoundException("webp",
                        () => NativeMethods.WebPEncodeBGRA(data, width, height, stride, quality, ref res))!;
                    break;
                case WebPPixelFormat.Rgba:
                    length = NativeLibraryLoader.FixDllNotFoundException("webp",
                        () => NativeMethods.WebPEncodeRGBA(data, width, height, stride, quality, ref res))!;
                    break;
                case WebPPixelFormat.Bgr:
                    length = NativeLibraryLoader.FixDllNotFoundException("webp",
                        () => NativeMethods.WebPEncodeBGR(data, width, height, stride, quality, ref res))!;
                    break;
                case WebPPixelFormat.Rgb:
                    length = NativeLibraryLoader.FixDllNotFoundException("webp",
                        () => NativeMethods.WebPEncodeRGB(data, width, height, stride, quality, ref res))!;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(format));
            }
            result = res;
            return length;
        }

        private static UIntPtr EncodeLossless(IntPtr data, int width, int height, int stride,
            WebPPixelFormat format, ref IntPtr result)
        {
            IntPtr res = result;
            UIntPtr length;
            switch (format)
            {
                case WebPPixelFormat.Bgra:
                    length = NativeLibraryLoader.FixDllNotFoundException("webp",
                        () => NativeMethods.WebPEncodeLosslessBGRA(data, width, height, stride, ref res))!;
                    break;
                case WebPPixelFormat.Rgba:
                    length = NativeLibraryLoader.FixDllNotFoundException("webp",
                        () => NativeMethods.WebPEncodeLosslessRGBA(data, width, height, stride, ref res))!;
                    break;
                case WebPPixelFormat.Bgr:
                    length = NativeLibraryLoader.FixDllNotFoundException("webp",
                        () => NativeMethods.WebPEncodeLosslessBGR(data, width, height, stride, ref res))!;
                    break;
                case WebPPixelFormat.Rgb:
                    length = NativeLibraryLoader.FixDllNotFoundException("webp",
                        () => NativeMethods.WebPEncodeLosslessRGB(data, width, height, stride, ref res))!;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(format));
            }
            result = res;
            return length;
        }

        /// <summary>
        /// Encodes raw pixel data using advanced WebPEncoderConfig settings.
        /// Uses the full WebPPicture/WebPConfig pipeline for maximum control.
        /// </summary>
        /// <param name="pixels">Raw pixel data</param>
        /// <param name="width">Image width in pixels</param>
        /// <param name="height">Image height in pixels</param>
        /// <param name="stride">Byte stride (bytes per row)</param>
        /// <param name="format">Pixel format of the input data</param>
        /// <param name="config">Advanced encoder configuration</param>
        /// <returns>Encoded WebP data</returns>
        public static byte[] Encode(byte[] pixels, int width, int height, int stride,
            WebPPixelFormat format, WebPEncoderConfig config)
        {
            var outputStream = new MemoryStream();
            Encode(pixels, width, height, stride, format, config, outputStream);
            return outputStream.ToArray();
        }

        /// <summary>
        /// Encodes raw pixel data using advanced WebPEncoderConfig settings and writes to a stream.
        /// </summary>
        public static void Encode(byte[] pixels, int width, int height, int stride,
            WebPPixelFormat format, WebPEncoderConfig config, Stream outputStream)
        {
            if (pixels == null) throw new ArgumentNullException(nameof(pixels));
            if (config == null) throw new ArgumentNullException(nameof(config));
            if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
            if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));
            if (outputStream == null) throw new ArgumentNullException(nameof(outputStream));
            ValidatePixelBuffer(pixels, width, height, stride, format);

            if (!config.Validate())
                throw new ArgumentException("Invalid WebP encoder configuration", nameof(config));

            var nativeConfig = config.GetNativeConfig();
            var picture = new WebPPicture();

            NativeLibraryLoader.FixDllNotFoundException("webp", () =>
            {
                if (NativeMethods.WebPPictureInitInternal(ref picture, NativeMethods.WEBP_ENCODER_ABI_VERSION) == 0)
                    throw new Exception("WebP version mismatch: failed to initialize picture");
                return 0;
            });

            picture.width = width;
            picture.height = height;
            picture.use_argb = 1;

            var pixelHandle = GCHandle.Alloc(pixels, GCHandleType.Pinned);
            var output = new EncodeOutput(outputStream);
            var outputHandle = GCHandle.Alloc(output);
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

                picture.writer = _writerDelegate;
                picture.custom_ptr = GCHandle.ToIntPtr(outputHandle);

                int encodeResult = NativeLibraryLoader.FixDllNotFoundException("webp",
                    () => NativeMethods.WebPEncode(ref nativeConfig, ref picture));

                if (encodeResult == 0)
                    throw new Exception($"WebP encode failed with error: {picture.error_code}");
            }
            finally
            {
                NativeLibraryLoader.FixDllNotFoundException("webp", () =>
                {
                    NativeMethods.WebPPictureFree(ref picture);
                    return 0;
                });
                pixelHandle.Free();
                outputHandle.Free();
            }
        }

        // Guards libwebp against an out-of-bounds read into the managed heap.
        // libwebp reads stride * height bytes from the pinned pointer; the
        // managed buffer must be at least that large. We also enforce the
        // libwebp dimension cap so width*height won't silently wrap.
        private static void ValidatePixelBuffer(byte[] pixels, int width, int height, int stride, WebPPixelFormat format)
        {
            if (width > NativeMethods.WEBP_MAX_DIMENSION || height > NativeMethods.WEBP_MAX_DIMENSION)
                throw new ArgumentOutOfRangeException(nameof(width),
                    $"width/height must be <= {NativeMethods.WEBP_MAX_DIMENSION}; got {width}x{height}.");

            int bytesPerPixel = (format == WebPPixelFormat.Rgb || format == WebPPixelFormat.Bgr) ? 3 : 4;
            long minStride = (long)width * bytesPerPixel;
            // libwebp accepts negative strides only for bottom-up bitmaps in
            // some entry points; we don't expose those entry points and require
            // a positive stride large enough for one row.
            if (stride < minStride)
                throw new ArgumentOutOfRangeException(nameof(stride),
                    $"stride {stride} is smaller than width*bytesPerPixel ({minStride}).");

            long needed = (long)stride * height;
            if (pixels.Length < needed)
                throw new ArgumentException(
                    $"pixels.Length {pixels.Length} is smaller than stride*height ({needed}).",
                    nameof(pixels));
        }
    }
}
