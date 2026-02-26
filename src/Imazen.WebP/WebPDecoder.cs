using System;
using System.IO;
using System.Runtime.InteropServices;
using Imazen.WebP.Extern;

namespace Imazen.WebP
{
    /// <summary>
    /// Pixel format for raw buffer encode/decode operations.
    /// </summary>
    public enum WebPPixelFormat
    {
        Bgra,
        Rgba,
        Bgr,
        Rgb
    }

    /// <summary>
    /// Raw buffer WebP decoder. No System.Drawing dependency — works on all platforms.
    /// </summary>
    public static class WebPDecoder
    {
        /// <summary>
        /// Decodes WebP data to a BGRA pixel buffer.
        /// Returns the pixel data as a byte array.
        /// </summary>
        public static byte[] Decode(byte[] data, out int width, out int height)
        {
            return Decode(data, out width, out height, WebPPixelFormat.Bgra);
        }

        /// <summary>
        /// Decodes WebP data to a pixel buffer in the specified format.
        /// </summary>
        public static byte[] Decode(byte[] data, out int width, out int height, WebPPixelFormat format)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            // Pin data and get info
            var dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                IntPtr dataPtr = dataHandle.AddrOfPinnedObject();
                UIntPtr dataSize = (UIntPtr)data.Length;

                int w = 0, h = 0;
                if (NativeLibraryLoader.FixDllNotFoundException("webp",
                    () => NativeMethods.WebPGetInfo(dataPtr, dataSize, ref w, ref h)) == 0)
                    throw new Exception("Invalid WebP header detected");

                width = w;
                height = h;

                int bytesPerPixel = (format == WebPPixelFormat.Bgr || format == WebPPixelFormat.Rgb) ? 3 : 4;
                int stride = w * bytesPerPixel;
                byte[] output = new byte[stride * h];

                var outHandle = GCHandle.Alloc(output, GCHandleType.Pinned);
                try
                {
                    IntPtr outPtr = outHandle.AddrOfPinnedObject();
                    UIntPtr outSize = (UIntPtr)output.Length;

                    IntPtr result = DecodeInto(dataPtr, dataSize, outPtr, outSize, stride, format);
                    if (outPtr != result)
                        throw new Exception("Failed to decode WebP image");
                }
                finally
                {
                    outHandle.Free();
                }

                return output;
            }
            finally
            {
                dataHandle.Free();
            }
        }

        /// <summary>
        /// Decodes WebP data directly into a caller-provided buffer.
        /// </summary>
        public static void Decode(byte[] data, byte[] output, int stride, WebPPixelFormat format)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (output == null) throw new ArgumentNullException(nameof(output));

            var dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
            var outHandle = GCHandle.Alloc(output, GCHandleType.Pinned);
            try
            {
                IntPtr dataPtr = dataHandle.AddrOfPinnedObject();
                IntPtr outPtr = outHandle.AddrOfPinnedObject();
                UIntPtr dataSize = (UIntPtr)data.Length;
                UIntPtr outSize = (UIntPtr)output.Length;

                IntPtr result = DecodeInto(dataPtr, dataSize, outPtr, outSize, stride, format);
                if (outPtr != result)
                    throw new Exception("Failed to decode WebP image");
            }
            finally
            {
                outHandle.Free();
                dataHandle.Free();
            }
        }

        /// <summary>
        /// Decodes WebP data from a stream to a BGRA pixel buffer.
        /// Reads the entire stream into memory first.
        /// </summary>
        public static byte[] DecodeFromStream(Stream stream, out int width, out int height)
        {
            return DecodeFromStream(stream, out width, out height, WebPPixelFormat.Bgra);
        }

        /// <summary>
        /// Decodes WebP data from a stream to a pixel buffer in the specified format.
        /// Reads the entire stream into memory first.
        /// </summary>
        public static byte[] DecodeFromStream(Stream stream, out int width, out int height, WebPPixelFormat format)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            byte[] data = ReadStreamFully(stream);
            return Decode(data, out width, out height, format);
        }

        /// <summary>
        /// Checks whether the given data starts with a valid WebP header (RIFF...WEBP).
        /// </summary>
        public static bool IsWebP(byte[] data)
        {
            if (data == null || data.Length < 12) return false;
            return data[0] == (byte)'R' && data[1] == (byte)'I' &&
                   data[2] == (byte)'F' && data[3] == (byte)'F' &&
                   data[8] == (byte)'W' && data[9] == (byte)'E' &&
                   data[10] == (byte)'B' && data[11] == (byte)'P';
        }

        /// <summary>
        /// Checks whether the given stream starts with a valid WebP header.
        /// The stream position is restored after checking.
        /// </summary>
        public static bool IsWebP(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (!stream.CanSeek) throw new ArgumentException("Stream must be seekable", nameof(stream));

            long pos = stream.Position;
            try
            {
                byte[] header = new byte[12];
                int read = 0;
                while (read < 12)
                {
                    int n = stream.Read(header, read, 12 - read);
                    if (n == 0) return false;
                    read += n;
                }
                return IsWebP(header);
            }
            finally
            {
                stream.Position = pos;
            }
        }

        private static byte[] ReadStreamFully(Stream stream)
        {
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

        private static IntPtr DecodeInto(IntPtr dataPtr, UIntPtr dataSize, IntPtr outPtr, UIntPtr outSize, int stride, WebPPixelFormat format)
        {
            switch (format)
            {
                case WebPPixelFormat.Bgra:
                    return NativeLibraryLoader.FixDllNotFoundException("webp",
                        () => NativeMethods.WebPDecodeBGRAInto(dataPtr, dataSize, outPtr, outSize, stride))!;
                case WebPPixelFormat.Rgba:
                    return NativeLibraryLoader.FixDllNotFoundException("webp",
                        () => NativeMethods.WebPDecodeRGBAInto(dataPtr, dataSize, outPtr, outSize, stride))!;
                case WebPPixelFormat.Bgr:
                    return NativeLibraryLoader.FixDllNotFoundException("webp",
                        () => NativeMethods.WebPDecodeBGRInto(dataPtr, dataSize, outPtr, outSize, stride))!;
                case WebPPixelFormat.Rgb:
                    return NativeLibraryLoader.FixDllNotFoundException("webp",
                        () => NativeMethods.WebPDecodeRGBInto(dataPtr, dataSize, outPtr, outSize, stride))!;
                default:
                    throw new ArgumentOutOfRangeException(nameof(format));
            }
        }
    }
}
