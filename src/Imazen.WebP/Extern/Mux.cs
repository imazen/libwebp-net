using System;
using System.Runtime.InteropServices;

#pragma warning disable 1591

namespace Imazen.WebP.Extern
{
    /// <summary>
    /// Animation parameters for the mux container.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WebPMuxAnimParams
    {
        /// <summary>Background color of the canvas (BGRA packed in uint32).</summary>
        public uint bgcolor;
        /// <summary>Number of times to loop the animation. 0 = infinite.</summary>
        public int loop_count;
    }

    /// <summary>
    /// Options for the animation encoder.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WebPAnimEncoderOptions
    {
        /// <summary>Animation parameters (background color, loop count).</summary>
        public WebPMuxAnimParams anim_params;
        /// <summary>If true, minimize output size (slower).</summary>
        public int minimize_size;
        /// <summary>Minimum keyframe interval (0 = default).</summary>
        public int kmin;
        /// <summary>Maximum keyframe interval (0 = default).</summary>
        public int kmax;
        /// <summary>If true, allow mixing lossy and lossless frames.</summary>
        public int allow_mixed;
        /// <summary>If true, print info and warnings to stderr.</summary>
        public int verbose;
        /// <summary>Padding for later use.</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
        public uint[] padding;
    }

    public partial class NativeMethods
    {
        /// <summary>WEBP_MUX_ABI_VERSION for libwebp 1.6.0</summary>
        public const int WEBP_MUX_ABI_VERSION = 0x0109;

        // --- WebPAnimEncoder ---

        /// <summary>Initialize animation encoder options to defaults.</summary>
        [DllImport("libwebpmux", EntryPoint = "WebPAnimEncoderOptionsInitInternal", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WebPAnimEncoderOptionsInitInternal(ref WebPAnimEncoderOptions enc_options, int abi_version);

        /// <summary>Create a new animation encoder (internal, versioned).</summary>
        [DllImport("libwebpmux", EntryPoint = "WebPAnimEncoderNewInternal", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WebPAnimEncoderNewInternal(int width, int height, ref WebPAnimEncoderOptions enc_options, int abi_version);

        /// <summary>Create a new animation encoder with default options (internal, versioned).</summary>
        [DllImport("libwebpmux", EntryPoint = "WebPAnimEncoderNewInternal", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WebPAnimEncoderNewInternalDefault(int width, int height, IntPtr enc_options, int abi_version);

        /// <summary>Add a frame to the animation. Frame and config are by ref.</summary>
        [DllImport("libwebpmux", EntryPoint = "WebPAnimEncoderAdd", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WebPAnimEncoderAdd(IntPtr enc, ref WebPPicture frame, int timestamp_ms, ref WebPConfig config);

        /// <summary>Add a frame with default config (config = NULL).</summary>
        [DllImport("libwebpmux", EntryPoint = "WebPAnimEncoderAdd", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WebPAnimEncoderAddDefaultConfig(IntPtr enc, ref WebPPicture frame, int timestamp_ms, IntPtr config);

        /// <summary>Signal end of frames (frame = NULL, config = NULL).</summary>
        [DllImport("libwebpmux", EntryPoint = "WebPAnimEncoderAdd", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WebPAnimEncoderAddNull(IntPtr enc, IntPtr frame, int timestamp_ms, IntPtr config);

        /// <summary>Assemble the animation into final WebP data.</summary>
        [DllImport("libwebpmux", EntryPoint = "WebPAnimEncoderAssemble", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WebPAnimEncoderAssemble(IntPtr enc, ref WebPData webp_data);

        /// <summary>Get the error string from the encoder.</summary>
        [DllImport("libwebpmux", EntryPoint = "WebPAnimEncoderGetError", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WebPAnimEncoderGetError(IntPtr enc);

        /// <summary>Delete the animation encoder and free resources.</summary>
        [DllImport("libwebpmux", EntryPoint = "WebPAnimEncoderDelete", CallingConvention = CallingConvention.Cdecl)]
        public static extern void WebPAnimEncoderDelete(IntPtr enc);

        // --- Inline wrappers ---

        /// <summary>Initialize animation encoder options (inline wrapper).</summary>
        public static int WebPAnimEncoderOptionsInit(ref WebPAnimEncoderOptions enc_options)
        {
            return WebPAnimEncoderOptionsInitInternal(ref enc_options, WEBP_MUX_ABI_VERSION);
        }

        /// <summary>Create a new animation encoder (inline wrapper).</summary>
        public static IntPtr WebPAnimEncoderNew(int width, int height, ref WebPAnimEncoderOptions enc_options)
        {
            return WebPAnimEncoderNewInternal(width, height, ref enc_options, WEBP_MUX_ABI_VERSION);
        }

        /// <summary>Create a new animation encoder with default options (inline wrapper).</summary>
        public static IntPtr WebPAnimEncoderNewDefault(int width, int height)
        {
            return WebPAnimEncoderNewInternalDefault(width, height, IntPtr.Zero, WEBP_MUX_ABI_VERSION);
        }
    }
}

#pragma warning restore 1591
