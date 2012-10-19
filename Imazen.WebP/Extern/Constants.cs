using System;
using System.Collections.Generic;
using System.Text;

namespace Imazen.WebP.Extern {
    public partial class NativeMethods {

        /// WEBP_DECODER_ABI_VERSION -> 0x0200
        public const int WEBP_DECODER_ABI_VERSION = 512;

        /// WEBP_ENCODER_ABI_VERSION -> 0x0200
        public const int WEBP_ENCODER_ABI_VERSION = 512;

        /// <summary>
        /// The maximum length of any dimension of a WebP image is 16383
        /// </summary>
        public const int WEBP_MAX_DIMENSION = 16383;
    }
}
