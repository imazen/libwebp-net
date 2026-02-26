using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using Imazen.WebP;

namespace Imazen.Test.Webp
{
    /// <summary>
    /// Tests for animated WebP encoding and decoding.
    /// </summary>
    public class TestAnimationApi
    {
        private static byte[] CreateSolidBgraFrame(int w, int h, byte r, byte g, byte b, byte a = 255)
        {
            byte[] pixels = new byte[w * h * 4];
            for (int i = 0; i < w * h; i++)
            {
                pixels[i * 4 + 0] = b;
                pixels[i * 4 + 1] = g;
                pixels[i * 4 + 2] = r;
                pixels[i * 4 + 3] = a;
            }
            return pixels;
        }

        [Fact]
        public void TestEncodeDecodeRoundtrip2Frames()
        {
            int w = 16, h = 16;
            byte[] frame1 = CreateSolidBgraFrame(w, h, 255, 0, 0);   // red
            byte[] frame2 = CreateSolidBgraFrame(w, h, 0, 255, 0);   // green

            byte[] webpData;
            using (var encoder = new AnimEncoder(w, h))
            {
                encoder.AddFrame(frame1, 0, -1);      // lossless at t=0ms
                encoder.AddFrame(frame2, 100, -1);     // lossless at t=100ms
                webpData = encoder.Assemble();
            }

            Assert.True(webpData.Length > 0);
            Assert.True(WebPDecoder.IsWebP(webpData));

            // Verify it's detected as animated
            var info = WebPInfo.GetImageInfo(webpData);
            Assert.Equal(w, info.Width);
            Assert.Equal(h, info.Height);
            Assert.True(info.HasAnimation);

            // Decode frames
            using (var decoder = new AnimDecoder(webpData))
            {
                Assert.Equal(w, decoder.Info.Width);
                Assert.Equal(h, decoder.Info.Height);
                Assert.Equal(2, decoder.Info.FrameCount);

                var frames = decoder.DecodeAllFrames();
                Assert.Equal(2, frames.Count);

                // Frame 1 should be red (lossless = pixel-perfect)
                Assert.Equal(0, frames[0].Pixels[0]);     // B
                Assert.Equal(0, frames[0].Pixels[1]);     // G
                Assert.Equal(255, frames[0].Pixels[2]);   // R
                Assert.Equal(255, frames[0].Pixels[3]);   // A

                // Frame 2 should be green
                Assert.Equal(0, frames[1].Pixels[0]);     // B
                Assert.Equal(255, frames[1].Pixels[1]);   // G
                Assert.Equal(0, frames[1].Pixels[2]);     // R
                Assert.Equal(255, frames[1].Pixels[3]);   // A
            }
        }

        [Fact]
        public void TestEncodeDecodeRoundtrip3Frames()
        {
            int w = 8, h = 8;
            byte[] r = CreateSolidBgraFrame(w, h, 255, 0, 0);
            byte[] g = CreateSolidBgraFrame(w, h, 0, 255, 0);
            byte[] b = CreateSolidBgraFrame(w, h, 0, 0, 255);

            byte[] webpData;
            using (var encoder = new AnimEncoder(w, h))
            {
                encoder.AddFrame(r, 0, -1);
                encoder.AddFrame(g, 200, -1);
                encoder.AddFrame(b, 400, -1);
                webpData = encoder.Assemble();
            }

            using (var decoder = new AnimDecoder(webpData))
            {
                Assert.Equal(3, decoder.Info.FrameCount);
                var frames = decoder.DecodeAllFrames();
                Assert.Equal(3, frames.Count);

                // Check timestamps
                Assert.Equal(0, frames[0].TimestampMs);
                Assert.Equal(200, frames[1].TimestampMs);
                Assert.Equal(400, frames[2].TimestampMs);

                // Check computed durations
                Assert.Equal(200, frames[0].DurationMs);
                Assert.Equal(200, frames[1].DurationMs);
                Assert.Equal(0, frames[2].DurationMs); // last frame
            }
        }

        [Fact]
        public void TestLoopCount()
        {
            int w = 4, h = 4;
            byte[] frame = CreateSolidBgraFrame(w, h, 128, 128, 128);

            byte[] webpData;
            using (var encoder = new AnimEncoder(w, h, loopCount: 3))
            {
                encoder.AddFrame(frame, 0, -1);
                encoder.AddFrame(frame, 50, -1);
                webpData = encoder.Assemble();
            }

            using (var decoder = new AnimDecoder(webpData))
            {
                Assert.Equal(3, decoder.Info.LoopCount);
            }
        }

        [Fact]
        public void TestInfiniteLoop()
        {
            int w = 4, h = 4;
            byte[] frame = CreateSolidBgraFrame(w, h, 200, 100, 50);

            byte[] webpData;
            using (var encoder = new AnimEncoder(w, h, loopCount: 0))
            {
                encoder.AddFrame(frame, 0, -1);
                encoder.AddFrame(frame, 100, -1);
                webpData = encoder.Assemble();
            }

            using (var decoder = new AnimDecoder(webpData))
            {
                Assert.Equal(0, decoder.Info.LoopCount); // 0 = infinite
            }
        }

        [Fact]
        public void TestAnimationWithAlpha()
        {
            int w = 8, h = 8;
            byte[] frame1 = CreateSolidBgraFrame(w, h, 255, 0, 0, 128);   // semi-transparent red
            byte[] frame2 = CreateSolidBgraFrame(w, h, 0, 0, 255, 64);    // mostly-transparent blue

            byte[] webpData;
            using (var encoder = new AnimEncoder(w, h))
            {
                encoder.AddFrame(frame1, 0, -1);
                encoder.AddFrame(frame2, 100, -1);
                webpData = encoder.Assemble();
            }

            var info = WebPInfo.GetImageInfo(webpData);
            Assert.True(info.HasAlpha);
            Assert.True(info.HasAnimation);

            using (var decoder = new AnimDecoder(webpData))
            {
                var frames = decoder.DecodeAllFrames();
                Assert.Equal(2, frames.Count);

                // Verify alpha is preserved (lossless)
                Assert.Equal(128, frames[0].Pixels[3]); // A of frame 1
                Assert.Equal(64, frames[1].Pixels[3]);   // A of frame 2
            }
        }

        [Fact]
        public void TestLossyAnimation()
        {
            int w = 16, h = 16;
            byte[] frame1 = CreateSolidBgraFrame(w, h, 255, 0, 0);
            byte[] frame2 = CreateSolidBgraFrame(w, h, 0, 255, 0);

            byte[] webpData;
            using (var encoder = new AnimEncoder(w, h))
            {
                encoder.AddFrame(frame1, 0, 75);    // lossy
                encoder.AddFrame(frame2, 100, 75);   // lossy
                webpData = encoder.Assemble();
            }

            Assert.True(webpData.Length > 0);

            using (var decoder = new AnimDecoder(webpData))
            {
                Assert.Equal(2, decoder.Info.FrameCount);
                var frames = decoder.DecodeAllFrames();
                Assert.Equal(2, frames.Count);

                // Lossy — colors should be approximate
                Assert.True(frames[0].Pixels[2] > 200); // R should be close to 255
                Assert.True(frames[1].Pixels[1] > 200); // G should be close to 255
            }
        }

        [Fact]
        public void TestFrameIterator()
        {
            int w = 8, h = 8;

            byte[] webpData;
            using (var encoder = new AnimEncoder(w, h))
            {
                for (int i = 0; i < 5; i++)
                {
                    byte[] frame = CreateSolidBgraFrame(w, h, (byte)(i * 50), 100, 200);
                    encoder.AddFrame(frame, i * 50, -1);
                }
                webpData = encoder.Assemble();
            }

            using (var decoder = new AnimDecoder(webpData))
            {
                int count = 0;
                while (decoder.HasMoreFrames())
                {
                    var frame = decoder.GetNextFrame();
                    Assert.NotNull(frame);
                    Assert.Equal(w, frame!.Width);
                    Assert.Equal(h, frame.Height);
                    count++;
                }
                Assert.Equal(5, count);

                // After exhausting, should return null
                Assert.Null(decoder.GetNextFrame());
            }
        }

        [Fact]
        public void TestDecoderReset()
        {
            int w = 4, h = 4;
            byte[] frame1 = CreateSolidBgraFrame(w, h, 255, 0, 0);
            byte[] frame2 = CreateSolidBgraFrame(w, h, 0, 255, 0);

            byte[] webpData;
            using (var encoder = new AnimEncoder(w, h))
            {
                encoder.AddFrame(frame1, 0, -1);
                encoder.AddFrame(frame2, 100, -1);
                webpData = encoder.Assemble();
            }

            using (var decoder = new AnimDecoder(webpData))
            {
                // First pass
                var pass1 = decoder.DecodeAllFrames();
                Assert.Equal(2, pass1.Count);

                // Reset and decode again
                decoder.Reset();
                var pass2 = decoder.DecodeAllFrames();
                Assert.Equal(2, pass2.Count);

                // Should be identical
                for (int i = 0; i < pass1[0].Pixels.Length; i++)
                    Assert.Equal(pass1[0].Pixels[i], pass2[0].Pixels[i]);
            }
        }

        [Fact]
        public void TestAssembleToStream()
        {
            int w = 8, h = 8;
            byte[] frame = CreateSolidBgraFrame(w, h, 100, 100, 100);

            using (var encoder = new AnimEncoder(w, h))
            using (var ms = new MemoryStream())
            {
                encoder.AddFrame(frame, 0, -1);
                encoder.AddFrame(frame, 100, -1);
                encoder.Assemble(ms);
                Assert.True(ms.Length > 0);
            }
        }

        [Fact]
        public void TestDecoderFromStream()
        {
            int w = 8, h = 8;
            byte[] frame = CreateSolidBgraFrame(w, h, 50, 100, 150);

            byte[] webpData;
            using (var encoder = new AnimEncoder(w, h))
            {
                encoder.AddFrame(frame, 0, -1);
                encoder.AddFrame(frame, 50, -1);
                webpData = encoder.Assemble();
            }

            using (var stream = new MemoryStream(webpData))
            using (var decoder = new AnimDecoder(stream))
            {
                Assert.Equal(2, decoder.Info.FrameCount);
                Assert.Equal(w, decoder.Info.Width);
                Assert.Equal(h, decoder.Info.Height);
            }
        }

        [Fact]
        public void TestAnimationWithDifferentFormats()
        {
            int w = 8, h = 8;
            // RGBA format
            byte[] rgbaFrame = new byte[w * h * 4];
            for (int i = 0; i < w * h; i++)
            {
                rgbaFrame[i * 4 + 0] = 255; // R
                rgbaFrame[i * 4 + 1] = 128; // G
                rgbaFrame[i * 4 + 2] = 0;   // B
                rgbaFrame[i * 4 + 3] = 255; // A
            }

            byte[] webpData;
            using (var encoder = new AnimEncoder(w, h))
            {
                encoder.AddFrame(rgbaFrame, w * 4, WebPPixelFormat.Rgba, 0, -1);
                encoder.AddFrame(rgbaFrame, w * 4, WebPPixelFormat.Rgba, 100, -1);
                webpData = encoder.Assemble();
            }

            using (var decoder = new AnimDecoder(webpData))
            {
                Assert.Equal(2, decoder.Info.FrameCount);
                var frames = decoder.DecodeAllFrames();
                Assert.Equal(2, frames.Count);
            }
        }

        [Fact]
        public void TestAnimationWithAdvancedConfig()
        {
            int w = 16, h = 16;
            byte[] frame1 = CreateSolidBgraFrame(w, h, 200, 50, 100);
            byte[] frame2 = CreateSolidBgraFrame(w, h, 50, 200, 100);

            var config = new WebPEncoderConfig()
                .SetQuality(80)
                .SetMethod(4)
                .SetSharpYuv();

            byte[] webpData;
            using (var encoder = new AnimEncoder(w, h))
            {
                encoder.AddFrame(frame1, w * 4, WebPPixelFormat.Bgra, 0, config);
                encoder.AddFrame(frame2, w * 4, WebPPixelFormat.Bgra, 200, config);
                webpData = encoder.Assemble();
            }

            using (var decoder = new AnimDecoder(webpData))
            {
                Assert.Equal(2, decoder.Info.FrameCount);
            }
        }

        [Fact]
        public void TestEncoderDispose()
        {
            var encoder = new AnimEncoder(4, 4);
            encoder.Dispose();
            // Should throw after dispose
            Assert.Throws<ObjectDisposedException>(() =>
                encoder.AddFrame(new byte[64], 0, -1));
        }

        [Fact]
        public void TestDecoderDispose()
        {
            int w = 4, h = 4;
            byte[] frame = CreateSolidBgraFrame(w, h, 128, 128, 128);

            byte[] webpData;
            using (var encoder = new AnimEncoder(w, h))
            {
                encoder.AddFrame(frame, 0, -1);
                encoder.AddFrame(frame, 50, -1);
                webpData = encoder.Assemble();
            }

            var decoder = new AnimDecoder(webpData);
            decoder.Dispose();
            Assert.Throws<ObjectDisposedException>(() => decoder.HasMoreFrames());
        }

        [Fact]
        public void TestEncoderNullFrameThrows()
        {
            using (var encoder = new AnimEncoder(8, 8))
            {
                Assert.Throws<ArgumentNullException>(() => encoder.AddFrame(null!, 0, -1));
            }
        }

        [Fact]
        public void TestDecoderNullDataThrows()
        {
            Assert.Throws<ArgumentNullException>(() => new AnimDecoder((byte[])null!));
        }

        [Fact]
        public void TestManyFrames()
        {
            int w = 4, h = 4;

            byte[] webpData;
            using (var encoder = new AnimEncoder(w, h))
            {
                for (int i = 0; i < 20; i++)
                {
                    byte c = (byte)(i * 12);
                    byte[] frame = CreateSolidBgraFrame(w, h, c, c, c);
                    encoder.AddFrame(frame, i * 50, -1);
                }
                webpData = encoder.Assemble();
            }

            using (var decoder = new AnimDecoder(webpData))
            {
                Assert.Equal(20, decoder.Info.FrameCount);
                var frames = decoder.DecodeAllFrames();
                Assert.Equal(20, frames.Count);
            }
        }
    }
}
