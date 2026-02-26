using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Xunit;
using Imazen.WebP;

namespace Imazen.Test.Webp
{
    /// <summary>
    /// Tests ported from resizer4's WebPTests.fs — gradient image encode/decode patterns.
    /// </summary>
    public class TestGradientImageRoundtrip
    {
        /// <summary>
        /// Creates a gradient bitmap similar to resizer4's Gradient plugin.
        /// Generates a smooth color gradient useful for encode quality testing.
        /// </summary>
        private static Bitmap CreateGradientBitmap(int width, int height, PixelFormat format = PixelFormat.Format32bppArgb)
        {
            var bitmap = new Bitmap(width, height, format);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float fx = (float)x / Math.Max(1, width - 1);
                    float fy = (float)y / Math.Max(1, height - 1);
                    int r = (int)(fx * 255);
                    int g = (int)(fy * 255);
                    int b = (int)((1f - fx) * 255);
                    int a = format == PixelFormat.Format32bppArgb ? (int)((1f - fy * 0.5f) * 255) : 255;
                    bitmap.SetPixel(x, y, Color.FromArgb(a, r, g, b));
                }
            }
            return bitmap;
        }

        /// <summary>
        /// Ported from resizer4: "We can encode a webp image"
        /// Encodes a 100x100 gradient to WebP.
        /// </summary>
        [Fact]
        public void TestEncodeGradientToWebP()
        {
            using (var bitmap = CreateGradientBitmap(100, 100))
            using (var ms = new MemoryStream())
            {
                new SimpleEncoder().Encode(bitmap, ms, 90);
                Assert.True(ms.Length > 0);

                // Verify RIFF/WEBP header
                var bytes = ms.ToArray();
                Assert.True(bytes.Length >= 12);
                Assert.Equal((byte)'R', bytes[0]);
                Assert.Equal((byte)'I', bytes[1]);
                Assert.Equal((byte)'F', bytes[2]);
                Assert.Equal((byte)'F', bytes[3]);
                Assert.Equal((byte)'W', bytes[8]);
                Assert.Equal((byte)'E', bytes[9]);
                Assert.Equal((byte)'B', bytes[10]);
                Assert.Equal((byte)'P', bytes[11]);
            }
        }

        /// <summary>
        /// Ported from resizer4: "We can encode and decode a webp image"
        /// Full roundtrip: gradient → WebP → decode → verify dimensions.
        /// </summary>
        [Fact]
        public void TestEncodeDecodeGradientRoundtrip()
        {
            var encoder = new SimpleEncoder();
            var decoder = new SimpleDecoder();

            using (var bitmap = CreateGradientBitmap(100, 100))
            using (var webpStream = new MemoryStream())
            {
                // Encode gradient to WebP
                encoder.Encode(bitmap, webpStream, 90);
                webpStream.Position = 0;

                // Decode WebP back to bitmap
                var webpBytes = webpStream.ToArray();
                using (var decoded = decoder.DecodeFromBytes(webpBytes, webpBytes.LongLength))
                {
                    Assert.Equal(100, decoded.Width);
                    Assert.Equal(100, decoded.Height);
                    Assert.Equal(PixelFormat.Format32bppArgb, decoded.PixelFormat);
                }
            }
        }

        /// <summary>
        /// Roundtrip: gradient → WebP (lossless) → decode → pixel-perfect.
        /// </summary>
        [Fact]
        public void TestLosslessGradientRoundtrip()
        {
            var encoder = new SimpleEncoder();
            var decoder = new SimpleDecoder();

            using (var bitmap = CreateGradientBitmap(50, 50))
            using (var webpStream = new MemoryStream())
            {
                encoder.Encode(bitmap, webpStream, -1); // lossless
                var webpBytes = webpStream.ToArray();
                using (var decoded = decoder.DecodeFromBytes(webpBytes, webpBytes.LongLength))
                {
                    Assert.Equal(bitmap.Width, decoded.Width);
                    Assert.Equal(bitmap.Height, decoded.Height);

                    // Pixel-perfect for lossless
                    for (int y = 0; y < bitmap.Height; y++)
                        for (int x = 0; x < bitmap.Width; x++)
                            Assert.Equal(bitmap.GetPixel(x, y).ToArgb(), decoded.GetPixel(x, y).ToArgb());
                }
            }
        }

        /// <summary>
        /// Test encoding a gradient at multiple quality levels.
        /// </summary>
        [Theory]
        [InlineData(0)]
        [InlineData(25)]
        [InlineData(50)]
        [InlineData(75)]
        [InlineData(90)]
        [InlineData(100)]
        [InlineData(-1)] // lossless
        public void TestEncodeGradientAtVariousQualities(float quality)
        {
            using (var bitmap = CreateGradientBitmap(64, 64))
            using (var ms = new MemoryStream())
            {
                new SimpleEncoder().Encode(bitmap, ms, quality);
                Assert.True(ms.Length > 0);
            }
        }

        /// <summary>
        /// Test gradient with 24bpp RGB format.
        /// </summary>
        [Fact]
        public void TestEncodeDecodeGradient24bpp()
        {
            var encoder = new SimpleEncoder();
            var decoder = new SimpleDecoder();

            using (var bitmap = CreateGradientBitmap(80, 60, PixelFormat.Format24bppRgb))
            using (var ms = new MemoryStream())
            {
                encoder.Encode(bitmap, ms, 85);
                var bytes = ms.ToArray();
                using (var decoded = decoder.DecodeFromBytes(bytes, bytes.LongLength))
                {
                    Assert.Equal(80, decoded.Width);
                    Assert.Equal(60, decoded.Height);
                }
            }
        }

        /// <summary>
        /// Tests that higher quality produces larger output for lossy encoding.
        /// </summary>
        [Fact]
        public void TestHigherQualityProducesLargerOutput()
        {
            long lowSize, highSize;

            using (var bitmap = CreateGradientBitmap(128, 128))
            {
                using (var ms = new MemoryStream())
                {
                    new SimpleEncoder().Encode(bitmap, ms, 10);
                    lowSize = ms.Length;
                }
                using (var ms = new MemoryStream())
                {
                    new SimpleEncoder().Encode(bitmap, ms, 100);
                    highSize = ms.Length;
                }
            }

            Assert.True(highSize > lowSize,
                $"Expected quality 100 ({highSize} bytes) > quality 10 ({lowSize} bytes)");
        }

        /// <summary>
        /// Test raw buffer gradient roundtrip (cross-platform).
        /// </summary>
        [Theory]
        [InlineData(WebPPixelFormat.Bgra)]
        [InlineData(WebPPixelFormat.Rgba)]
        [InlineData(WebPPixelFormat.Bgr)]
        [InlineData(WebPPixelFormat.Rgb)]
        public void TestRawGradientLosslessRoundtrip(WebPPixelFormat format)
        {
            int w = 32, h = 32;
            int bpp = (format == WebPPixelFormat.Bgr || format == WebPPixelFormat.Rgb) ? 3 : 4;
            int stride = w * bpp;
            byte[] pixels = CreateGradientPixels(w, h, format);

            byte[] encoded = WebPEncoder.Encode(pixels, w, h, stride, format, -1); // lossless
            Assert.True(encoded.Length > 0);

            int dw, dh;
            byte[] decoded = WebPDecoder.Decode(encoded, out dw, out dh, format);

            Assert.Equal(w, dw);
            Assert.Equal(h, dh);
            Assert.Equal(pixels.Length, decoded.Length);

            for (int i = 0; i < pixels.Length; i++)
                Assert.Equal(pixels[i], decoded[i]);
        }

        private static byte[] CreateGradientPixels(int width, int height, WebPPixelFormat format)
        {
            int bpp = (format == WebPPixelFormat.Bgr || format == WebPPixelFormat.Rgb) ? 3 : 4;
            byte[] pixels = new byte[width * height * bpp];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float fx = (float)x / Math.Max(1, width - 1);
                    float fy = (float)y / Math.Max(1, height - 1);
                    byte r = (byte)(fx * 255);
                    byte g = (byte)(fy * 255);
                    byte b = (byte)((1f - fx) * 255);
                    byte a = 255;
                    int offset = (y * width + x) * bpp;

                    switch (format)
                    {
                        case WebPPixelFormat.Bgra:
                            pixels[offset] = b; pixels[offset + 1] = g;
                            pixels[offset + 2] = r; pixels[offset + 3] = a;
                            break;
                        case WebPPixelFormat.Rgba:
                            pixels[offset] = r; pixels[offset + 1] = g;
                            pixels[offset + 2] = b; pixels[offset + 3] = a;
                            break;
                        case WebPPixelFormat.Bgr:
                            pixels[offset] = b; pixels[offset + 1] = g;
                            pixels[offset + 2] = r;
                            break;
                        case WebPPixelFormat.Rgb:
                            pixels[offset] = r; pixels[offset + 1] = g;
                            pixels[offset + 2] = b;
                            break;
                    }
                }
            }
            return pixels;
        }
    }
}
