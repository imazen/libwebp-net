using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Xunit;
using Imazen.WebP;
using Imazen.Test.Webp;

namespace Imazen.Test.WebP
{
    public class TestSimpleEncoder
    {
        [Fact]
        public void TestEncoderVersionNotNull()
        {
            var version = SimpleEncoder.GetEncoderVersion();
            Assert.NotNull(version);
            Assert.NotEmpty(version);
            Assert.Contains(".", version);
        }

        [WindowsFact]
        public void TestEncodeJpegToWebP()
        {
            var encoder = new SimpleEncoder();
            var fileName = "testimage.jpg";
            Assert.True(File.Exists(fileName), $"Test image not found: {fileName}");

            using (var bitmapStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var img = Image.FromStream(bitmapStream))
            using (var bitmap = new Bitmap(img))
            using (var outStream = new MemoryStream())
            {
                encoder.Encode(bitmap, outStream, 80);
                Assert.True(outStream.Length > 0, "Encoded WebP should have non-zero length");

                // Verify it starts with RIFF header
                var bytes = outStream.ToArray();
                Assert.True(bytes.Length >= 12);
                Assert.Equal((byte)'R', bytes[0]);
                Assert.Equal((byte)'I', bytes[1]);
                Assert.Equal((byte)'F', bytes[2]);
                Assert.Equal((byte)'F', bytes[3]);
            }
        }

        [WindowsTheory]
        [InlineData(0)]
        [InlineData(50)]
        [InlineData(75)]
        [InlineData(100)]
        public void TestEncodeAtVariousQualities(float quality)
        {
            var encoder = new SimpleEncoder();
            using (var bitmap = new Bitmap(64, 64, PixelFormat.Format32bppArgb))
            using (var outStream = new MemoryStream())
            {
                using (var g = Graphics.FromImage(bitmap))
                    g.Clear(Color.CornflowerBlue);

                encoder.Encode(bitmap, outStream, quality);
                Assert.True(outStream.Length > 0);
            }
        }

        [WindowsFact]
        public void TestLosslessEncode()
        {
            var encoder = new SimpleEncoder();
            using (var bitmap = new Bitmap(32, 32, PixelFormat.Format32bppArgb))
            using (var outStream = new MemoryStream())
            {
                using (var g = Graphics.FromImage(bitmap))
                    g.Clear(Color.Red);

                encoder.Encode(bitmap, outStream, -1); // lossless
                Assert.True(outStream.Length > 0);
            }
        }

        [WindowsFact]
        public void TestEncode24bppRgb()
        {
            var encoder = new SimpleEncoder();
            using (var bitmap = new Bitmap(32, 32, PixelFormat.Format24bppRgb))
            using (var outStream = new MemoryStream())
            {
                using (var g = Graphics.FromImage(bitmap))
                    g.Clear(Color.Green);

                encoder.Encode(bitmap, outStream, 75);
                Assert.True(outStream.Length > 0);
            }
        }

        [WindowsFact]
        public void TestEncodeNonStandardPixelFormat()
        {
            var encoder = new SimpleEncoder();
            // Format32bppRgb will be auto-converted to 32bppArgb
            using (var bitmap = new Bitmap(16, 16, PixelFormat.Format32bppRgb))
            using (var outStream = new MemoryStream())
            {
                using (var g = Graphics.FromImage(bitmap))
                    g.Clear(Color.Blue);

                encoder.Encode(bitmap, outStream, 75);
                Assert.True(outStream.Length > 0);
            }
        }
    }
}
