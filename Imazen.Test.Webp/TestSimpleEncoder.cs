using System;
using Xunit;
using Imazen.WebP;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using Imazen.WebP.Extern;

namespace Imazen.Test.WebP
{
    public class TestSimpleEncoder
    {
        [Fact]
        public void TestVersion(){
            Imazen.WebP.Extern.LoadLibrary.LoadWebPOrFail();
            Assert.Equal("0.4.1",SimpleEncoder.GetEncoderVersion());
        }
        [Fact]
        public void TestEncSimple()
        {
            Imazen.WebP.Extern.LoadLibrary.LoadWebPOrFail();

            var encoder = new SimpleEncoder();
            var fileName = "testimage.jpg";
            var outFileName = "testimageout.webp";
            File.Delete(outFileName);

            Bitmap mBitmap;
            FileStream outStream = new FileStream(outFileName, FileMode.Create);
            using (Stream BitmapStream = System.IO.File.Open(fileName, System.IO.FileMode.Open))
            {
                Image img = Image.FromStream(BitmapStream);

                mBitmap = new Bitmap(img);

                encoder.Encode(mBitmap, outStream, 100, false);
            }

            FileInfo finfo = new FileInfo(outFileName);
            Assert.True(finfo.Exists);
        }
    }
}
