using System;
using System.Runtime.InteropServices;
using Imazen.WebP.Extern;

namespace Imazen.WebP
{
    /// <summary>
    /// Information about a WebP image, retrieved without full decoding.
    /// </summary>
    public class WebPImageInfo
    {
        /// <summary>Image width in pixels.</summary>
        public int Width { get; }
        /// <summary>Image height in pixels.</summary>
        public int Height { get; }
        /// <summary>Whether the image contains an alpha channel.</summary>
        public bool HasAlpha { get; }
        /// <summary>Whether the image is an animated WebP.</summary>
        public bool HasAnimation { get; }
        /// <summary>
        /// 0 = undefined/mixed, 1 = lossy, 2 = lossless
        /// </summary>
        public int Format { get; }

        internal WebPImageInfo(int width, int height, bool hasAlpha, bool hasAnimation, int format)
        {
            Width = width;
            Height = height;
            HasAlpha = hasAlpha;
            HasAnimation = hasAnimation;
            Format = format;
        }
    }

    /// <summary>
    /// Static utility class for probing WebP bitstream features without full decoding.
    /// </summary>
    public static class WebPInfo
    {
        /// <summary>
        /// Retrieves basic header information (width, height) from WebP data.
        /// Returns false if the header is invalid.
        /// </summary>
        public static bool TryGetSize(byte[] data, out int width, out int height)
        {
            width = 0;
            height = 0;
            if (data == null || data.Length < 12) return false;

            var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                IntPtr ptr = handle.AddrOfPinnedObject();
                UIntPtr size = (UIntPtr)data.Length;
                int w = 0, h = 0;
                int result = NativeLibraryLoader.FixDllNotFoundException("webp",
                    () => NativeMethods.WebPGetInfo(ptr, size, ref w, ref h));
                width = w;
                height = h;
                return result != 0;
            }
            finally
            {
                handle.Free();
            }
        }

        /// <summary>
        /// Retrieves detailed bitstream features (dimensions, alpha, animation, format).
        /// </summary>
        public static WebPImageInfo GetImageInfo(byte[] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (data.Length < 12) throw new ArgumentException("Data too short to be a valid WebP file", nameof(data));

            var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                IntPtr ptr = handle.AddrOfPinnedObject();
                return GetImageInfo(ptr, data.Length);
            }
            finally
            {
                handle.Free();
            }
        }

        /// <summary>
        /// Retrieves detailed bitstream features from an IntPtr.
        /// </summary>
        public static WebPImageInfo GetImageInfo(IntPtr data, long length)
        {
            if (data == IntPtr.Zero) throw new ArgumentNullException(nameof(data));
            if (length < 12) throw new ArgumentException("Data too short to be a valid WebP file", nameof(length));

            var features = new WebPBitstreamFeatures();
            var status = NativeLibraryLoader.FixDllNotFoundException("webp",
                () => NativeMethods.WebPGetFeatures(data, (UIntPtr)length, ref features));

            if (status != VP8StatusCode.VP8_STATUS_OK)
                throw new Exception($"Failed to get WebP features: {status}");

            return new WebPImageInfo(
                features.width,
                features.height,
                features.has_alpha != 0,
                features.has_animation != 0,
                features.format);
        }
    }
}
