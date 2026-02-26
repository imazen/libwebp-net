using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using Imazen.WebP.Extern;

namespace Imazen.WebP {

    public class SimpleDecoder {

        public static string GetDecoderVersion()
        {
            uint v = (uint)NativeLibraryLoader.FixDllNotFoundException("webp",
                () => NativeMethods.WebPGetDecoderVersion())!;
            var revision = v % 256;
            var minor = (v >> 8) % 256;
            var major = (v >> 16) % 256;
            return major + "." + minor + "." + revision;
        }

        public SimpleDecoder() { }

        public unsafe Bitmap DecodeFromBytes(byte[] data, long length) {
            fixed (byte* dataptr = data) {
                return DecodeFromPointer((IntPtr)dataptr, length);
            }
        }

        public Bitmap DecodeFromPointer(IntPtr data, long length) {
            int w = 0, h = 0;
            // Validate header and determine size
            if (NativeLibraryLoader.FixDllNotFoundException("webp",
                () => NativeMethods.WebPGetInfo(data, (UIntPtr)length, ref w, ref h)) == 0)
                throw new Exception("Invalid WebP header detected");

            bool success = false;
            Bitmap? b = null;
            BitmapData? bd = null;
            try {
                b = new Bitmap(w, h, PixelFormat.Format32bppArgb);
                bd = b.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                IntPtr result = NativeLibraryLoader.FixDllNotFoundException("webp",
                    () => NativeMethods.WebPDecodeBGRAInto(data, (UIntPtr)length, bd.Scan0, (UIntPtr)(bd.Stride * bd.Height), bd.Stride))!;
                if (bd.Scan0 != result) throw new Exception("Failed to decode WebP image with error " + (long)result);
                success = true;
            } finally {
                if (bd != null && b != null) b.UnlockBits(bd);
                if (!success && b != null) b.Dispose();
            }
            return b;
        }

        /// <summary>
        /// Decodes a WebP image from a stream, returning a Bitmap.
        /// Reads the entire stream into memory first.
        /// </summary>
        public Bitmap DecodeFromStream(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            byte[] data = ReadStreamFully(stream);
            return DecodeFromBytes(data, data.LongLength);
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
    }
}
