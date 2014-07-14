using System;
using System.Collections.Generic;
using System.Text;

namespace Imazen.WebP.Extern {
    public partial class NativeMethods {

        /// WEBP_DECODER_ABI_VERSION 0x0203    // MAJOR(8b) + MINOR(8b)
        public const int WEBP_DECODER_ABI_VERSION = 515;

        /// WEBP_ENCODER_ABI_VERSION 0x0202    // MAJOR(8b) + MINOR(8b)
        public const int WEBP_ENCODER_ABI_VERSION = 514;

        /// <summary>
        /// The maximum length of any dimension of a WebP image is 16383
        /// </summary>
        public const int WEBP_MAX_DIMENSION = 16383;
    }
}
