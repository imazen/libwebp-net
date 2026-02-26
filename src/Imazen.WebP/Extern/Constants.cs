using System;
#pragma warning disable 1591
namespace Imazen.WebP.Extern {
    public partial class NativeMethods {

        /// WEBP_DECODER_ABI_VERSION 0x0210    // MAJOR(8b) + MINOR(8b) — libwebp 1.6.0
        public const int WEBP_DECODER_ABI_VERSION = 0x0210;

        /// WEBP_ENCODER_ABI_VERSION 0x0210    // MAJOR(8b) + MINOR(8b) — libwebp 1.6.0
        public const int WEBP_ENCODER_ABI_VERSION = 0x0210;

        /// <summary>
        /// The maximum length of any dimension of a WebP image is 16383
        /// </summary>
        public const int WEBP_MAX_DIMENSION = 16383;
    }
}
#pragma warning restore 1591
