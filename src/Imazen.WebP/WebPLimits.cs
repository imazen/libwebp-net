using System;

namespace Imazen.WebP
{
    /// <summary>
    /// Process-wide resource limits applied by the managed WebP wrappers
    /// before any data reaches libwebp. Adjust before the first decode if you
    /// accept WebP data from untrusted sources (HTTP uploads, peer-to-peer
    /// transfers, etc.) so a hostile or buggy producer cannot drive the
    /// process out of memory.
    /// </summary>
    public static class WebPLimits
    {
        /// <summary>
        /// Default cap on <c>DecodeFromStream</c>-style APIs that buffer the
        /// entire encoded WebP into memory before handing it to libwebp.
        /// Default: <c>int.MaxValue</c> bytes (effectively unlimited), to
        /// preserve historical behavior. Lower this when accepting WebP data
        /// from untrusted streams. Callers can also override on a per-call
        /// basis via the <c>maxBytes</c> overloads.
        /// </summary>
        public static long MaxDecodeStreamBytes { get; set; } = int.MaxValue;
    }
}
