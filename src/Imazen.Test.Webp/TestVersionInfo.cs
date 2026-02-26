using System;
using Xunit;
using Imazen.WebP;
using Imazen.WebP.Extern;

namespace Imazen.Test.Webp
{
    public class TestVersionInfo
    {
        [Fact]
        public void DecoderVersionIsReasonable()
        {
            // Use public API to trigger library loading
            var versionStr = SimpleDecoder.GetDecoderVersion();
            Assert.NotNull(versionStr);

            int ver = NativeMethods.WebPGetDecoderVersion();
            int major = (ver >> 16) & 0xFF;
            int minor = (ver >> 8) & 0xFF;
            Assert.Equal(1, major);
            Assert.True(minor >= 6, $"Expected decoder minor >= 6, got {major}.{minor}");
        }

        [Fact]
        public void EncoderVersionIsReasonable()
        {
            // Trigger library loading via public API
            _ = SimpleEncoder.GetEncoderVersion();

            int ver = NativeMethods.WebPGetEncoderVersion();
            int major = (ver >> 16) & 0xFF;
            int minor = (ver >> 8) & 0xFF;
            Assert.Equal(1, major);
            Assert.True(minor >= 6, $"Expected encoder minor >= 6, got {major}.{minor}");
        }

        [Fact]
        public void AbiVersionCheck_Passes()
        {
            // Trigger library loading
            _ = SimpleDecoder.GetDecoderVersion();
            AbiVersionCheck.ValidateOrThrow();
        }

        [Fact]
        public void GetVersionString_ReturnsNonEmpty()
        {
            string version = AbiVersionCheck.GetVersionString();
            Assert.NotNull(version);
            Assert.NotEmpty(version);
            Assert.Contains("decoder=", version);
            Assert.Contains("encoder=", version);
        }

        [Fact]
        public void AbiConstants_MatchExpectedMajorVersion()
        {
            // ABI major version should be 0x02 for libwebp 1.x
            Assert.Equal(0x02, NativeMethods.WEBP_DECODER_ABI_VERSION >> 8);
            Assert.Equal(0x02, NativeMethods.WEBP_ENCODER_ABI_VERSION >> 8);
            Assert.Equal(0x01, NativeMethods.WEBP_MUX_ABI_VERSION >> 8);
            Assert.Equal(0x01, NativeMethods.WEBP_DEMUX_ABI_VERSION >> 8);
        }
    }
}
