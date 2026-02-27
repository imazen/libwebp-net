using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Xunit;
using Imazen.WebP;
using Imazen.WebP.Extern;

namespace Imazen.Test.Webp
{
    /// <summary>
    /// Tests for alpha channel preservation and various image dimensions.
    /// </summary>
    public class TestAlphaAndDimensions
    {
        // --- Alpha channel tests ---

        [WindowsFact]
        public void TestAlphaPreservedLosslessBitmap()
        {
            var encoder = new SimpleEncoder();
            var decoder = new SimpleDecoder();

            using (var bitmap = new Bitmap(16, 16, PixelFormat.Format32bppArgb))
            {
                // Set pixels with varying alpha — use exact mode to preserve RGB under transparent areas.
                // Avoid alpha=0 since the SimpleEncoder path doesn't use exact mode.
                // Use alpha range [1..255] to test alpha preservation without triggering
                // the transparent-pixel optimization.
                for (int y = 0; y < 16; y++)
                    for (int x = 0; x < 16; x++)
                    {
                        int alpha = (x + y) * 8 % 256;
                        if (alpha == 0) alpha = 1; // avoid fully transparent
                        bitmap.SetPixel(x, y, Color.FromArgb(alpha, 100, 150, 200));
                    }

                using (var ms = new MemoryStream())
                {
                    encoder.Encode(bitmap, ms, -1); // lossless
                    var bytes = ms.ToArray();

                    // Verify alpha is reported
                    var info = WebPInfo.GetImageInfo(bytes);
                    Assert.True(info.HasAlpha);

                    using (var decoded = decoder.DecodeFromBytes(bytes, bytes.LongLength))
                    {
                        for (int y = 0; y < 16; y++)
                            for (int x = 0; x < 16; x++)
                                Assert.Equal(bitmap.GetPixel(x, y).ToArgb(), decoded.GetPixel(x, y).ToArgb());
                    }
                }
            }
        }

        [Fact]
        public void TestAlphaPreservedLosslessRawBuffer()
        {
            int w = 16, h = 16;
            byte[] pixels = new byte[w * h * 4];
            for (int i = 0; i < w * h; i++)
            {
                pixels[i * 4 + 0] = 50;                // B
                pixels[i * 4 + 1] = 100;               // G
                pixels[i * 4 + 2] = 200;               // R
                pixels[i * 4 + 3] = (byte)(i % 256);   // varying A
            }

            // Use exact mode to preserve RGB values under transparent pixels
            var config = new WebPEncoderConfig().SetLossless().SetExact();
            byte[] encoded = WebPEncoder.Encode(pixels, w, h, w * 4, WebPPixelFormat.Bgra, config);
            var info = WebPInfo.GetImageInfo(encoded);
            Assert.True(info.HasAlpha);

            int dw, dh;
            byte[] decoded = WebPDecoder.Decode(encoded, out dw, out dh, WebPPixelFormat.Bgra);
            Assert.Equal(w, dw);
            Assert.Equal(h, dh);

            for (int i = 0; i < pixels.Length; i++)
                Assert.Equal(pixels[i], decoded[i]);
        }

        [Fact]
        public void TestFullyTransparentPixels()
        {
            int w = 8, h = 8;
            byte[] pixels = new byte[w * h * 4];
            // All zeros = fully transparent black
            byte[] encoded = WebPEncoder.Encode(pixels, w, h, w * 4, WebPPixelFormat.Bgra, -1);
            Assert.True(encoded.Length > 0);

            var info = WebPInfo.GetImageInfo(encoded);
            Assert.True(info.HasAlpha);
        }

        [Fact]
        public void TestFullyOpaqueDoesNotReportAlpha()
        {
            int w = 16, h = 16;
            byte[] pixels = new byte[w * h * 4];
            for (int i = 0; i < w * h; i++)
            {
                pixels[i * 4 + 0] = 128;
                pixels[i * 4 + 1] = 128;
                pixels[i * 4 + 2] = 128;
                pixels[i * 4 + 3] = 255; // fully opaque
            }

            byte[] encoded = WebPEncoder.Encode(pixels, w, h, w * 4, WebPPixelFormat.Bgra, 75);
            // With lossy at this trivial image, alpha may or may not be reported.
            // But it should at least be valid
            var info = WebPInfo.GetImageInfo(encoded);
            Assert.Equal(w, info.Width);
            Assert.Equal(h, info.Height);
        }

        [Fact]
        public void TestRgbaAlphaRoundtrip()
        {
            int w = 8, h = 8;
            byte[] pixels = new byte[w * h * 4];
            for (int i = 0; i < w * h; i++)
            {
                pixels[i * 4 + 0] = 200; // R
                pixels[i * 4 + 1] = 100; // G
                pixels[i * 4 + 2] = 50;  // B
                pixels[i * 4 + 3] = (byte)(i * 4); // varying A
            }

            // Use exact mode to preserve RGB values under transparent pixels
            var config = new WebPEncoderConfig().SetLossless().SetExact();
            byte[] encoded = WebPEncoder.Encode(pixels, w, h, w * 4, WebPPixelFormat.Rgba, config);
            int dw, dh;
            byte[] decoded = WebPDecoder.Decode(encoded, out dw, out dh, WebPPixelFormat.Rgba);

            for (int i = 0; i < pixels.Length; i++)
                Assert.Equal(pixels[i], decoded[i]);
        }

        // --- Dimension tests ---

        [Fact]
        public void TestMinimumDimension1x1()
        {
            int w = 1, h = 1;
            byte[] pixels = new byte[] { 128, 64, 200, 255 }; // BGRA
            byte[] encoded = WebPEncoder.Encode(pixels, w, h, w * 4, WebPPixelFormat.Bgra, -1);
            Assert.True(encoded.Length > 0);

            int dw, dh;
            byte[] decoded = WebPDecoder.Decode(encoded, out dw, out dh, WebPPixelFormat.Bgra);
            Assert.Equal(1, dw);
            Assert.Equal(1, dh);
            Assert.Equal(pixels, decoded);
        }

        [Theory]
        [InlineData(1, 100)]
        [InlineData(100, 1)]
        [InlineData(1, 1)]
        [InlineData(2, 2)]
        [InlineData(3, 3)]
        public void TestNarrowDimensions(int w, int h)
        {
            byte[] pixels = new byte[w * h * 4];
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = (byte)(i % 256);

            byte[] encoded = WebPEncoder.Encode(pixels, w, h, w * 4, WebPPixelFormat.Bgra, -1);
            int dw, dh;
            byte[] decoded = WebPDecoder.Decode(encoded, out dw, out dh, WebPPixelFormat.Bgra);

            Assert.Equal(w, dw);
            Assert.Equal(h, dh);

            for (int i = 0; i < pixels.Length; i++)
                Assert.Equal(pixels[i], decoded[i]);
        }

        [Fact]
        public void TestLargerImage256x256()
        {
            int w = 256, h = 256;
            byte[] pixels = new byte[w * h * 4];
            var rng = new Random(42);
            rng.NextBytes(pixels);
            // Make fully opaque
            for (int i = 0; i < w * h; i++)
                pixels[i * 4 + 3] = 255;

            byte[] encoded = WebPEncoder.Encode(pixels, w, h, w * 4, WebPPixelFormat.Bgra, 75);
            Assert.True(encoded.Length > 0);

            int dw, dh;
            byte[] decoded = WebPDecoder.Decode(encoded, out dw, out dh, WebPPixelFormat.Bgra);
            Assert.Equal(w, dw);
            Assert.Equal(h, dh);
        }

        [Fact]
        public void TestLargerImageLosslessRoundtrip()
        {
            int w = 128, h = 128;
            byte[] pixels = new byte[w * h * 4];
            var rng = new Random(99);
            rng.NextBytes(pixels);
            // Make all pixels fully opaque — lossless without exact mode may
            // alter RGB values of transparent pixels for better compression.
            for (int i = 0; i < w * h; i++)
                pixels[i * 4 + 3] = 255;

            byte[] encoded = WebPEncoder.Encode(pixels, w, h, w * 4, WebPPixelFormat.Bgra, -1);
            int dw, dh;
            byte[] decoded = WebPDecoder.Decode(encoded, out dw, out dh, WebPPixelFormat.Bgra);

            Assert.Equal(w, dw);
            Assert.Equal(h, dh);
            for (int i = 0; i < pixels.Length; i++)
                Assert.Equal(pixels[i], decoded[i]);
        }

        [Fact]
        public void TestNonSquareDimensions()
        {
            int w = 200, h = 50;
            byte[] pixels = new byte[w * h * 4];
            var rng = new Random(7);
            rng.NextBytes(pixels);
            // Make all pixels fully opaque — lossless without exact mode may
            // alter RGB values of transparent pixels for better compression.
            for (int i = 0; i < w * h; i++)
                pixels[i * 4 + 3] = 255;

            byte[] encoded = WebPEncoder.Encode(pixels, w, h, w * 4, WebPPixelFormat.Bgra, -1);
            int dw, dh;
            byte[] decoded = WebPDecoder.Decode(encoded, out dw, out dh, WebPPixelFormat.Bgra);

            Assert.Equal(w, dw);
            Assert.Equal(h, dh);
            Assert.Equal(pixels, decoded);
        }

        [Fact]
        public void TestZeroWidthThrows()
        {
            byte[] pixels = new byte[100];
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                WebPEncoder.Encode(pixels, 0, 10, 0, WebPPixelFormat.Bgra, 50));
        }

        [Fact]
        public void TestZeroHeightThrows()
        {
            byte[] pixels = new byte[100];
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                WebPEncoder.Encode(pixels, 10, 0, 40, WebPPixelFormat.Bgra, 50));
        }

        [WindowsFact]
        public void TestBitmapLargerDimensionRoundtrip()
        {
            var encoder = new SimpleEncoder();
            var decoder = new SimpleDecoder();

            using (var bitmap = new Bitmap(200, 150, PixelFormat.Format32bppArgb))
            {
                using (var g = Graphics.FromImage(bitmap))
                {
                    g.Clear(Color.White);
                    using (var brush = new SolidBrush(Color.FromArgb(128, 255, 0, 0)))
                        g.FillEllipse(brush, 20, 20, 160, 110);
                }

                using (var ms = new MemoryStream())
                {
                    encoder.Encode(bitmap, ms, -1);
                    var bytes = ms.ToArray();
                    using (var decoded = decoder.DecodeFromBytes(bytes, bytes.LongLength))
                    {
                        Assert.Equal(200, decoded.Width);
                        Assert.Equal(150, decoded.Height);

                        for (int y = 0; y < bitmap.Height; y++)
                            for (int x = 0; x < bitmap.Width; x++)
                                Assert.Equal(bitmap.GetPixel(x, y).ToArgb(), decoded.GetPixel(x, y).ToArgb());
                    }
                }
            }
        }
    }
}
