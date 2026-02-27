using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Xunit;
using Imazen.WebP;

namespace Imazen.Test.Webp
{
    /// <summary>
    /// Tests for stream-based decoding APIs (ported from resizer4 WebPDecoder plugin pattern).
    /// </summary>
    public class TestStreamApi
    {
        [WindowsFact]
        public void TestSimpleDecoderDecodeFromStream()
        {
            var encoder = new SimpleEncoder();
            var decoder = new SimpleDecoder();

            using (var bitmap = new Bitmap(32, 32, PixelFormat.Format32bppArgb))
            {
                using (var g = Graphics.FromImage(bitmap))
                    g.Clear(Color.DarkCyan);

                using (var webpStream = new MemoryStream())
                {
                    encoder.Encode(bitmap, webpStream, 75);
                    webpStream.Position = 0;

                    using (var decoded = decoder.DecodeFromStream(webpStream))
                    {
                        Assert.Equal(32, decoded.Width);
                        Assert.Equal(32, decoded.Height);
                        Assert.Equal(PixelFormat.Format32bppArgb, decoded.PixelFormat);
                    }
                }
            }
        }

        [WindowsFact]
        public void TestSimpleDecoderDecodeFromStreamLossless()
        {
            var encoder = new SimpleEncoder();
            var decoder = new SimpleDecoder();

            using (var bitmap = new Bitmap(16, 16, PixelFormat.Format32bppArgb))
            {
                using (var g = Graphics.FromImage(bitmap))
                    g.Clear(Color.FromArgb(200, 100, 50, 25));

                using (var webpStream = new MemoryStream())
                {
                    encoder.Encode(bitmap, webpStream, -1); // lossless
                    webpStream.Position = 0;

                    using (var decoded = decoder.DecodeFromStream(webpStream))
                    {
                        Assert.Equal(16, decoded.Width);
                        Assert.Equal(16, decoded.Height);

                        // Pixel-perfect for lossless
                        for (int y = 0; y < bitmap.Height; y++)
                            for (int x = 0; x < bitmap.Width; x++)
                                Assert.Equal(bitmap.GetPixel(x, y).ToArgb(), decoded.GetPixel(x, y).ToArgb());
                    }
                }
            }
        }

        [Fact]
        public void TestSimpleDecoderDecodeFromStreamNullThrows()
        {
            var decoder = new SimpleDecoder();
            Assert.Throws<ArgumentNullException>(() => decoder.DecodeFromStream(null!));
        }

        [Fact]
        public void TestRawDecoderDecodeFromStream()
        {
            int w = 24, h = 24;
            byte[] pixels = new byte[w * h * 4];
            for (int i = 0; i < w * h; i++)
            {
                pixels[i * 4 + 0] = 100; // B
                pixels[i * 4 + 1] = 150; // G
                pixels[i * 4 + 2] = 200; // R
                pixels[i * 4 + 3] = 255; // A
            }

            byte[] encoded = WebPEncoder.Encode(pixels, w, h, w * 4, WebPPixelFormat.Bgra, -1);

            using (var stream = new MemoryStream(encoded))
            {
                int dw, dh;
                byte[] decoded = WebPDecoder.DecodeFromStream(stream, out dw, out dh);
                Assert.Equal(w, dw);
                Assert.Equal(h, dh);

                for (int i = 0; i < pixels.Length; i++)
                    Assert.Equal(pixels[i], decoded[i]);
            }
        }

        [Fact]
        public void TestRawDecoderDecodeFromStreamWithFormat()
        {
            int w = 16, h = 16;
            byte[] pixels = new byte[w * h * 3];
            for (int i = 0; i < w * h; i++)
            {
                pixels[i * 3 + 0] = 255; // R
                pixels[i * 3 + 1] = 128; // G
                pixels[i * 3 + 2] = 0;   // B
            }

            byte[] encoded = WebPEncoder.Encode(pixels, w, h, w * 3, WebPPixelFormat.Rgb, -1);

            using (var stream = new MemoryStream(encoded))
            {
                int dw, dh;
                byte[] decoded = WebPDecoder.DecodeFromStream(stream, out dw, out dh, WebPPixelFormat.Rgb);
                Assert.Equal(w, dw);
                Assert.Equal(h, dh);

                for (int i = 0; i < pixels.Length; i++)
                    Assert.Equal(pixels[i], decoded[i]);
            }
        }

        [Fact]
        public void TestRawDecoderDecodeFromStreamNullThrows()
        {
            int w, h;
            Assert.Throws<ArgumentNullException>(() => WebPDecoder.DecodeFromStream(null!, out w, out h));
        }

        [Fact]
        public void TestIsWebPFromBytes()
        {
            int w = 8, h = 8;
            byte[] pixels = new byte[w * h * 4];
            byte[] encoded = WebPEncoder.Encode(pixels, w, h, w * 4, WebPPixelFormat.Bgra, 50);
            Assert.True(WebPDecoder.IsWebP(encoded));
        }

        [Fact]
        public void TestIsWebPFromBytesInvalid()
        {
            Assert.False(WebPDecoder.IsWebP(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }));
            Assert.False(WebPDecoder.IsWebP(new byte[] { 0, 1, 2 }));
            Assert.False(WebPDecoder.IsWebP((byte[]?)null));
        }

        [Fact]
        public void TestIsWebPFromStream()
        {
            int w = 8, h = 8;
            byte[] pixels = new byte[w * h * 4];
            byte[] encoded = WebPEncoder.Encode(pixels, w, h, w * 4, WebPPixelFormat.Bgra, 50);

            using (var stream = new MemoryStream(encoded))
            {
                Assert.True(WebPDecoder.IsWebP(stream));
                // Position should be restored
                Assert.Equal(0, stream.Position);
            }
        }

        [Fact]
        public void TestIsWebPFromStreamNonWebP()
        {
            // JPEG header
            var jpegHeader = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0, 0, 0, 0, 0, 0, 0, 0 };
            using (var stream = new MemoryStream(jpegHeader))
            {
                Assert.False(WebPDecoder.IsWebP(stream));
                Assert.Equal(0, stream.Position);
            }
        }

        /// <summary>
        /// Full stream roundtrip: Bitmap → encode to stream → decode from stream.
        /// Mirrors resizer4's encode-then-decode pattern.
        /// </summary>
        [WindowsFact]
        public void TestFullStreamRoundtrip()
        {
            var encoder = new SimpleEncoder();
            var decoder = new SimpleDecoder();

            using (var original = new Bitmap(64, 48, PixelFormat.Format32bppArgb))
            {
                // Paint a pattern
                for (int y = 0; y < 48; y++)
                    for (int x = 0; x < 64; x++)
                        original.SetPixel(x, y, Color.FromArgb(255, x * 4 % 256, y * 5 % 256, (x + y) * 3 % 256));

                using (var webpStream = new MemoryStream())
                {
                    // Encode to stream
                    encoder.Encode(original, webpStream, -1); // lossless
                    Assert.True(webpStream.Length > 0);

                    // Reset and decode from stream
                    webpStream.Position = 0;
                    using (var decoded = decoder.DecodeFromStream(webpStream))
                    {
                        Assert.Equal(original.Width, decoded.Width);
                        Assert.Equal(original.Height, decoded.Height);

                        for (int y = 0; y < original.Height; y++)
                            for (int x = 0; x < original.Width; x++)
                                Assert.Equal(original.GetPixel(x, y).ToArgb(), decoded.GetPixel(x, y).ToArgb());
                    }
                }
            }
        }
    }
}
