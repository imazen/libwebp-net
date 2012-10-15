using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Imazen.WebP.Extern {
    // Warning: Ignoring Procedure WebPIsPremultipliedMode because it is defined inline.
    // Warning: Ignoring Procedure WebPIsAlphaMode because it is defined inline.
    // Warning: Ignoring Procedure WebPIsRGBMode because it is defined inline.
    // Warning: Ignoring Procedure WebPInitDecBuffer because it is defined inline.
    // Warning: Ignoring Procedure WebPIDecGetYUV because it is defined inline.
    // Warning: Ignoring Procedure WebPGetFeatures because it is defined inline.
    // Warning: Ignoring Procedure WebPInitDecoderConfig because it is defined inline.

    public enum WEBP_CSP_MODE {

        /// MODE_RGB -> 0
        MODE_RGB = 0,

        /// MODE_RGBA -> 1
        MODE_RGBA = 1,

        /// MODE_BGR -> 2
        MODE_BGR = 2,

        /// MODE_BGRA -> 3
        MODE_BGRA = 3,

        /// MODE_ARGB -> 4
        MODE_ARGB = 4,

        /// MODE_RGBA_4444 -> 5
        MODE_RGBA_4444 = 5,

        /// MODE_RGB_565 -> 6
        MODE_RGB_565 = 6,

        /// MODE_rgbA -> 7
        MODE_rgbA = 7,

        /// MODE_bgrA -> 8
        MODE_bgrA = 8,

        /// MODE_Argb -> 9
        MODE_Argb = 9,

        /// MODE_rgbA_4444 -> 10
        MODE_rgbA_4444 = 10,

        /// MODE_YUV -> 11
        MODE_YUV = 11,

        /// MODE_YUVA -> 12
        MODE_YUVA = 12,

        /// MODE_LAST -> 13
        MODE_LAST = 13,
    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct WebPRGBABuffer {

        /// uint8_t*
        [MarshalAsAttribute(UnmanagedType.LPStr)]
        public string rgba;

        /// int
        public int stride;

        /// size_t->unsigned int
        public uint size;
    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct WebPYUVABuffer {

        /// uint8_t*
        [MarshalAsAttribute(UnmanagedType.LPStr)]
        public string y;

        /// uint8_t*
        [MarshalAsAttribute(UnmanagedType.LPStr)]
        public string u;

        /// uint8_t*
        [MarshalAsAttribute(UnmanagedType.LPStr)]
        public string v;

        /// uint8_t*
        [MarshalAsAttribute(UnmanagedType.LPStr)]
        public string a;

        /// int
        public int y_stride;

        /// int
        public int u_stride;

        /// int
        public int v_stride;

        /// int
        public int a_stride;

        /// size_t->unsigned int
        public uint y_size;

        /// size_t->unsigned int
        public uint u_size;

        /// size_t->unsigned int
        public uint v_size;

        /// size_t->unsigned int
        public uint a_size;
    }

    [StructLayoutAttribute(LayoutKind.Explicit)]
    public struct Anonymous_690ed5ec_4c3d_40c6_9bd0_0747b5a28b54 {

        /// WebPRGBABuffer->Anonymous_47cdec86_3c1a_4b39_ab93_76bc7499076a
        [FieldOffsetAttribute(0)]
        public WebPRGBABuffer RGBA;

        /// WebPYUVABuffer->Anonymous_70de6e8e_c3ae_4506_bef0_c17f17a7e678
        [FieldOffsetAttribute(0)]
        public WebPYUVABuffer YUVA;
    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct WebPDecBuffer {

        /// WEBP_CSP_MODE->Anonymous_cb136f5b_1d5d_49a0_aca4_656a79e9d159
        public WEBP_CSP_MODE colorspace;

        /// int
        public int width;

        /// int
        public int height;

        /// int
        public int is_external_memory;

        /// Anonymous_690ed5ec_4c3d_40c6_9bd0_0747b5a28b54
        public Anonymous_690ed5ec_4c3d_40c6_9bd0_0747b5a28b54 u;

        /// uint32_t[4]
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
        public uint[] pad;

        /// uint8_t*
        [MarshalAsAttribute(UnmanagedType.LPStr)]
        public string private_memory;
    }

    public enum VP8StatusCode {

        /// VP8_STATUS_OK -> 0
        VP8_STATUS_OK = 0,

        VP8_STATUS_OUT_OF_MEMORY,

        VP8_STATUS_INVALID_PARAM,

        VP8_STATUS_BITSTREAM_ERROR,

        VP8_STATUS_UNSUPPORTED_FEATURE,

        VP8_STATUS_SUSPENDED,

        VP8_STATUS_USER_ABORT,

        VP8_STATUS_NOT_ENOUGH_DATA,
    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct WebPIDecoder {
    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct WebPBitstreamFeatures {

        /// int
        public int width;

        /// int
        public int height;

        /// int
        public int has_alpha;

        /// int
        public int bitstream_version;

        /// int
        public int no_incremental_decoding;

        /// int
        public int rotate;

        /// int
        public int uv_sampling;

        /// uint32_t[3]
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.U4)]
        public uint[] pad;
    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct WebPDecoderOptions {

        /// int
        public int bypass_filtering;

        /// int
        public int no_fancy_upsampling;

        /// int
        public int use_cropping;

        /// int
        public int crop_left;

        /// int
        public int crop_top;

        /// int
        public int crop_width;

        /// int
        public int crop_height;

        /// int
        public int use_scaling;

        /// int
        public int scaled_width;

        /// int
        public int scaled_height;

        /// int
        public int use_threads;

        /// int
        public int force_rotation;

        /// int
        public int no_enhancement;

        /// uint32_t[6]
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 6, ArraySubType = UnmanagedType.U4)]
        public uint[] pad;
    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct WebPDecoderConfig {

        /// WebPBitstreamFeatures->Anonymous_c6b01f0b_3e38_4731_b2d6_9c0e3bdb71aa
        public WebPBitstreamFeatures input;

        /// WebPDecBuffer->Anonymous_5c438b36_7de6_498e_934a_d3613b37f5fc
        public WebPDecBuffer output;

        /// WebPDecoderOptions->Anonymous_78066979_3e1e_4d74_aee5_f316b20e3385
        public WebPDecoderOptions options;
    }

    public partial class NativeMethods {

        /// Return Type: int
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPGetDecoderVersion")]
        public static extern int WebPGetDecoderVersion();


        /// Return Type: int
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///width: int*
        ///height: int*
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPGetInfo")]
        public static extern int WebPGetInfo([InAttribute()] IntPtr data, [MarshalAsAttribute(UnmanagedType.SysUInt)] uint data_size, ref int width, ref int height);


        /// Return Type: uint8_t*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///width: int*
        ///height: int*
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPDecodeRGBA")]
        public static extern IntPtr WebPDecodeRGBA([InAttribute()] IntPtr data, [MarshalAsAttribute(UnmanagedType.SysUInt)] uint data_size, ref int width, ref int height);


        /// Return Type: uint8_t*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///width: int*
        ///height: int*
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPDecodeARGB")]
        public static extern IntPtr WebPDecodeARGB([InAttribute()] IntPtr data, [MarshalAsAttribute(UnmanagedType.SysUInt)] uint data_size, ref int width, ref int height);


        /// Return Type: uint8_t*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///width: int*
        ///height: int*
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPDecodeBGRA")]
        public static extern IntPtr WebPDecodeBGRA([InAttribute()] IntPtr data, [MarshalAsAttribute(UnmanagedType.SysUInt)] uint data_size, ref int width, ref int height);


        /// Return Type: uint8_t*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///width: int*
        ///height: int*
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPDecodeRGB")]
        public static extern IntPtr WebPDecodeRGB([InAttribute()] IntPtr data, [MarshalAsAttribute(UnmanagedType.SysUInt)] uint data_size, ref int width, ref int height);


        /// Return Type: uint8_t*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///width: int*
        ///height: int*
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPDecodeBGR")]
        public static extern IntPtr WebPDecodeBGR([InAttribute()] IntPtr data, [MarshalAsAttribute(UnmanagedType.SysUInt)] uint data_size, ref int width, ref int height);


        /// Return Type: uint8_t*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///width: int*
        ///height: int*
        ///u: uint8_t**
        ///v: uint8_t**
        ///stride: int*
        ///uv_stride: int*
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPDecodeYUV")]
        public static extern IntPtr WebPDecodeYUV([InAttribute()] IntPtr data, [MarshalAsAttribute(UnmanagedType.SysUInt)] uint data_size, ref int width, ref int height, ref IntPtr u, ref IntPtr v, ref int stride, ref int uv_stride);


        /// Return Type: uint8_t*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///output_buffer: uint8_t*
        ///output_buffer_size: size_t->unsigned int
        ///output_stride: int
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPDecodeRGBAInto")]
        public static extern IntPtr WebPDecodeRGBAInto([InAttribute()] IntPtr data, [MarshalAsAttribute(UnmanagedType.SysUInt)] uint data_size, IntPtr output_buffer, [MarshalAsAttribute(UnmanagedType.SysUInt)] uint output_buffer_size, int output_stride);


        /// Return Type: uint8_t*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///output_buffer: uint8_t*
        ///output_buffer_size: size_t->unsigned int
        ///output_stride: int
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPDecodeARGBInto")]
        public static extern IntPtr WebPDecodeARGBInto([InAttribute()] IntPtr data, [MarshalAsAttribute(UnmanagedType.SysUInt)] uint data_size, IntPtr output_buffer, [MarshalAsAttribute(UnmanagedType.SysUInt)] uint output_buffer_size, int output_stride);


        /// Return Type: uint8_t*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///output_buffer: uint8_t*
        ///output_buffer_size: size_t->unsigned int
        ///output_stride: int
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPDecodeBGRAInto")]
        public static extern IntPtr WebPDecodeBGRAInto([InAttribute()] IntPtr data, [MarshalAsAttribute(UnmanagedType.SysUInt)] uint data_size, IntPtr output_buffer, [MarshalAsAttribute(UnmanagedType.SysUInt)] uint output_buffer_size, int output_stride);


        /// Return Type: uint8_t*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///output_buffer: uint8_t*
        ///output_buffer_size: size_t->unsigned int
        ///output_stride: int
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPDecodeRGBInto")]
        public static extern IntPtr WebPDecodeRGBInto([InAttribute()] IntPtr data, [MarshalAsAttribute(UnmanagedType.SysUInt)] uint data_size, IntPtr output_buffer, [MarshalAsAttribute(UnmanagedType.SysUInt)] uint output_buffer_size, int output_stride);


        /// Return Type: uint8_t*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///output_buffer: uint8_t*
        ///output_buffer_size: size_t->unsigned int
        ///output_stride: int
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPDecodeBGRInto")]
        public static extern IntPtr WebPDecodeBGRInto([InAttribute()] IntPtr data, [MarshalAsAttribute(UnmanagedType.SysUInt)] uint data_size, IntPtr output_buffer, [MarshalAsAttribute(UnmanagedType.SysUInt)] uint output_buffer_size, int output_stride);


        /// Return Type: uint8_t*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///luma: uint8_t*
        ///luma_size: size_t->unsigned int
        ///luma_stride: int
        ///u: uint8_t*
        ///u_size: size_t->unsigned int
        ///u_stride: int
        ///v: uint8_t*
        ///v_size: size_t->unsigned int
        ///v_stride: int
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPDecodeYUVInto")]
        public static extern IntPtr WebPDecodeYUVInto([InAttribute()] IntPtr data, [MarshalAsAttribute(UnmanagedType.SysUInt)] uint data_size, IntPtr luma, [MarshalAsAttribute(UnmanagedType.SysUInt)] uint luma_size, int luma_stride, IntPtr u, [MarshalAsAttribute(UnmanagedType.SysUInt)] uint u_size, int u_stride, IntPtr v, [MarshalAsAttribute(UnmanagedType.SysUInt)] uint v_size, int v_stride);


        /// Return Type: int
        ///param0: WebPDecBuffer*
        ///param1: int
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPInitDecBufferInternal")]
        public static extern int WebPInitDecBufferInternal(ref WebPDecBuffer param0, int param1);


        /// Return Type: void
        ///buffer: WebPDecBuffer*
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPFreeDecBuffer")]
        public static extern void WebPFreeDecBuffer(ref WebPDecBuffer buffer);


        /// Return Type: WebPIDecoder*
        ///output_buffer: WebPDecBuffer*
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPINewDecoder")]
        public static extern IntPtr WebPINewDecoder(ref WebPDecBuffer output_buffer);


        /// Return Type: WebPIDecoder*
        ///csp: WEBP_CSP_MODE->Anonymous_cb136f5b_1d5d_49a0_aca4_656a79e9d159
        ///output_buffer: uint8_t*
        ///output_buffer_size: size_t->unsigned int
        ///output_stride: int
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPINewRGB")]
        public static extern IntPtr WebPINewRGB(WEBP_CSP_MODE csp, IntPtr output_buffer, [MarshalAsAttribute(UnmanagedType.SysUInt)] uint output_buffer_size, int output_stride);


        /// Return Type: WebPIDecoder*
        ///luma: uint8_t*
        ///luma_size: size_t->unsigned int
        ///luma_stride: int
        ///u: uint8_t*
        ///u_size: size_t->unsigned int
        ///u_stride: int
        ///v: uint8_t*
        ///v_size: size_t->unsigned int
        ///v_stride: int
        ///a: uint8_t*
        ///a_size: size_t->unsigned int
        ///a_stride: int
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPINewYUVA")]
        public static extern IntPtr WebPINewYUVA(IntPtr luma, [MarshalAsAttribute(UnmanagedType.SysUInt)] uint luma_size, int luma_stride, IntPtr u, [MarshalAsAttribute(UnmanagedType.SysUInt)] uint u_size, int u_stride, IntPtr v, [MarshalAsAttribute(UnmanagedType.SysUInt)] uint v_size, int v_stride, IntPtr a, [MarshalAsAttribute(UnmanagedType.SysUInt)] uint a_size, int a_stride);


        /// Return Type: WebPIDecoder*
        ///luma: uint8_t*
        ///luma_size: size_t->unsigned int
        ///luma_stride: int
        ///u: uint8_t*
        ///u_size: size_t->unsigned int
        ///u_stride: int
        ///v: uint8_t*
        ///v_size: size_t->unsigned int
        ///v_stride: int
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPINewYUV")]
        public static extern IntPtr WebPINewYUV(IntPtr luma, [MarshalAsAttribute(UnmanagedType.SysUInt)] uint luma_size, int luma_stride, IntPtr u, [MarshalAsAttribute(UnmanagedType.SysUInt)] uint u_size, int u_stride, IntPtr v, [MarshalAsAttribute(UnmanagedType.SysUInt)] uint v_size, int v_stride);


        /// Return Type: void
        ///idec: WebPIDecoder*
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPIDelete")]
        public static extern void WebPIDelete(ref WebPIDecoder idec);


        /// Return Type: VP8StatusCode->Anonymous_b244cc15_fbc7_4c41_8884_71fe4f515cd6
        ///idec: WebPIDecoder*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPIAppend")]
        public static extern VP8StatusCode WebPIAppend(ref WebPIDecoder idec, [InAttribute()] IntPtr data, [MarshalAsAttribute(UnmanagedType.SysUInt)] uint data_size);


        /// Return Type: VP8StatusCode->Anonymous_b244cc15_fbc7_4c41_8884_71fe4f515cd6
        ///idec: WebPIDecoder*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPIUpdate")]
        public static extern VP8StatusCode WebPIUpdate(ref WebPIDecoder idec, [InAttribute()] IntPtr data, [MarshalAsAttribute(UnmanagedType.SysUInt)] uint data_size);


        /// Return Type: uint8_t*
        ///idec: WebPIDecoder*
        ///last_y: int*
        ///width: int*
        ///height: int*
        ///stride: int*
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPIDecGetRGB")]
        public static extern IntPtr WebPIDecGetRGB(ref WebPIDecoder idec, ref int last_y, ref int width, ref int height, ref int stride);


        /// Return Type: uint8_t*
        ///idec: WebPIDecoder*
        ///last_y: int*
        ///u: uint8_t**
        ///v: uint8_t**
        ///a: uint8_t**
        ///width: int*
        ///height: int*
        ///stride: int*
        ///uv_stride: int*
        ///a_stride: int*
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPIDecGetYUVA")]
        public static extern IntPtr WebPIDecGetYUVA(ref WebPIDecoder idec, ref int last_y, ref IntPtr u, ref IntPtr v, ref IntPtr a, ref int width, ref int height, ref int stride, ref int uv_stride, ref int a_stride);


        /// Return Type: WebPDecBuffer*
        ///idec: WebPIDecoder*
        ///left: int*
        ///top: int*
        ///width: int*
        ///height: int*
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPIDecodedArea")]
        public static extern IntPtr WebPIDecodedArea(ref WebPIDecoder idec, ref int left, ref int top, ref int width, ref int height);


        /// Return Type: VP8StatusCode->Anonymous_b244cc15_fbc7_4c41_8884_71fe4f515cd6
        ///param0: uint8_t*
        ///param1: size_t->unsigned int
        ///param2: WebPBitstreamFeatures*
        ///param3: int
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPGetFeaturesInternal")]
        public static extern VP8StatusCode WebPGetFeaturesInternal([InAttribute()] IntPtr param0, [MarshalAsAttribute(UnmanagedType.SysUInt)] uint param1, ref WebPBitstreamFeatures param2, int param3);


        /// Return Type: int
        ///param0: WebPDecoderConfig*
        ///param1: int
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPInitDecoderConfigInternal")]
        public static extern int WebPInitDecoderConfigInternal(ref WebPDecoderConfig param0, int param1);


        /// Return Type: WebPIDecoder*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///config: WebPDecoderConfig*
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPIDecode")]
        public static extern IntPtr WebPIDecode([InAttribute()] IntPtr data, [MarshalAsAttribute(UnmanagedType.SysUInt)] uint data_size, ref WebPDecoderConfig config);


        /// Return Type: VP8StatusCode->Anonymous_b244cc15_fbc7_4c41_8884_71fe4f515cd6
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///config: WebPDecoderConfig*
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPDecode")]
        public static extern VP8StatusCode WebPDecode([InAttribute()] IntPtr data, [MarshalAsAttribute(UnmanagedType.SysUInt)] uint data_size, ref WebPDecoderConfig config);

    }
}