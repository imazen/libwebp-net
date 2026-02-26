using System;
using System.Runtime.InteropServices;

#pragma warning disable 1591

namespace Imazen.WebP.Extern
{
    /// <summary>
    /// Generic data container (from mux_types.h).
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WebPData
    {
        /// <summary>Pointer to data bytes.</summary>
        public IntPtr bytes;
        /// <summary>Size of the data.</summary>
        public UIntPtr size;
    }

    /// <summary>
    /// Dispose method for animation frames.
    /// </summary>
    public enum WebPMuxAnimDispose
    {
        WEBP_MUX_DISPOSE_NONE = 0,
        WEBP_MUX_DISPOSE_BACKGROUND = 1
    }

    /// <summary>
    /// Blend method for animation frames.
    /// </summary>
    public enum WebPMuxAnimBlend
    {
        WEBP_MUX_BLEND = 0,
        WEBP_MUX_NO_BLEND = 1
    }

    /// <summary>
    /// Feature flags for extended format.
    /// </summary>
    [Flags]
    public enum WebPFeatureFlags : uint
    {
        ANIMATION_FLAG = 0x00000002,
        XMP_FLAG = 0x00000004,
        EXIF_FLAG = 0x00000008,
        ALPHA_FLAG = 0x00000010,
        ICCP_FLAG = 0x00000020,
        ALL_VALID_FLAGS = 0x0000003e
    }
}

#pragma warning restore 1591
