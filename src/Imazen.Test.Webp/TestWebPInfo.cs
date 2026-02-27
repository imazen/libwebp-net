using System;
using System.IO;
using Xunit;
using Imazen.WebP;

namespace Imazen.Test.Webp
{
    public class TestWebPInfo
    {
        [Fact]
        public void TestGetSizeFromFile()
        {
            var fileName = "testimage.webp";
            if (!File.Exists(fileName)) return;

            byte[] data;
            using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var ms = new MemoryStream())
            {
                fs.CopyTo(ms);
                data = ms.ToArray();
            }
            bool result = WebPInfo.TryGetSize(data, out int width, out int height);

            Assert.True(result);
            Assert.True(width > 0);
            Assert.True(height > 0);
        }

        [Fact]
        public void TestGetImageInfoFromFile()
        {
            var fileName = "testimage.webp";
            if (!File.Exists(fileName)) return;

            byte[] data;
            using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var ms = new MemoryStream())
            {
                fs.CopyTo(ms);
                data = ms.ToArray();
            }
            var info = WebPInfo.GetImageInfo(data);

            Assert.True(info.Width > 0);
            Assert.True(info.Height > 0);
            Assert.True(info.Format >= 0 && info.Format <= 2);
        }

        [Fact]
        public void TestGetImageInfoFromEncodedData()
        {
            // Encode a known image, then probe it
            int w = 32, h = 24;
            byte[] pixels = new byte[w * h * 4];
            // Fill with opaque red (BGRA)
            for (int i = 0; i < w * h; i++)
            {
                pixels[i * 4 + 0] = 0;     // B
                pixels[i * 4 + 1] = 0;     // G
                pixels[i * 4 + 2] = 255;   // R
                pixels[i * 4 + 3] = 255;   // A
            }

            byte[] encoded = WebPEncoder.Encode(pixels, w, h, w * 4, WebPPixelFormat.Bgra, 75);
            var info = WebPInfo.GetImageInfo(encoded);

            Assert.Equal(w, info.Width);
            Assert.Equal(h, info.Height);
        }

        [Fact]
        public void TestGetImageInfoLossless()
        {
            int w = 16, h = 16;
            byte[] pixels = new byte[w * h * 4];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = 128;

            byte[] encoded = WebPEncoder.Encode(pixels, w, h, w * 4, WebPPixelFormat.Bgra, -1);
            var info = WebPInfo.GetImageInfo(encoded);

            Assert.Equal(w, info.Width);
            Assert.Equal(h, info.Height);
            Assert.Equal(2, info.Format); // lossless = 2
        }

        [Fact]
        public void TestTryGetSizeInvalidData()
        {
            byte[] badData = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
            bool result = WebPInfo.TryGetSize(badData, out int width, out int height);
            Assert.False(result);
        }

        [Fact]
        public void TestTryGetSizeTooShort()
        {
            byte[] shortData = { 0, 1, 2 };
            bool result = WebPInfo.TryGetSize(shortData, out int width, out int height);
            Assert.False(result);
        }

        [Fact]
        public void TestGetImageInfoNullThrows()
        {
            Assert.Throws<ArgumentNullException>(() => WebPInfo.GetImageInfo(null!));
        }

        [Fact]
        public void TestGetImageInfoTooShortThrows()
        {
            Assert.Throws<ArgumentException>(() => WebPInfo.GetImageInfo(new byte[] { 0, 1 }));
        }
    }
}
