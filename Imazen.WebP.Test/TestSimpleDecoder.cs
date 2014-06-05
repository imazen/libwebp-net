using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Imazen.WebP;
using System.Drawing;
using System.IO;

namespace UnitTestProject1
{
    [TestClass]
    public class TestSimpleDecoder
    {
        [TestMethod]
        public void TestDecSimple()
        {

            var decoder = new SimpleDecoder();
            var fileName = "testimage.webp";
            var outFile = "testimageout.jpg";
            File.Delete(outFile);
            FileStream outStream = new FileStream(outFile, FileMode.Create);
            using (Stream inputStream = System.IO.File.Open(fileName, System.IO.FileMode.Open))
            {

                var bytes = ReadFully(inputStream);
                var outBitmap = decoder.DecodeFromBytes(bytes, bytes.LongLength);
                outBitmap.Save(outStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                outStream.Close();
            }

            FileInfo finfo = new FileInfo(outFile);
            Assert.IsTrue(finfo.Length > 5000);
            

        }

        public static byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }


}
