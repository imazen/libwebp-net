using System;
using System.IO;
using Xunit;
using Imazen.WebP;

namespace Imazen.Test.Webp
{
    public class TestRawBufferApi
    {
        private byte[] CreateSolidColorPixels(int width, int height, WebPPixelFormat format,
            byte r, byte g, byte b, byte a = 255)
        {
            int bpp = (format == WebPPixelFormat.Bgr || format == WebPPixelFormat.Rgb) ? 3 : 4;
            byte[] pixels = new byte[width * height * bpp];
            for (int i = 0; i < width * height; i++)
            {
                int offset = i * bpp;
                switch (format)
                {
                    case WebPPixelFormat.Bgra:
                        pixels[offset] = b;
                        pixels[offset + 1] = g;
                        pixels[offset + 2] = r;
                        pixels[offset + 3] = a;
                        break;
                    case WebPPixelFormat.Rgba:
                        pixels[offset] = r;
                        pixels[offset + 1] = g;
                        pixels[offset + 2] = b;
                        pixels[offset + 3] = a;
                        break;
                    case WebPPixelFormat.Bgr:
                        pixels[offset] = b;
                        pixels[offset + 1] = g;
                        pixels[offset + 2] = r;
                        break;
                    case WebPPixelFormat.Rgb:
                        pixels[offset] = r;
                        pixels[offset + 1] = g;
                        pixels[offset + 2] = b;
                        break;
                }
            }
            return pixels;
        }

        [Theory]
        [InlineData(WebPPixelFormat.Bgra)]
        [InlineData(WebPPixelFormat.Rgba)]
        [InlineData(WebPPixelFormat.Bgr)]
        [InlineData(WebPPixelFormat.Rgb)]
        public void TestLosslessRoundtripAllFormats(WebPPixelFormat format)
        {
            int w = 16, h = 16;
            int bpp = (format == WebPPixelFormat.Bgr || format == WebPPixelFormat.Rgb) ? 3 : 4;
            int stride = w * bpp;

            byte[] original = CreateSolidColorPixels(w, h, format, 128, 64, 200);
            byte[] encoded = WebPEncoder.Encode(original, w, h, stride, format, -1); // lossless

            Assert.NotNull(encoded);
            Assert.True(encoded.Length > 0);

            // Decode back in same format
            int dw, dh;
            byte[] decoded = WebPDecoder.Decode(encoded, out dw, out dh, format);

            Assert.Equal(w, dw);
            Assert.Equal(h, dh);
            Assert.Equal(original.Length, decoded.Length);

            // Pixel-perfect for lossless
            for (int i = 0; i < original.Length; i++)
            {
                Assert.Equal(original[i], decoded[i]);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(50)]
        [InlineData(75)]
        [InlineData(100)]
        public void TestLossyEncodeAtVariousQualities(float quality)
        {
            int w = 32, h = 32;
            byte[] pixels = CreateSolidColorPixels(w, h, WebPPixelFormat.Bgra, 100, 150, 200, 255);
            byte[] encoded = WebPEncoder.Encode(pixels, w, h, w * 4, WebPPixelFormat.Bgra, quality);
            Assert.True(encoded.Length > 0);
        }

        [Fact]
        public void TestEncodeToStream()
        {
            int w = 16, h = 16;
            byte[] pixels = CreateSolidColorPixels(w, h, WebPPixelFormat.Rgba, 255, 0, 0, 255);
            using (var ms = new MemoryStream())
            {
                WebPEncoder.Encode(pixels, w, h, w * 4, WebPPixelFormat.Rgba, 75, ms);
                Assert.True(ms.Length > 0);
            }
        }

        [Fact]
        public void TestDecodeIntoProvidedBuffer()
        {
            int w = 8, h = 8;
            byte[] pixels = CreateSolidColorPixels(w, h, WebPPixelFormat.Bgra, 50, 100, 150, 255);
            byte[] encoded = WebPEncoder.Encode(pixels, w, h, w * 4, WebPPixelFormat.Bgra, -1);

            byte[] output = new byte[w * h * 4];
            WebPDecoder.Decode(encoded, output, w * 4, WebPPixelFormat.Bgra);

            for (int i = 0; i < pixels.Length; i++)
                Assert.Equal(pixels[i], output[i]);
        }

        [Fact]
        public void TestEncodeNullPixelsThrows()
        {
            Assert.Throws<ArgumentNullException>(() =>
                WebPEncoder.Encode(null!, 10, 10, 40, WebPPixelFormat.Bgra, 75));
        }

        [Fact]
        public void TestDecodeInvalidDataThrows()
        {
            var badData = new byte[] { 0, 1, 2, 3, 4, 5 };
            int w, h;
            Assert.Throws<Exception>(() => WebPDecoder.Decode(badData, out w, out h));
        }

        [Fact]
        public void TestDecodeWebPFileRawBuffer()
        {
            var fileName = "testimage.webp";
            if (!File.Exists(fileName)) return; // skip if no test file

            byte[] data = File.ReadAllBytes(fileName);
            int w, h;
            byte[] pixels = WebPDecoder.Decode(data, out w, out h, WebPPixelFormat.Bgra);

            Assert.True(w > 0);
            Assert.True(h > 0);
            Assert.Equal(w * h * 4, pixels.Length);
        }
    }
}
