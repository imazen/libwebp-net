using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using Imazen.WebP.Extern;

namespace Imazen.WebP {
    /// <summary>
    /// Encodes Bitmap objects into WebP format
    /// </summary>
    public class SimpleEncoder {
        public SimpleEncoder() { }

        public static string GetEncoderVersion()
        {
            uint v = (uint)NativeLibraryLoader.FixDllNotFoundException("webp",
                () => NativeMethods.WebPGetEncoderVersion())!;
            var revision = v % 256;
            var minor = (v >> 8) % 256;
            var major = (v >> 16) % 256;
            return major + "." + minor + "." + revision;
        }

        /// <summary>
        /// Encodes the given RGB(A) bitmap to the given stream. Specify quality = -1 for lossless, otherwise specify a value between 0 and 100.
        /// </summary>
        [Obsolete("Use Encode(Bitmap, Stream, float) instead")]
        public void Encode(Bitmap from, Stream to, float quality, bool noAlpha) {
            Encode(from, to, quality);
        }

        /// <summary>
        /// Encodes the given RGB(A) bitmap to the given stream. Specify quality = -1 for lossless, otherwise specify a value between 0 and 100.
        /// </summary>
        public void Encode(Bitmap from, Stream to, float quality) {
            IntPtr result;
            long length;

            Encode(from, quality, out result, out length);
            try {
                byte[] buffer = new byte[4096];
                for (int i = 0; i < length; i += buffer.Length) {
                    int used = (int)Math.Min((int)buffer.Length, length - i);
                    Marshal.Copy((IntPtr)((long)result + i), buffer, 0, used);
                    to.Write(buffer, 0, used);
                }
            } finally {
                NativeMethods.WebPSafeFree(result);
            }
        }

        /// <summary>
        /// Encodes the given RGB(A) bitmap to an unmanaged memory buffer.
        /// Specify quality = -1 for lossless, otherwise specify a value between 0 and 100.
        /// </summary>
        [Obsolete("Use Encode(Bitmap, float, out IntPtr, out long) instead")]
        public void Encode(Bitmap b, float quality, bool noAlpha, out IntPtr result, out long length) {
            Encode(b, quality, out result, out length);
        }

        /// <summary>
        /// Encodes the given RGB(A) bitmap to an unmanaged memory buffer.
        /// Specify quality = -1 for lossless, otherwise specify a value between 0 and 100.
        /// </summary>
        public void Encode(Bitmap b, float quality, out IntPtr result, out long length) {
            if (quality < -1) quality = -1;
            if (quality > 100) quality = 100;
            int w = b.Width;
            int h = b.Height;
            var bd = b.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, b.PixelFormat);
            try {
                result = IntPtr.Zero;
                IntPtr scan0 = bd.Scan0;
                int stride = bd.Stride;

                if (b.PixelFormat == PixelFormat.Format32bppArgb) {
                    IntPtr res = IntPtr.Zero;
                    if (quality == -1)
                        length = (long)NativeLibraryLoader.FixDllNotFoundException("webp",
                            () => NativeMethods.WebPEncodeLosslessBGRA(scan0, w, h, stride, ref res))!;
                    else
                        length = (long)NativeLibraryLoader.FixDllNotFoundException("webp",
                            () => NativeMethods.WebPEncodeBGRA(scan0, w, h, stride, quality, ref res))!;
                    result = res;
                } else if (b.PixelFormat == PixelFormat.Format24bppRgb) {
                    IntPtr res = IntPtr.Zero;
                    if (quality == -1)
                        length = (long)NativeLibraryLoader.FixDllNotFoundException("webp",
                            () => NativeMethods.WebPEncodeLosslessBGR(scan0, w, h, stride, ref res))!;
                    else
                        length = (long)NativeLibraryLoader.FixDllNotFoundException("webp",
                            () => NativeMethods.WebPEncodeBGR(scan0, w, h, stride, quality, ref res))!;
                    result = res;
                } else {
                    using (Bitmap b2 = b.Clone(new Rectangle(0, 0, b.Width, b.Height), PixelFormat.Format32bppArgb))
                    {
                        Encode(b2, quality, out result, out length);
                    }
                }
                if (length == 0) throw new Exception("WebP encode failed!");
            } finally {
                b.UnlockBits(bd);
            }
        }

        /// <summary>
        /// Encodes the given RGB(A) bitmap using advanced WebPEncoderConfig settings.
        /// Uses the full WebPPicture/WebPConfig pipeline for maximum control.
        /// </summary>
        public void Encode(Bitmap b, Stream to, WebPEncoderConfig config)
        {
            if (b == null) throw new ArgumentNullException(nameof(b));
            if (to == null) throw new ArgumentNullException(nameof(to));
            if (config == null) throw new ArgumentNullException(nameof(config));

            int w = b.Width;
            int h = b.Height;

            // Ensure we have a compatible pixel format
            Bitmap source = b;
            bool ownsSource = false;
            if (b.PixelFormat != PixelFormat.Format32bppArgb && b.PixelFormat != PixelFormat.Format24bppRgb)
            {
                source = b.Clone(new Rectangle(0, 0, b.Width, b.Height), PixelFormat.Format32bppArgb);
                ownsSource = true;
            }

            try
            {
                var bd = source.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, source.PixelFormat);
                try
                {
                    int bpp = source.PixelFormat == PixelFormat.Format24bppRgb ? 3 : 4;
                    int stride = bd.Stride;
                    byte[] pixels = new byte[Math.Abs(stride) * h];
                    Marshal.Copy(bd.Scan0, pixels, 0, pixels.Length);

                    WebPPixelFormat fmt = source.PixelFormat == PixelFormat.Format24bppRgb
                        ? WebPPixelFormat.Bgr
                        : WebPPixelFormat.Bgra;

                    WebPEncoder.Encode(pixels, w, h, Math.Abs(stride), fmt, config, to);
                }
                finally
                {
                    source.UnlockBits(bd);
                }
            }
            finally
            {
                if (ownsSource) source.Dispose();
            }
        }
    }
}
