using System;
using System.Runtime.InteropServices;

#pragma warning disable 1591

namespace Imazen.WebP.Extern
{

    public enum WebPImageHint
    {
        /// WEBP_HINT_DEFAULT -> 0
        WEBP_HINT_DEFAULT = 0,
        WEBP_HINT_PICTURE,
        WEBP_HINT_PHOTO,
        WEBP_HINT_GRAPH,
        WEBP_HINT_LAST,
    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct WebPConfig
    {
        /// Lossless encoding (0=lossy(default), 1=lossless).
        public int lossless;

        /// between 0 (smallest file) and 100 (biggest)
        public float quality;

        /// quality/speed trade-off (0=fast, 6=slower-better)
        public int method;

        /// WebPImageHint
        public WebPImageHint image_hint;

        // Parameters related to lossy compression only:

        /// if non-zero, set the desired target size in bytes.
        public int target_size;

        /// if non-zero, specifies the minimal distortion to try to achieve.
        public float target_PSNR;

        /// maximum number of segments to use, in [1..4]
        public int segments;

        /// Spatial Noise Shaping. 0=off, 100=maximum.
        public int sns_strength;

        /// range: [0 = off .. 100 = strongest]
        public int filter_strength;

        /// range: [0 = off .. 7 = least sharp]
        public int filter_sharpness;

        /// filtering type: 0 = simple, 1 = strong
        public int filter_type;

        ///  Auto adjust filter's strength [0 = off, 1 = on]
        public int autofilter;

        /// Algorithm for encoding the alpha plane (0 = none, 1 = compressed with WebP lossless).
        public int alpha_compression;

        /// Predictive filtering method for alpha plane. 0: none, 1: fast, 2: best.
        public int alpha_filtering;

        /// Between 0 (smallest size) and 100 (lossless). Default is 100.
        public int alpha_quality;

        /// number of entropy-analysis passes (in [1..10]).
        public int pass;

        /// if true, export the compressed picture back.
        public int show_compressed;

        /// preprocessing filter: 0=none, 1=segment-smooth, 2=pseudo-random dithering
        public int preprocessing;

        /// log2(number of token partitions) in [0..3].
        public int partitions;

        /// quality degradation allowed to fit the 512k limit on prediction modes coding.
        public int partition_limit;

        /// If true, compression parameters will be remapped to better match JPEG output size.
        public int emulate_jpeg_size;

        /// If non-zero, try and use multi-threaded encoding.
        public int thread_level;

        /// If set, reduce memory usage (but increase CPU use).
        public int low_memory;

        /// Near lossless encoding [0 = max loss .. 100 = off (default)].
        public int near_lossless;

        /// if non-zero, preserve the exact RGB values under transparent area.
        public int exact;

        /// reserved for future lossless feature
        public int use_delta_palette;

        /// if needed, use sharp (and slow) RGB->YUV conversion
        public int use_sharp_yuv;

        /// minimum permissible quality factor
        public int qmin;

        /// maximum permissible quality factor
        public int qmax;
    }

    public enum WebPPreset
    {
        /// WEBP_PRESET_DEFAULT -> 0
        WEBP_PRESET_DEFAULT = 0,
        /// digital picture, like portrait, inner shot
        WEBP_PRESET_PICTURE,
        /// outdoor photograph, with natural lighting
        WEBP_PRESET_PHOTO,
        /// hand or line drawing, with high-contrast details
        WEBP_PRESET_DRAWING,
        /// small-sized colorful images
        WEBP_PRESET_ICON,
        /// text-like
        WEBP_PRESET_TEXT,
    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct WebPAuxStats
    {
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

        /// uint32_t
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

        /// lossless header (transform, huffman etc) size
        public int lossless_hdr_size;

        /// lossless image data size
        public int lossless_data_size;

        /// precision bits for cross-color transform
        public int cross_color_transform_bits;

        /// uint32_t[1]
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 1, ArraySubType = UnmanagedType.U4)]
        public uint[] pad;
    }

    /// Return Type: int
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int WebPWriterFunction([InAttribute()] IntPtr data, UIntPtr data_size, ref WebPPicture picture);

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct WebPMemoryWriter
    {
        /// uint8_t*
        public IntPtr mem;
        /// size_t
        public UIntPtr size;
        /// size_t
        public UIntPtr max_size;
        /// uint32_t[1]
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 1, ArraySubType = UnmanagedType.U4)]
        public uint[] pad;
    }

    /// Return Type: int
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int WebPProgressHook(int percent, ref WebPPicture picture);

    public enum WebPEncCSP
    {
        /// 4:2:0 (half-res chroma x and y)
        WEBP_YUV420 = 0,
        /// bit-mask to get the UV sampling factors
        WEBP_CSP_UV_MASK = 3,
        /// 4:2:0 with alpha
        WEBP_YUV420A = 4,
        /// Bit mask to set alpha
        WEBP_CSP_ALPHA_BIT = 4,
    }

    public enum WebPEncodingError
    {
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
    public struct WebPPicture
    {
        //   INPUT
        public int use_argb;

        /// colorspace: should be YUV420 for now
        public WebPEncCSP colorspace;

        /// int
        public int width;
        /// int
        public int height;

        /// uint8_t* pointers to luma/chroma planes.
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

        // OUTPUT

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

        /// WebPEncodingError
        public WebPEncodingError error_code;

        /// WebPProgressHook
        public WebPProgressHook progress_hook;
        /// void*
        public IntPtr user_data;

        /// uint32_t[3]
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.U4)]
        public uint[] pad3;

        /// uint8_t*
        public IntPtr pad4;
        /// uint8_t*
        public IntPtr pad5;

        /// uint32_t[8]
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = UnmanagedType.U4)]
        public uint[] pad6;

        /// void*
        public IntPtr memory_;
        /// void*
        public IntPtr memory_argb_;

        /// void*[2]
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.SysUInt)]
        public IntPtr[] pad7;
    }


    public partial class NativeMethods
    {
        /// Return Type: int
        [DllImportAttribute("libwebp", EntryPoint = "WebPGetEncoderVersion", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WebPGetEncoderVersion();

        /// Lossy encode RGB
        [DllImportAttribute("libwebp", EntryPoint = "WebPEncodeRGB", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr WebPEncodeRGB([InAttribute()] IntPtr rgb, int width, int height, int stride, float quality_factor, ref IntPtr output);

        /// Lossy encode BGR
        [DllImportAttribute("libwebp", EntryPoint = "WebPEncodeBGR", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr WebPEncodeBGR([InAttribute()] IntPtr bgr, int width, int height, int stride, float quality_factor, ref IntPtr output);

        /// Lossy encode RGBA
        [DllImportAttribute("libwebp", EntryPoint = "WebPEncodeRGBA", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr WebPEncodeRGBA([InAttribute()] IntPtr rgba, int width, int height, int stride, float quality_factor, ref IntPtr output);

        /// Lossy encode BGRA
        [DllImportAttribute("libwebp", EntryPoint = "WebPEncodeBGRA", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr WebPEncodeBGRA([InAttribute()] IntPtr bgra, int width, int height, int stride, float quality_factor, ref IntPtr output);

        /// Lossless encode RGB
        [DllImportAttribute("libwebp", EntryPoint = "WebPEncodeLosslessRGB", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr WebPEncodeLosslessRGB([InAttribute()] IntPtr rgb, int width, int height, int stride, ref IntPtr output);

        /// Lossless encode BGR
        [DllImportAttribute("libwebp", EntryPoint = "WebPEncodeLosslessBGR", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr WebPEncodeLosslessBGR([InAttribute()] IntPtr bgr, int width, int height, int stride, ref IntPtr output);

        /// Lossless encode RGBA
        [DllImportAttribute("libwebp", EntryPoint = "WebPEncodeLosslessRGBA", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr WebPEncodeLosslessRGBA([InAttribute()] IntPtr rgba, int width, int height, int stride, ref IntPtr output);

        /// Lossless encode BGRA
        [DllImportAttribute("libwebp", EntryPoint = "WebPEncodeLosslessBGRA", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr WebPEncodeLosslessBGRA([InAttribute()] IntPtr bgra, int width, int height, int stride, ref IntPtr output);

        /// WebPConfigInitInternal
        [DllImportAttribute("libwebp", EntryPoint = "WebPConfigInitInternal", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WebPConfigInitInternal(ref WebPConfig param0, WebPPreset param1, float param2, int param3);

        /// WebPConfigLosslessPreset — configure config for lossless with given level (0..9)
        [DllImportAttribute("libwebp", EntryPoint = "WebPConfigLosslessPreset", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WebPConfigLosslessPreset(ref WebPConfig config, int level);

        /// WebPValidateConfig
        [DllImportAttribute("libwebp", EntryPoint = "WebPValidateConfig", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WebPValidateConfig(ref WebPConfig config);

        /// WebPMemoryWriterInit
        [DllImportAttribute("libwebp", EntryPoint = "WebPMemoryWriterInit", CallingConvention = CallingConvention.Cdecl)]
        public static extern void WebPMemoryWriterInit(ref WebPMemoryWriter writer);

        /// WebPMemoryWriterClear
        [DllImportAttribute("libwebp", EntryPoint = "WebPMemoryWriterClear", CallingConvention = CallingConvention.Cdecl)]
        public static extern void WebPMemoryWriterClear(ref WebPMemoryWriter writer);

        /// WebPMemoryWrite
        [DllImportAttribute("libwebp", EntryPoint = "WebPMemoryWrite", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WebPMemoryWrite([InAttribute()] IntPtr data, UIntPtr data_size, ref WebPPicture picture);

        /// WebPPictureInitInternal
        [DllImportAttribute("libwebp", EntryPoint = "WebPPictureInitInternal", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WebPPictureInitInternal(ref WebPPicture param0, int param1);

        /// WebPPictureAlloc
        [DllImportAttribute("libwebp", EntryPoint = "WebPPictureAlloc", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WebPPictureAlloc(ref WebPPicture picture);

        /// WebPPictureFree
        [DllImportAttribute("libwebp", EntryPoint = "WebPPictureFree", CallingConvention = CallingConvention.Cdecl)]
        public static extern void WebPPictureFree(ref WebPPicture picture);

        /// WebPPictureCopy
        [DllImportAttribute("libwebp", EntryPoint = "WebPPictureCopy", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WebPPictureCopy(ref WebPPicture src, ref WebPPicture dst);

        /// WebPPictureDistortion
        [DllImportAttribute("libwebp", EntryPoint = "WebPPictureDistortion", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WebPPictureDistortion(ref WebPPicture src, ref WebPPicture reference, int metric_type, ref float result);

        /// WebPPictureCrop
        [DllImportAttribute("libwebp", EntryPoint = "WebPPictureCrop", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WebPPictureCrop(ref WebPPicture picture, int left, int top, int width, int height);

        /// WebPPictureView
        [DllImportAttribute("libwebp", EntryPoint = "WebPPictureView", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WebPPictureView(ref WebPPicture src, int left, int top, int width, int height, ref WebPPicture dst);

        /// WebPPictureIsView
        [DllImportAttribute("libwebp", EntryPoint = "WebPPictureIsView", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WebPPictureIsView(ref WebPPicture picture);

        /// WebPPictureRescale
        [DllImportAttribute("libwebp", EntryPoint = "WebPPictureRescale", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WebPPictureRescale(ref WebPPicture pic, int width, int height);

        /// WebPPictureImportRGB
        [DllImportAttribute("libwebp", EntryPoint = "WebPPictureImportRGB", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WebPPictureImportRGB(ref WebPPicture picture, [InAttribute()] IntPtr rgb, int rgb_stride);

        /// WebPPictureImportRGBA
        [DllImportAttribute("libwebp", EntryPoint = "WebPPictureImportRGBA", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WebPPictureImportRGBA(ref WebPPicture picture, [InAttribute()] IntPtr rgba, int rgba_stride);

        /// WebPPictureSmartARGBToYUVA
        [DllImportAttribute("libwebp", EntryPoint = "WebPPictureSmartARGBToYUVA", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WebPPictureSmartARGBToYUVA(ref WebPPicture picture);

        /// WebPPictureImportRGBX
        [DllImportAttribute("libwebp", EntryPoint = "WebPPictureImportRGBX", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WebPPictureImportRGBX(ref WebPPicture picture, [InAttribute()] IntPtr rgbx, int rgbx_stride);

        /// WebPPictureImportBGR
        [DllImportAttribute("libwebp", EntryPoint = "WebPPictureImportBGR", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WebPPictureImportBGR(ref WebPPicture picture, [InAttribute()] IntPtr bgr, int bgr_stride);

        /// WebPPictureImportBGRA
        [DllImportAttribute("libwebp", EntryPoint = "WebPPictureImportBGRA", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WebPPictureImportBGRA(ref WebPPicture picture, [InAttribute()] IntPtr bgra, int bgra_stride);

        /// WebPPictureImportBGRX
        [DllImportAttribute("libwebp", EntryPoint = "WebPPictureImportBGRX", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WebPPictureImportBGRX(ref WebPPicture picture, [InAttribute()] IntPtr bgrx, int bgrx_stride);

        /// WebPPictureARGBToYUVA
        [DllImportAttribute("libwebp", EntryPoint = "WebPPictureARGBToYUVA", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WebPPictureARGBToYUVA(ref WebPPicture picture, WebPEncCSP colorspace);

        /// WebPPictureYUVAToARGB
        [DllImportAttribute("libwebp", EntryPoint = "WebPPictureYUVAToARGB", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WebPPictureYUVAToARGB(ref WebPPicture picture);

        /// WebPCleanupTransparentArea
        [DllImportAttribute("libwebp", EntryPoint = "WebPCleanupTransparentArea", CallingConvention = CallingConvention.Cdecl)]
        public static extern void WebPCleanupTransparentArea(ref WebPPicture picture);

        /// WebPPictureHasTransparency
        [DllImportAttribute("libwebp", EntryPoint = "WebPPictureHasTransparency", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WebPPictureHasTransparency(ref WebPPicture picture);

        /// WebPPictureARGBToYUVADithered
        [DllImportAttribute("libwebp", EntryPoint = "WebPPictureARGBToYUVADithered", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WebPPictureARGBToYUVADithered(ref WebPPicture picture, WebPEncCSP colorspace, float dithering);

        /// WebPPictureSharpARGBToYUVA
        [DllImportAttribute("libwebp", EntryPoint = "WebPPictureSharpARGBToYUVA", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WebPPictureSharpARGBToYUVA(ref WebPPicture picture);

        /// WebPPlaneDistortion
        [DllImportAttribute("libwebp", EntryPoint = "WebPPlaneDistortion", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WebPPlaneDistortion([InAttribute()] IntPtr src, UIntPtr src_stride, [InAttribute()] IntPtr reference, UIntPtr ref_stride, int width, int height, UIntPtr x_step, int type, ref float distortion, ref float result);

        /// WebPBlendAlpha
        [DllImportAttribute("libwebp", EntryPoint = "WebPBlendAlpha", CallingConvention = CallingConvention.Cdecl)]
        public static extern void WebPBlendAlpha(ref WebPPicture picture, uint background_rgb);

        /// WebPEncode
        [DllImportAttribute("libwebp", EntryPoint = "WebPEncode", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WebPEncode(ref WebPConfig config, ref WebPPicture picture);
    }
}

#pragma warning restore 1591
