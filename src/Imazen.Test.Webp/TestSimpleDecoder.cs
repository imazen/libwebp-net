using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Xunit;
using Imazen.WebP;

namespace Imazen.Test.Webp
{
    public class TestSimpleDecoder
    {
        [Fact]
        public void TestDecoderVersionNotNull()
        {
            var version = SimpleDecoder.GetDecoderVersion();
            Assert.NotNull(version);
            Assert.NotEmpty(version);
            Assert.Contains(".", version);
        }

        [Fact]
        public void TestDecodeWebPFile()
        {
            var decoder = new SimpleDecoder();
            var fileName = "testimage.webp";
            Assert.True(File.Exists(fileName), $"Test image not found: {fileName}");

            using (var inputStream = File.Open(fileName, FileMode.Open))
            {
                var bytes = ReadFully(inputStream);
                using (var bitmap = decoder.DecodeFromBytes(bytes, bytes.LongLength))
                {
                    Assert.True(bitmap.Width > 0);
                    Assert.True(bitmap.Height > 0);
                    Assert.Equal(PixelFormat.Format32bppArgb, bitmap.PixelFormat);
                }
            }
        }

        [Fact]
        public void TestDecodeInvalidDataThrows()
        {
            var decoder = new SimpleDecoder();
            var badData = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            Assert.Throws<Exception>(() => decoder.DecodeFromBytes(badData, badData.LongLength));
        }

        [Fact]
        public void TestDecodeZeroLengthThrows()
        {
            var decoder = new SimpleDecoder();
            Assert.Throws<Exception>(() => decoder.DecodeFromBytes(new byte[0], 0));
        }

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}
