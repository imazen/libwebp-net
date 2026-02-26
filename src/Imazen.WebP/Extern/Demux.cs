using System;
using System.Runtime.InteropServices;

#pragma warning disable 1591

namespace Imazen.WebP.Extern
{
    /// <summary>
    /// Options for the animation decoder.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WebPAnimDecoderOptions
    {
        /// <summary>Output colorspace. Only BGRA, RGBA, and related modes are supported.</summary>
        public WEBP_CSP_MODE color_mode;
        /// <summary>If true, use multi-threaded decoding.</summary>
        public int use_threads;
        /// <summary>Padding for later use.</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7, ArraySubType = UnmanagedType.U4)]
        public uint[] padding;
    }

    /// <summary>
    /// Information about an animated WebP image.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WebPAnimInfo
    {
        /// <summary>Canvas width.</summary>
        public uint canvas_width;
        /// <summary>Canvas height.</summary>
        public uint canvas_height;
        /// <summary>Number of loop iterations (0 = infinite).</summary>
        public uint loop_count;
        /// <summary>Background color (BGRA packed in uint32).</summary>
        public uint bgcolor;
        /// <summary>Total number of frames.</summary>
        public uint frame_count;
        /// <summary>Padding for later use.</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
        public uint[] pad;
    }

    public partial class NativeMethods
    {
        /// <summary>WEBP_DEMUX_ABI_VERSION for libwebp 1.6.0</summary>
        public const int WEBP_DEMUX_ABI_VERSION = 0x0107;

        // --- WebPAnimDecoder ---

        /// <summary>Initialize animation decoder options to defaults.</summary>
        [DllImport("libwebpdemux", EntryPoint = "WebPAnimDecoderOptionsInitInternal", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WebPAnimDecoderOptionsInitInternal(ref WebPAnimDecoderOptions dec_options, int abi_version);

        /// <summary>Create a new animation decoder (internal, versioned).</summary>
        [DllImport("libwebpdemux", EntryPoint = "WebPAnimDecoderNewInternal", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WebPAnimDecoderNewInternal(ref WebPData webp_data, ref WebPAnimDecoderOptions dec_options, int abi_version);

        /// <summary>Create a new animation decoder with default options (internal, versioned).</summary>
        [DllImport("libwebpdemux", EntryPoint = "WebPAnimDecoderNewInternal", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WebPAnimDecoderNewInternalDefault(ref WebPData webp_data, IntPtr dec_options, int abi_version);

        /// <summary>Get animation info (frame count, dimensions, loop count, etc.).</summary>
        [DllImport("libwebpdemux", EntryPoint = "WebPAnimDecoderGetInfo", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WebPAnimDecoderGetInfo(IntPtr dec, ref WebPAnimInfo info);

        /// <summary>
        /// Get the next frame. buf receives a pointer to the decoded RGBA/BGRA data
        /// (owned by the decoder, valid until next call). timestamp receives the frame timestamp in ms.
        /// </summary>
        [DllImport("libwebpdemux", EntryPoint = "WebPAnimDecoderGetNext", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WebPAnimDecoderGetNext(IntPtr dec, ref IntPtr buf, ref int timestamp);

        /// <summary>Check if there are more frames to decode.</summary>
        [DllImport("libwebpdemux", EntryPoint = "WebPAnimDecoderHasMoreFrames", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WebPAnimDecoderHasMoreFrames(IntPtr dec);

        /// <summary>Reset the decoder to the first frame.</summary>
        [DllImport("libwebpdemux", EntryPoint = "WebPAnimDecoderReset", CallingConvention = CallingConvention.Cdecl)]
        public static extern void WebPAnimDecoderReset(IntPtr dec);

        /// <summary>Delete the animation decoder and free resources.</summary>
        [DllImport("libwebpdemux", EntryPoint = "WebPAnimDecoderDelete", CallingConvention = CallingConvention.Cdecl)]
        public static extern void WebPAnimDecoderDelete(IntPtr dec);

        // --- Inline wrappers ---

        /// <summary>Initialize animation decoder options (inline wrapper).</summary>
        public static int WebPAnimDecoderOptionsInit(ref WebPAnimDecoderOptions dec_options)
        {
            return WebPAnimDecoderOptionsInitInternal(ref dec_options, WEBP_DEMUX_ABI_VERSION);
        }

        /// <summary>Create a new animation decoder (inline wrapper with options).</summary>
        public static IntPtr WebPAnimDecoderNew(ref WebPData webp_data, ref WebPAnimDecoderOptions dec_options)
        {
            return WebPAnimDecoderNewInternal(ref webp_data, ref dec_options, WEBP_DEMUX_ABI_VERSION);
        }

        /// <summary>Create a new animation decoder with default options (inline wrapper).</summary>
        public static IntPtr WebPAnimDecoderNewDefault(ref WebPData webp_data)
        {
            return WebPAnimDecoderNewInternalDefault(ref webp_data, IntPtr.Zero, WEBP_DEMUX_ABI_VERSION);
        }
    }
}

#pragma warning restore 1591
