using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;


namespace Imazen.WebP.Extern {
    public enum WebPImageHint {

        /// WEBP_HINT_DEFAULT -> 0
        WEBP_HINT_DEFAULT = 0,

        WEBP_HINT_PICTURE,

        WEBP_HINT_PHOTO,

        WEBP_HINT_GRAPH,

        WEBP_HINT_LAST,
    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct WebPConfig {

        /// int
        public int lossless;

        /// float
        public float quality;

        /// int
        public int method;

        /// WebPImageHint->Anonymous_838f22f5_6f57_48a0_9ecb_8eec917009f9
        public WebPImageHint image_hint;

        /// int
        public int target_size;

        /// float
        public float target_PSNR;

        /// int
        public int segments;

        /// int
        public int sns_strength;

        /// int
        public int filter_strength;

        /// int
        public int filter_sharpness;

        /// int
        public int filter_type;

        /// int
        public int autofilter;

        /// int
        public int alpha_compression;

        /// int
        public int alpha_filtering;

        /// int
        public int alpha_quality;

        /// int
        public int pass;

        /// int
        public int show_compressed;

        /// int
        public int preprocessing;

        /// int
        public int partitions;

        /// int
        public int partition_limit;

        /// uint32_t[8]
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = UnmanagedType.U4)]
        public uint[] pad;
    }

    public enum WebPPreset {

        /// WEBP_PRESET_DEFAULT -> 0
        WEBP_PRESET_DEFAULT = 0,

        WEBP_PRESET_PICTURE,

        WEBP_PRESET_PHOTO,

        WEBP_PRESET_DRAWING,

        WEBP_PRESET_ICON,

        WEBP_PRESET_TEXT,
    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct WebPAuxStats {

        /// int
        public int coded_size;

        /// float[5]
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 5, ArraySubType = UnmanagedType.R4)]
        public float[] PSNR;

        /// int[3]
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I4)]
        public int[] block_count;

        /// int[2]
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I4)]
        public int[] header_bytes;

        /// int[12]
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 12, ArraySubType = UnmanagedType.I4)]
        public int[] residual_bytes;

        /// int[4]
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.I4)]
        public int[] segment_size;

        /// int[4]
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.I4)]
        public int[] segment_quant;

        /// int[4]
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.I4)]
        public int[] segment_level;

        /// int
        public int alpha_data_size;

        /// int
        public int layer_data_size;

        /// uint32_t->unsigned int
        public uint lossless_features;

        /// int
        public int histogram_bits;

        /// int
        public int transform_bits;

        /// int
        public int cache_bits;

        /// int
        public int palette_size;

        /// int
        public int lossless_size;

        /// uint32_t[4]
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
        public uint[] pad;
    }

    /// Return Type: int
    ///data: uint8_t*
    ///data_size: size_t->unsigned int
    ///picture: WebPPicture*
    public delegate int WebPWriterFunction([InAttribute()] IntPtr data, UIntPtr data_size, ref WebPPicture picture);

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct WebPMemoryWriter {

        /// uint8_t*
        public IntPtr mem;

        /// size_t->unsigned int
        public UIntPtr size;

        /// size_t->unsigned int
        public UIntPtr max_size;

        /// uint32_t[1]
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 1, ArraySubType = UnmanagedType.U4)]
        public uint[] pad;
    }

    /// Return Type: int
    ///percent: int
    ///picture: WebPPicture*
    public delegate int WebPProgressHook(int percent, ref WebPPicture picture);

    public enum WebPEncCSP {

        /// WEBP_YUV420 -> 0
        WEBP_YUV420 = 0,

        /// WEBP_YUV422 -> 1
        WEBP_YUV422 = 1,

        /// WEBP_YUV444 -> 2
        WEBP_YUV444 = 2,

        /// WEBP_YUV400 -> 3
        WEBP_YUV400 = 3,

        /// WEBP_CSP_UV_MASK -> 3
        WEBP_CSP_UV_MASK = 3,

        /// WEBP_YUV420A -> 4
        WEBP_YUV420A = 4,

        /// WEBP_YUV422A -> 5
        WEBP_YUV422A = 5,

        /// WEBP_YUV444A -> 6
        WEBP_YUV444A = 6,

        /// WEBP_YUV400A -> 7
        WEBP_YUV400A = 7,

        /// WEBP_CSP_ALPHA_BIT -> 4
        WEBP_CSP_ALPHA_BIT = 4,
    }

    public enum WebPEncodingError {

        /// VP8_ENC_OK -> 0
        VP8_ENC_OK = 0,

        VP8_ENC_ERROR_OUT_OF_MEMORY,

        VP8_ENC_ERROR_BITSTREAM_OUT_OF_MEMORY,

        VP8_ENC_ERROR_NULL_PARAMETER,

        VP8_ENC_ERROR_INVALID_CONFIGURATION,

        VP8_ENC_ERROR_BAD_DIMENSION,

        VP8_ENC_ERROR_PARTITION0_OVERFLOW,

        VP8_ENC_ERROR_PARTITION_OVERFLOW,

        VP8_ENC_ERROR_BAD_WRITE,

        VP8_ENC_ERROR_FILE_TOO_BIG,

        VP8_ENC_ERROR_USER_ABORT,

        VP8_ENC_ERROR_LAST,
    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct WebPPicture {

        /// int
        public int use_argb;

        /// WebPEncCSP->Anonymous_84ce7065_fe91_48b4_93d8_1f0e84319dba
        public WebPEncCSP colorspace;

        /// int
        public int width;

        /// int
        public int height;

        /// uint8_t*
        public IntPtr y;

        /// uint8_t*
        public IntPtr u;

        /// uint8_t*
        public IntPtr v;

        /// int
        public int y_stride;

        /// int
        public int uv_stride;

        /// uint8_t*
        public IntPtr a;

        /// int
        public int a_stride;

        /// uint32_t[2]
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.U4)]
        public uint[] pad1;

        /// uint32_t*
        public IntPtr argb;

        /// int
        public int argb_stride;

        /// uint32_t[3]
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.U4)]
        public uint[] pad2;

        /// WebPWriterFunction
        public WebPWriterFunction writer;

        /// void*
        public IntPtr custom_ptr;

        /// int
        public int extra_info_type;

        /// uint8_t*
        public IntPtr extra_info;

        /// WebPAuxStats*
        public IntPtr stats;

        /// WebPEncodingError->Anonymous_8b714d63_f91b_46af_b0d0_667c703ed356
        public WebPEncodingError error_code;

        /// WebPProgressHook
        public WebPProgressHook progress_hook;

        /// void*
        public IntPtr user_data;

        /// uint32_t[3]
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.U4)]
        public uint[] pad3;

        /// uint8_t*
        public IntPtr u0;

        /// uint8_t*
        public IntPtr v0;

        /// int
        public int uv0_stride;

        /// uint32_t[7]
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 7, ArraySubType = UnmanagedType.U4)]
        public uint[] pad4;

        /// void*
        public IntPtr memory_;

        /// void*
        public IntPtr memory_argb_;

        /// void*[2]
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.SysUInt)]
        public IntPtr[] pad5;
    }

    public partial class NativeMethods {

        /// Return Type: int
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPGetEncoderVersion")]
        public static extern int WebPGetEncoderVersion();


        /// Return Type: size_t->unsigned int
        ///rgb: uint8_t*
        ///width: int
        ///height: int
        ///stride: int
        ///quality_factor: float
        ///output: uint8_t**
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPEncodeRGB")]
        public static extern UIntPtr WebPEncodeRGB([InAttribute()] IntPtr rgb, int width, int height, int stride, float quality_factor, ref IntPtr output);


        /// Return Type: size_t->unsigned int
        ///bgr: uint8_t*
        ///width: int
        ///height: int
        ///stride: int
        ///quality_factor: float
        ///output: uint8_t**
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPEncodeBGR")]
        public static extern UIntPtr WebPEncodeBGR([InAttribute()] IntPtr bgr, int width, int height, int stride, float quality_factor, ref IntPtr output);


        /// Return Type: size_t->unsigned int
        ///rgba: uint8_t*
        ///width: int
        ///height: int
        ///stride: int
        ///quality_factor: float
        ///output: uint8_t**
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPEncodeRGBA")]
        public static extern UIntPtr WebPEncodeRGBA([InAttribute()] IntPtr rgba, int width, int height, int stride, float quality_factor, ref IntPtr output);


        /// Return Type: size_t->unsigned int
        ///bgra: uint8_t*
        ///width: int
        ///height: int
        ///stride: int
        ///quality_factor: float
        ///output: uint8_t**
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPEncodeBGRA")]
        public static extern IntPtr WebPEncodeBGRA([InAttribute()] IntPtr bgra, int width, int height, int stride, float quality_factor, ref IntPtr output);


        /// Return Type: size_t->unsigned int
        ///rgb: uint8_t*
        ///width: int
        ///height: int
        ///stride: int
        ///output: uint8_t**
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPEncodeLosslessRGB")]
        public static extern UIntPtr WebPEncodeLosslessRGB([InAttribute()] IntPtr rgb, int width, int height, int stride, ref IntPtr output);


        /// Return Type: size_t->unsigned int
        ///bgr: uint8_t*
        ///width: int
        ///height: int
        ///stride: int
        ///output: uint8_t**
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPEncodeLosslessBGR")]
        public static extern UIntPtr WebPEncodeLosslessBGR([InAttribute()] IntPtr bgr, int width, int height, int stride, ref IntPtr output);


        /// Return Type: size_t->unsigned int
        ///rgba: uint8_t*
        ///width: int
        ///height: int
        ///stride: int
        ///output: uint8_t**
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPEncodeLosslessRGBA")]
        public static extern UIntPtr WebPEncodeLosslessRGBA([InAttribute()] IntPtr rgba, int width, int height, int stride, ref IntPtr output);


        /// Return Type: size_t->unsigned int
        ///bgra: uint8_t*
        ///width: int
        ///height: int
        ///stride: int
        ///output: uint8_t**
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPEncodeLosslessBGRA")]
        public static extern UIntPtr WebPEncodeLosslessBGRA([InAttribute()] IntPtr bgra, int width, int height, int stride, ref IntPtr output);


        /// Return Type: int
        ///param0: WebPConfig*
        ///param1: WebPPreset->Anonymous_017d4167_f53e_4b3d_b029_592ff5c3f80b
        ///param2: float
        ///param3: int
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPConfigInitInternal")]
        public static extern int WebPConfigInitInternal(ref WebPConfig param0, WebPPreset param1, float param2, int param3);


        /// Return Type: int
        ///config: WebPConfig*
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPValidateConfig")]
        public static extern int WebPValidateConfig(ref WebPConfig config);


        /// Return Type: void
        ///writer: WebPMemoryWriter*
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPMemoryWriterInit")]
        public static extern void WebPMemoryWriterInit(ref WebPMemoryWriter writer);


        /// Return Type: int
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///picture: WebPPicture*
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPMemoryWrite")]
        public static extern int WebPMemoryWrite([InAttribute()] IntPtr data, UIntPtr data_size, ref WebPPicture picture);


        /// Return Type: int
        ///param0: WebPPicture*
        ///param1: int
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPPictureInitInternal")]
        public static extern int WebPPictureInitInternal(ref WebPPicture param0, int param1);


        /// Return Type: int
        ///picture: WebPPicture*
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPPictureAlloc")]
        public static extern int WebPPictureAlloc(ref WebPPicture picture);


        /// Return Type: void
        ///picture: WebPPicture*
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPPictureFree")]
        public static extern void WebPPictureFree(ref WebPPicture picture);


        /// Return Type: int
        ///src: WebPPicture*
        ///dst: WebPPicture*
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPPictureCopy")]
        public static extern int WebPPictureCopy(ref WebPPicture src, ref WebPPicture dst);


        /// Return Type: int
        ///pic1: WebPPicture*
        ///pic2: WebPPicture*
        ///metric_type: int
        ///result: float*
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPPictureDistortion")]
        public static extern int WebPPictureDistortion(ref WebPPicture pic1, ref WebPPicture pic2, int metric_type, ref float result);


        /// Return Type: int
        ///picture: WebPPicture*
        ///left: int
        ///top: int
        ///width: int
        ///height: int
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPPictureCrop")]
        public static extern int WebPPictureCrop(ref WebPPicture picture, int left, int top, int width, int height);


        /// Return Type: int
        ///src: WebPPicture*
        ///left: int
        ///top: int
        ///width: int
        ///height: int
        ///dst: WebPPicture*
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPPictureView")]
        public static extern int WebPPictureView(ref WebPPicture src, int left, int top, int width, int height, ref WebPPicture dst);


        /// Return Type: int
        ///picture: WebPPicture*
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPPictureIsView")]
        public static extern int WebPPictureIsView(ref WebPPicture picture);


        /// Return Type: int
        ///pic: WebPPicture*
        ///width: int
        ///height: int
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPPictureRescale")]
        public static extern int WebPPictureRescale(ref WebPPicture pic, int width, int height);


        /// Return Type: int
        ///picture: WebPPicture*
        ///rgb: uint8_t*
        ///rgb_stride: int
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPPictureImportRGB")]
        public static extern int WebPPictureImportRGB(ref WebPPicture picture, [InAttribute()] IntPtr rgb, int rgb_stride);


        /// Return Type: int
        ///picture: WebPPicture*
        ///rgba: uint8_t*
        ///rgba_stride: int
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPPictureImportRGBA")]
        public static extern int WebPPictureImportRGBA(ref WebPPicture picture, [InAttribute()] IntPtr rgba, int rgba_stride);


        /// Return Type: int
        ///picture: WebPPicture*
        ///rgbx: uint8_t*
        ///rgbx_stride: int
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPPictureImportRGBX")]
        public static extern int WebPPictureImportRGBX(ref WebPPicture picture, [InAttribute()] IntPtr rgbx, int rgbx_stride);


        /// Return Type: int
        ///picture: WebPPicture*
        ///bgr: uint8_t*
        ///bgr_stride: int
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPPictureImportBGR")]
        public static extern int WebPPictureImportBGR(ref WebPPicture picture, [InAttribute()] IntPtr bgr, int bgr_stride);


        /// Return Type: int
        ///picture: WebPPicture*
        ///bgra: uint8_t*
        ///bgra_stride: int
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPPictureImportBGRA")]
        public static extern int WebPPictureImportBGRA(ref WebPPicture picture, [InAttribute()] IntPtr bgra, int bgra_stride);


        /// Return Type: int
        ///picture: WebPPicture*
        ///bgrx: uint8_t*
        ///bgrx_stride: int
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPPictureImportBGRX")]
        public static extern int WebPPictureImportBGRX(ref WebPPicture picture, [InAttribute()] IntPtr bgrx, int bgrx_stride);


        /// Return Type: int
        ///picture: WebPPicture*
        ///colorspace: WebPEncCSP->Anonymous_84ce7065_fe91_48b4_93d8_1f0e84319dba
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPPictureARGBToYUVA")]
        public static extern int WebPPictureARGBToYUVA(ref WebPPicture picture, WebPEncCSP colorspace);


        /// Return Type: int
        ///picture: WebPPicture*
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPPictureYUVAToARGB")]
        public static extern int WebPPictureYUVAToARGB(ref WebPPicture picture);


        /// Return Type: void
        ///picture: WebPPicture*
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPCleanupTransparentArea")]
        public static extern void WebPCleanupTransparentArea(ref WebPPicture picture);


        /// Return Type: int
        ///picture: WebPPicture*
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPPictureHasTransparency")]
        public static extern int WebPPictureHasTransparency(ref WebPPicture picture);


        /// Return Type: int
        ///config: WebPConfig*
        ///picture: WebPPicture*
        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPEncode")]
        public static extern int WebPEncode(ref WebPConfig config, ref WebPPicture picture);

    }
}