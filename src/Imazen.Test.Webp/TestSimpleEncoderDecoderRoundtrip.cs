using System;
using System.Drawing;
using System.Drawing.Imaging;
using Xunit;
using Imazen.WebP;

namespace Imazen.Test.Webp
{
    public class TestSimpleEncoderDecoderRoundtrip
    {
        private static readonly Random random = new Random();

        private static byte RandomByte()
        {
            return (byte)random.Next(255);
        }

        private static Color RandomRgb()
        {
            return Color.FromArgb(0, RandomByte(), RandomByte(), RandomByte());
        }

        private static Color RandomArgb()
        {
            return Color.FromArgb(RandomByte(), RandomRgb());
        }

        private Bitmap GenerateTestBitmap(PixelFormat fmt, int width, int height, Func<Color> pixelValue)
        {
            var bitmap = new Bitmap(width, height, fmt);
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var color = pixelValue();
                    bitmap.SetPixel(x, y, color);
                }
            }

            return bitmap;
        }

        private void TestLosslessRoundtrip(Bitmap gdiBitmap)
        {
            var encoder = new SimpleEncoder();
            var decoder = new SimpleDecoder();

            using (var outStream = new System.IO.MemoryStream())
            {
                encoder.Encode(gdiBitmap, outStream, -1);
                outStream.Close();

                var webpBytes = outStream.ToArray();
                var reloaded = decoder.DecodeFromBytes(webpBytes, webpBytes.LongLength);

                Assert.Equal(gdiBitmap.Height, reloaded.Height);
                Assert.Equal(gdiBitmap.Width, reloaded.Width);

                for (var y = 0; y < reloaded.Height; y++)
                {
                    for (var x = 0; x < reloaded.Width; x++)
                    {
                        var expectedColor = gdiBitmap.GetPixel(x, y);
                        var actualColor   = reloaded.GetPixel(x, y);
                        Assert.Equal(expectedColor, actualColor);
                    }
                }
            }
        }

        [Fact]
        public void TestRgb32()
        {
            Imazen.WebP.Extern.LoadLibrary.LoadWebPOrFail();

            using (var gdiBitmap = GenerateTestBitmap(PixelFormat.Format32bppRgb, 10, 10, TestSimpleEncoderDecoderRoundtrip.RandomRgb))
            {
                TestLosslessRoundtrip(gdiBitmap);
            }
        }

        [Fact]
        public void TestRgb24()
        {
            Imazen.WebP.Extern.LoadLibrary.LoadWebPOrFail();

            using (var gdiBitmap = GenerateTestBitmap(PixelFormat.Format24bppRgb, 10, 10, TestSimpleEncoderDecoderRoundtrip.RandomRgb))
            {
                TestLosslessRoundtrip(gdiBitmap);
            }
        }

        [Fact]
        public void TestAgb32()
        {
            Imazen.WebP.Extern.LoadLibrary.LoadWebPOrFail();

            using (var gdiBitmap = GenerateTestBitmap(PixelFormat.Format32bppArgb, 10, 10, TestSimpleEncoderDecoderRoundtrip.RandomArgb))
            {
                TestLosslessRoundtrip(gdiBitmap);
            }
        }
    }
}
