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
    /// Tests for advanced encoding using WebPEncoderConfig with the full WebPPicture pipeline.
    /// </summary>
    public class TestAdvancedEncoding
    {
        private static byte[] CreateSolidPixels(int w, int h, WebPPixelFormat format,
            byte r = 128, byte g = 64, byte b = 200, byte a = 255)
        {
            int bpp = (format == WebPPixelFormat.Bgr || format == WebPPixelFormat.Rgb) ? 3 : 4;
            byte[] pixels = new byte[w * h * bpp];
            for (int i = 0; i < w * h; i++)
            {
                int offset = i * bpp;
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
            return pixels;
        }

        [Fact]
        public void TestEncodeWithDefaultConfig()
        {
            var config = new WebPEncoderConfig();
            int w = 32, h = 32;
            byte[] pixels = CreateSolidPixels(w, h, WebPPixelFormat.Bgra);

            byte[] encoded = WebPEncoder.Encode(pixels, w, h, w * 4, WebPPixelFormat.Bgra, config);
            Assert.True(encoded.Length > 0);
            Assert.True(WebPDecoder.IsWebP(encoded));
        }

        [Theory]
        [InlineData(WebPPreset.WEBP_PRESET_DEFAULT)]
        [InlineData(WebPPreset.WEBP_PRESET_PICTURE)]
        [InlineData(WebPPreset.WEBP_PRESET_PHOTO)]
        [InlineData(WebPPreset.WEBP_PRESET_DRAWING)]
        [InlineData(WebPPreset.WEBP_PRESET_ICON)]
        [InlineData(WebPPreset.WEBP_PRESET_TEXT)]
        public void TestEncodeWithAllPresets(WebPPreset preset)
        {
            var config = new WebPEncoderConfig(preset, 75);
            int w = 32, h = 32;
            byte[] pixels = CreateSolidPixels(w, h, WebPPixelFormat.Bgra);

            byte[] encoded = WebPEncoder.Encode(pixels, w, h, w * 4, WebPPixelFormat.Bgra, config);
            Assert.True(encoded.Length > 0);

            // Verify we can decode it back
            int dw, dh;
            byte[] decoded = WebPDecoder.Decode(encoded, out dw, out dh);
            Assert.Equal(w, dw);
            Assert.Equal(h, dh);
        }

        [Theory]
        [InlineData(WebPPixelFormat.Bgra)]
        [InlineData(WebPPixelFormat.Rgba)]
        [InlineData(WebPPixelFormat.Bgr)]
        [InlineData(WebPPixelFormat.Rgb)]
        public void TestEncodeWithConfigAllFormats(WebPPixelFormat format)
        {
            var config = new WebPEncoderConfig().SetQuality(80);
            int w = 16, h = 16;
            int bpp = (format == WebPPixelFormat.Bgr || format == WebPPixelFormat.Rgb) ? 3 : 4;
            byte[] pixels = CreateSolidPixels(w, h, format);

            byte[] encoded = WebPEncoder.Encode(pixels, w, h, w * bpp, format, config);
            Assert.True(encoded.Length > 0);

            int dw, dh;
            WebPDecoder.Decode(encoded, out dw, out dh);
            Assert.Equal(w, dw);
            Assert.Equal(h, dh);
        }

        [Fact]
        public void TestEncodeWithLosslessConfig()
        {
            var config = new WebPEncoderConfig().SetLossless().SetMethod(6);
            int w = 16, h = 16;
            byte[] pixels = CreateSolidPixels(w, h, WebPPixelFormat.Bgra);

            byte[] encoded = WebPEncoder.Encode(pixels, w, h, w * 4, WebPPixelFormat.Bgra, config);
            Assert.True(encoded.Length > 0);

            // Verify lossless via WebPInfo
            var info = WebPInfo.GetImageInfo(encoded);
            Assert.Equal(w, info.Width);
            Assert.Equal(h, info.Height);
            Assert.Equal(2, info.Format); // 2 = lossless
        }

        [Fact]
        public void TestEncodeWithLosslessPresetConfig()
        {
            var config = new WebPEncoderConfig().SetLosslessPreset(6);
            int w = 16, h = 16;
            byte[] pixels = CreateSolidPixels(w, h, WebPPixelFormat.Bgra);

            byte[] encoded = WebPEncoder.Encode(pixels, w, h, w * 4, WebPPixelFormat.Bgra, config);
            Assert.True(encoded.Length > 0);

            var info = WebPInfo.GetImageInfo(encoded);
            Assert.Equal(2, info.Format); // lossless
        }

        [Fact]
        public void TestEncodeWithSharpYuv()
        {
            var config = new WebPEncoderConfig()
                .SetQuality(75)
                .SetSharpYuv();

            int w = 32, h = 32;
            byte[] pixels = CreateSolidPixels(w, h, WebPPixelFormat.Bgra);

            byte[] encoded = WebPEncoder.Encode(pixels, w, h, w * 4, WebPPixelFormat.Bgra, config);
            Assert.True(encoded.Length > 0);
        }

        [Fact]
        public void TestEncodeWithMultiThreaded()
        {
            var config = new WebPEncoderConfig()
                .SetQuality(85)
                .SetMultiThreaded()
                .SetMethod(4);

            int w = 64, h = 64;
            byte[] pixels = CreateSolidPixels(w, h, WebPPixelFormat.Bgra);

            byte[] encoded = WebPEncoder.Encode(pixels, w, h, w * 4, WebPPixelFormat.Bgra, config);
            Assert.True(encoded.Length > 0);
        }

        [Fact]
        public void TestEncodeWithNearLossless()
        {
            var config = new WebPEncoderConfig()
                .SetLossless()
                .SetNearLossless(60);

            int w = 32, h = 32;
            byte[] pixels = CreateSolidPixels(w, h, WebPPixelFormat.Bgra);

            byte[] encoded = WebPEncoder.Encode(pixels, w, h, w * 4, WebPPixelFormat.Bgra, config);
            Assert.True(encoded.Length > 0);
        }

        [Fact]
        public void TestEncodeWithExactMode()
        {
            var config = new WebPEncoderConfig()
                .SetLossless()
                .SetExact();

            int w = 16, h = 16;
            byte[] pixels = CreateSolidPixels(w, h, WebPPixelFormat.Bgra, a: 0); // transparent pixels

            byte[] encoded = WebPEncoder.Encode(pixels, w, h, w * 4, WebPPixelFormat.Bgra, config);
            Assert.True(encoded.Length > 0);

            // Exact mode should preserve RGB under transparent area
            int dw, dh;
            byte[] decoded = WebPDecoder.Decode(encoded, out dw, out dh, WebPPixelFormat.Bgra);
            for (int i = 0; i < pixels.Length; i++)
                Assert.Equal(pixels[i], decoded[i]);
        }

        [Fact]
        public void TestEncodeWithAlphaQuality()
        {
            var config = new WebPEncoderConfig()
                .SetQuality(80)
                .SetAlphaQuality(50);

            int w = 32, h = 32;
            byte[] pixels = CreateSolidPixels(w, h, WebPPixelFormat.Bgra, a: 128);

            byte[] encoded = WebPEncoder.Encode(pixels, w, h, w * 4, WebPPixelFormat.Bgra, config);
            Assert.True(encoded.Length > 0);
        }

        [Fact]
        public void TestEncodeWithConfigToStream()
        {
            var config = new WebPEncoderConfig(WebPPreset.WEBP_PRESET_PHOTO, 85);
            int w = 32, h = 32;
            byte[] pixels = CreateSolidPixels(w, h, WebPPixelFormat.Rgba);

            using (var ms = new MemoryStream())
            {
                WebPEncoder.Encode(pixels, w, h, w * 4, WebPPixelFormat.Rgba, config, ms);
                Assert.True(ms.Length > 0);
            }
        }

        [Fact]
        public void TestEncodeWithConfigNullPixelsThrows()
        {
            var config = new WebPEncoderConfig();
            Assert.Throws<ArgumentNullException>(() =>
                WebPEncoder.Encode(null!, 10, 10, 40, WebPPixelFormat.Bgra, config));
        }

        [Fact]
        public void TestEncodeWithConfigNullConfigThrows()
        {
            byte[] pixels = new byte[100];
            Assert.Throws<ArgumentNullException>(() =>
                WebPEncoder.Encode(pixels, 5, 5, 20, WebPPixelFormat.Bgra, (WebPEncoderConfig)null!));
        }

        /// <summary>
        /// Test SimpleEncoder with WebPEncoderConfig (Bitmap API).
        /// </summary>
        [Fact]
        public void TestSimpleEncoderWithConfig()
        {
            var config = new WebPEncoderConfig(WebPPreset.WEBP_PRESET_PHOTO, 85);
            var encoder = new SimpleEncoder();

            using (var bitmap = new Bitmap(48, 48, PixelFormat.Format32bppArgb))
            using (var ms = new MemoryStream())
            {
                using (var g = Graphics.FromImage(bitmap))
                    g.Clear(Color.ForestGreen);

                encoder.Encode(bitmap, ms, config);
                Assert.True(ms.Length > 0);

                var bytes = ms.ToArray();
                Assert.True(WebPDecoder.IsWebP(bytes));
            }
        }

        [Fact]
        public void TestSimpleEncoderWithLosslessConfig()
        {
            var config = new WebPEncoderConfig().SetLossless();
            var encoder = new SimpleEncoder();
            var decoder = new SimpleDecoder();

            using (var bitmap = new Bitmap(24, 24, PixelFormat.Format32bppArgb))
            {
                using (var g = Graphics.FromImage(bitmap))
                    g.Clear(Color.FromArgb(180, 50, 100, 200));

                using (var ms = new MemoryStream())
                {
                    encoder.Encode(bitmap, ms, config);
                    var bytes = ms.ToArray();
                    using (var decoded = decoder.DecodeFromBytes(bytes, bytes.LongLength))
                    {
                        for (int y = 0; y < bitmap.Height; y++)
                            for (int x = 0; x < bitmap.Width; x++)
                                Assert.Equal(bitmap.GetPixel(x, y).ToArgb(), decoded.GetPixel(x, y).ToArgb());
                    }
                }
            }
        }

        [Fact]
        public void TestSimpleEncoderWithConfig24bpp()
        {
            var config = new WebPEncoderConfig().SetQuality(90);
            var encoder = new SimpleEncoder();

            using (var bitmap = new Bitmap(32, 32, PixelFormat.Format24bppRgb))
            using (var ms = new MemoryStream())
            {
                using (var g = Graphics.FromImage(bitmap))
                    g.Clear(Color.Orange);

                encoder.Encode(bitmap, ms, config);
                Assert.True(ms.Length > 0);
            }
        }

        [Fact]
        public void TestSimpleEncoderWithConfigNonStandardFormat()
        {
            var config = new WebPEncoderConfig().SetQuality(75);
            var encoder = new SimpleEncoder();

            // Format32bppRgb — will be auto-converted to Format32bppArgb
            using (var bitmap = new Bitmap(16, 16, PixelFormat.Format32bppRgb))
            using (var ms = new MemoryStream())
            {
                using (var g = Graphics.FromImage(bitmap))
                    g.Clear(Color.Purple);

                encoder.Encode(bitmap, ms, config);
                Assert.True(ms.Length > 0);
            }
        }

        /// <summary>
        /// Test that quality 90 with config produces similar output to simple encode at quality 90.
        /// </summary>
        [Fact]
        public void TestConfigEncodeComparableToSimpleEncode()
        {
            var config = new WebPEncoderConfig().SetQuality(90);
            int w = 32, h = 32;
            byte[] pixels = CreateSolidPixels(w, h, WebPPixelFormat.Bgra);

            byte[] configEncoded = WebPEncoder.Encode(pixels, w, h, w * 4, WebPPixelFormat.Bgra, config);
            byte[] simpleEncoded = WebPEncoder.Encode(pixels, w, h, w * 4, WebPPixelFormat.Bgra, 90);

            // Both should produce valid WebP with same dimensions
            var info1 = WebPInfo.GetImageInfo(configEncoded);
            var info2 = WebPInfo.GetImageInfo(simpleEncoded);
            Assert.Equal(info1.Width, info2.Width);
            Assert.Equal(info1.Height, info2.Height);
        }

        /// <summary>
        /// Test encoding with Photo preset and various SNS/filter strengths.
        /// </summary>
        [Fact]
        public void TestEncodeWithFullFluentConfig()
        {
            var config = new WebPEncoderConfig(WebPPreset.WEBP_PRESET_PHOTO, 85)
                .SetMethod(4)
                .SetMultiThreaded()
                .SetSnsStrength(75)
                .SetFilterStrength(40)
                .SetAlphaQuality(80)
                .SetImageHint(WebPImageHint.WEBP_HINT_PHOTO);

            Assert.True(config.Validate());

            int w = 64, h = 64;
            byte[] pixels = CreateSolidPixels(w, h, WebPPixelFormat.Bgra);
            byte[] encoded = WebPEncoder.Encode(pixels, w, h, w * 4, WebPPixelFormat.Bgra, config);

            Assert.True(encoded.Length > 0);
            var info = WebPInfo.GetImageInfo(encoded);
            Assert.Equal(w, info.Width);
            Assert.Equal(h, info.Height);
        }
    }
}
