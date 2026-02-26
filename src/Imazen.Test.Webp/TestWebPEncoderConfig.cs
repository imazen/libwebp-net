using System;
using Xunit;
using Imazen.WebP;
using Imazen.WebP.Extern;

namespace Imazen.Test.Webp
{
    public class TestWebPEncoderConfig
    {
        [Fact]
        public void TestDefaultConfigIsValid()
        {
            var config = new WebPEncoderConfig();
            Assert.True(config.Validate());
        }

        [Fact]
        public void TestPresetConfigIsValid()
        {
            var config = new WebPEncoderConfig(WebPPreset.WEBP_PRESET_PHOTO, 85);
            Assert.True(config.Validate());
        }

        [Theory]
        [InlineData(WebPPreset.WEBP_PRESET_DEFAULT)]
        [InlineData(WebPPreset.WEBP_PRESET_PICTURE)]
        [InlineData(WebPPreset.WEBP_PRESET_PHOTO)]
        [InlineData(WebPPreset.WEBP_PRESET_DRAWING)]
        [InlineData(WebPPreset.WEBP_PRESET_ICON)]
        [InlineData(WebPPreset.WEBP_PRESET_TEXT)]
        public void TestAllPresetsValid(WebPPreset preset)
        {
            var config = new WebPEncoderConfig(preset, 75);
            Assert.True(config.Validate());
        }

        [Fact]
        public void TestFluentApi()
        {
            var config = new WebPEncoderConfig()
                .SetQuality(90)
                .SetMethod(4)
                .SetMultiThreaded()
                .SetSnsStrength(50)
                .SetFilterStrength(30)
                .SetAlphaQuality(100);

            Assert.True(config.Validate());
        }

        [Fact]
        public void TestLosslessConfig()
        {
            var config = new WebPEncoderConfig()
                .SetLossless()
                .SetMethod(6);

            Assert.True(config.Validate());
        }

        [Fact]
        public void TestLosslessPreset()
        {
            var config = new WebPEncoderConfig()
                .SetLosslessPreset(6);

            Assert.True(config.Validate());
        }

        [Fact]
        public void TestNearLossless()
        {
            var config = new WebPEncoderConfig()
                .SetLossless()
                .SetNearLossless(60);

            Assert.True(config.Validate());
        }

        [Fact]
        public void TestSharpYuv()
        {
            var config = new WebPEncoderConfig()
                .SetQuality(75)
                .SetSharpYuv();

            Assert.True(config.Validate());
        }

        [Fact]
        public void TestExactMode()
        {
            var config = new WebPEncoderConfig()
                .SetLossless()
                .SetExact();

            Assert.True(config.Validate());
        }
    }
}
