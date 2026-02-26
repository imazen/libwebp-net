using System;
using System.Runtime.InteropServices;
using Xunit;
using Imazen.WebP.Extern;

namespace Imazen.Test.Webp
{
    /// <summary>
    /// Static assertions on P/Invoke struct sizes and field offsets.
    /// These catch ABI drift — if libwebp changes a struct layout, these tests fail
    /// before the mismatch causes heap corruption at runtime.
    ///
    /// Expected values are derived from the libwebp 1.6.0 headers and verified
    /// by tools/abi_check.c compiled in CI.
    /// </summary>
    public class TestPInvokeLayout
    {
        private static bool Is64Bit => IntPtr.Size == 8;

        // =====================================================================
        // WebPConfig — 29 fields, no padding (libwebp 1.6.0)
        // All fields are int (4 bytes) or float (4 bytes), so size = 29 * 4 = 116
        // =====================================================================

        [Fact]
        public void WebPConfig_Size()
        {
            // 29 fields × 4 bytes each = 116 bytes
            Assert.Equal(116, Marshal.SizeOf<WebPConfig>());
        }

        [Fact]
        public void WebPConfig_QualityOffset()
        {
            Assert.Equal(4, (int)Marshal.OffsetOf<WebPConfig>("quality"));
        }

        [Fact]
        public void WebPConfig_MethodOffset()
        {
            Assert.Equal(8, (int)Marshal.OffsetOf<WebPConfig>("method"));
        }

        [Fact]
        public void WebPConfig_QminOffset()
        {
            // Field 27 (0-indexed): 27 * 4 = 108
            Assert.Equal(108, (int)Marshal.OffsetOf<WebPConfig>("qmin"));
        }

        [Fact]
        public void WebPConfig_QmaxOffset()
        {
            // Field 28 (0-indexed): 28 * 4 = 112
            Assert.Equal(112, (int)Marshal.OffsetOf<WebPConfig>("qmax"));
        }

        // =====================================================================
        // WebPBitstreamFeatures — 5 ints + pad[5] = 40 bytes
        // =====================================================================

        [Fact]
        public void WebPBitstreamFeatures_Size()
        {
            Assert.Equal(40, Marshal.SizeOf<WebPBitstreamFeatures>());
        }

        // =====================================================================
        // WebPDecoderOptions — 14 ints + pad[5] = 76 bytes
        // =====================================================================

        [Fact]
        public void WebPDecoderOptions_Size()
        {
            Assert.Equal(76, Marshal.SizeOf<WebPDecoderOptions>());
        }

        // =====================================================================
        // WebPRGBABuffer — 1 ptr + 1 int + 1 size_t
        // 64-bit: 8 + 4 + (4 pad) + 8 = 24  or 8 + 4 + 4(pad) + 8 = 24
        // 32-bit: 4 + 4 + 4 = 12
        // =====================================================================

        [Fact]
        public void WebPRGBABuffer_Size()
        {
            int expected = Is64Bit ? 24 : 12;
            Assert.Equal(expected, Marshal.SizeOf<WebPRGBABuffer>());
        }

        // =====================================================================
        // WebPYUVABuffer — 4 ptrs + 4 ints + 4 size_t
        // 64-bit: 4*8 + 4*4 + 4*8 = 32 + 16 + 32 = 80
        // 32-bit: 4*4 + 4*4 + 4*4 = 16 + 16 + 16 = 48
        // =====================================================================

        [Fact]
        public void WebPYUVABuffer_Size()
        {
            int expected = Is64Bit ? 80 : 48;
            Assert.Equal(expected, Marshal.SizeOf<WebPYUVABuffer>());
        }

        // =====================================================================
        // WebPDecBuffer — colorspace(4) + width(4) + height(4) + is_external(4) +
        //   union(max(RGBA,YUVA)) + pad[4](16) + private_memory(ptr)
        // 64-bit: 16 + 80 + 16 + 8 = 120
        // 32-bit: 16 + 48 + 16 + 4 = 84
        // =====================================================================

        [Fact]
        public void WebPDecBuffer_Size()
        {
            int expected = Is64Bit ? 120 : 84;
            Assert.Equal(expected, Marshal.SizeOf<WebPDecBuffer>());
        }

        // =====================================================================
        // WebPMuxAnimParams — bgcolor(4) + loop_count(4) = 8
        // =====================================================================

        [Fact]
        public void WebPMuxAnimParams_Size()
        {
            Assert.Equal(8, Marshal.SizeOf<WebPMuxAnimParams>());
        }

        // =====================================================================
        // WebPAnimEncoderOptions — anim_params(8) + 5 ints + padding[4](16)
        //   = 8 + 20 + 16 = 44
        // =====================================================================

        [Fact]
        public void WebPAnimEncoderOptions_Size()
        {
            Assert.Equal(44, Marshal.SizeOf<WebPAnimEncoderOptions>());
        }

        // =====================================================================
        // WebPAnimDecoderOptions — color_mode(4) + use_threads(4) + padding[7](28) = 36
        // =====================================================================

        [Fact]
        public void WebPAnimDecoderOptions_Size()
        {
            Assert.Equal(36, Marshal.SizeOf<WebPAnimDecoderOptions>());
        }

        // =====================================================================
        // WebPAnimInfo — 5 uints + pad[4] = 36
        // =====================================================================

        [Fact]
        public void WebPAnimInfo_Size()
        {
            Assert.Equal(36, Marshal.SizeOf<WebPAnimInfo>());
        }

        // =====================================================================
        // WebPData — bytes(ptr) + size(size_t)
        // 64-bit: 8 + 8 = 16
        // 32-bit: 4 + 4 = 8
        // =====================================================================

        [Fact]
        public void WebPData_Size()
        {
            int expected = Is64Bit ? 16 : 8;
            Assert.Equal(expected, Marshal.SizeOf<WebPData>());
        }

        // =====================================================================
        // WebPMemoryWriter — mem(ptr) + size(size_t) + max_size(size_t) + pad[1](4)
        // 64-bit: 8 + 8 + 8 + 4 = 28 → padded to 32? Actually no padding needed
        //   unless alignment forces it. Let's check: 8+8+8+4 = 28, but struct
        //   has pointer so alignment is 8. 28 rounds up to 32.
        // 32-bit: 4 + 4 + 4 + 4 = 16
        // =====================================================================

        [Fact]
        public void WebPMemoryWriter_Size()
        {
            int expected = Is64Bit ? 32 : 16;
            Assert.Equal(expected, Marshal.SizeOf<WebPMemoryWriter>());
        }

        // =====================================================================
        // WebPAuxStats — complex struct with arrays
        // All int/float/uint32 fields, no pointers, so same on 32/64-bit
        // coded_size(4) + PSNR[5](20) + block_count[3](12) + header_bytes[2](8) +
        // residual_bytes[12](48) + segment_size[4](16) + segment_quant[4](16) +
        // segment_level[4](16) + alpha_data_size(4) + layer_data_size(4) +
        // lossless_features(4) + histogram_bits(4) + transform_bits(4) +
        // cache_bits(4) + palette_size(4) + lossless_size(4) +
        // lossless_hdr_size(4) + lossless_data_size(4) +
        // cross_color_transform_bits(4) + pad[1](4)
        // = 4+20+12+8+48+16+16+16+4+4+4+4+4+4+4+4+4+4+4+4 = 188
        // =====================================================================

        [Fact]
        public void WebPAuxStats_Size()
        {
            Assert.Equal(188, Marshal.SizeOf<WebPAuxStats>());
        }

        // =====================================================================
        // WebPPicture — large struct with many pointers, differs on 32/64-bit
        // =====================================================================

        [Fact]
        public void WebPPicture_SizeIsPositive()
        {
            int size = Marshal.SizeOf<WebPPicture>();
            // Sanity check: struct must be at least large enough for all fields
            Assert.True(size > 100, $"WebPPicture size too small: {size}");
            // On 64-bit it should be around 200+, on 32-bit around 140+
            if (Is64Bit)
                Assert.True(size >= 176, $"WebPPicture size unexpectedly small on 64-bit: {size}");
            else
                Assert.True(size >= 136, $"WebPPicture size unexpectedly small on 32-bit: {size}");
        }

        [Fact]
        public void WebPPicture_WidthOffset()
        {
            // use_argb(int=4) + colorspace(enum=4) = 8
            Assert.Equal(8, (int)Marshal.OffsetOf<WebPPicture>("width"));
        }

        [Fact]
        public void WebPPicture_HeightOffset()
        {
            Assert.Equal(12, (int)Marshal.OffsetOf<WebPPicture>("height"));
        }

        // =====================================================================
        // WebPDecoderConfig — composed of 3 nested structs
        // input(WebPBitstreamFeatures) + output(WebPDecBuffer) + options(WebPDecoderOptions)
        // =====================================================================

        [Fact]
        public void WebPDecoderConfig_Size()
        {
            int componentSum = Marshal.SizeOf<WebPBitstreamFeatures>()
                             + Marshal.SizeOf<WebPDecBuffer>()
                             + Marshal.SizeOf<WebPDecoderOptions>();
            int actual = Marshal.SizeOf<WebPDecoderConfig>();
            // The struct may include trailing alignment padding
            Assert.True(actual >= componentSum,
                $"WebPDecoderConfig ({actual}) should be >= sum of components ({componentSum})");
            // Must be a multiple of pointer size due to alignment
            Assert.Equal(0, actual % IntPtr.Size);
        }

        // =====================================================================
        // Enum sizes — all C enums are int-sized
        // =====================================================================

        [Fact]
        public unsafe void EnumSizes()
        {
            // Enums are int-sized (4 bytes) in C; C# enums default to int backing
            Assert.Equal(4, sizeof(WEBP_CSP_MODE));
            Assert.Equal(4, sizeof(VP8StatusCode));
            Assert.Equal(4, sizeof(WebPImageHint));
            Assert.Equal(4, sizeof(WebPPreset));
            Assert.Equal(4, sizeof(WebPEncCSP));
            Assert.Equal(4, sizeof(WebPEncodingError));
        }
    }
}
