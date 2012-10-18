using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using Imazen.WebP.Extern;
using System.IO;

namespace Imazen.WebP {
    public class SimpleEncoder {
        public SimpleEncoder() { }


        /// <summary>
        /// Encodes the given RGB(A) bitmap to the given stream. Specify quality = -1 for lossless, otherwise specify a value between 0 and 100.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="quality"></param>
        public void Encode(Bitmap from, Stream to, float quality) {
            IntPtr result;
            long length;

            Encode(from, quality, out result, out length);
            try {
                byte[] buffer = new byte[4096];
                for (int i = 0; i < length; i += buffer.Length) {
                    int used = (int)Math.Min((int)buffer.Length, length - i);
                    Marshal.Copy(result, buffer, i, used);
                    to.Write(buffer, 0, used);
                }
            } finally {
                NativeMethods.WebPFree(result);
            }

        }

        public void Encode(Bitmap b, float quality, out IntPtr result, out long length) {
            if (quality < -1) quality = -1;
            if (quality > 100) quality = 100;
            int w = b.Width;
            int h = b.Height;
            var bd = b.LockBits(new Rectangle(0, 0, w, h), System.Drawing.Imaging.ImageLockMode.ReadOnly, b.PixelFormat);
            try {
                result = IntPtr.Zero;
                if (b.PixelFormat == System.Drawing.Imaging.PixelFormat.Format32bppArgb){
                    if (quality == -1) length = NativeMethods.WebPEncodeLosslessBGRA(bd.Scan0, w, h, bd.Stride, ref result);
                    else length = NativeMethods.WebPEncodeBGRA(bd.Scan0, w, h, bd.Stride, quality, ref result);
                }else if (b.PixelFormat == System.Drawing.Imaging.PixelFormat.Format32bppRgb){
                    if (quality == -1) length = NativeMethods.WebPEncodeLosslessBGRA(bd.Scan0, w, h, bd.Stride, ref result);
                    else length = NativeMethods.WebPEncodeBGRA(bd.Scan0, w, h, bd.Stride, quality, ref result);
                }else{
                    throw new NotSupportedException("Only Format32bppArgb and Format32bppRgb bitmaps are supported");
                }
                if (length == 0) throw new Exception("WebP encode failed!");

            } finally {
                b.UnlockBits(bd);
            }
        }
    }
}
