using System;
using Imazen.WebP.Extern;

namespace Imazen.WebP
{
    /// <summary>
    /// Validates ABI compatibility between the managed bindings and the loaded native libwebp libraries.
    /// </summary>
    public static class AbiVersionCheck
    {
        private static volatile bool _validated;

        /// <summary>
        /// Validates that the loaded native library is ABI-compatible.
        /// Throws <see cref="NotSupportedException"/> if the major version doesn't match.
        /// </summary>
        public static void ValidateOrThrow()
        {
            if (_validated) return;

            int decoderVer = NativeMethods.WebPGetDecoderVersion();
            int encoderVer = NativeMethods.WebPGetEncoderVersion();

            int decMajor = (decoderVer >> 16) & 0xFF;
            int encMajor = (encoderVer >> 16) & 0xFF;

            if (decMajor != 1 || encMajor != 1)
            {
                throw new NotSupportedException(
                    $"Incompatible libwebp version. Expected 1.x, got decoder={FormatVersion(decoderVer)}, encoder={FormatVersion(encoderVer)}");
            }

            _validated = true;
        }

        /// <summary>
        /// Returns a human-readable version string for the loaded native library.
        /// </summary>
        public static string GetVersionString()
        {
            int decoderVer = NativeLibraryLoader.FixDllNotFoundException("webp",
                () => NativeMethods.WebPGetDecoderVersion());
            int encoderVer = NativeMethods.WebPGetEncoderVersion();
            return $"decoder={FormatVersion(decoderVer)}, encoder={FormatVersion(encoderVer)}";
        }

        private static string FormatVersion(int version)
        {
            int major = (version >> 16) & 0xFF;
            int minor = (version >> 8) & 0xFF;
            int revision = version & 0xFF;
            return $"{major}.{minor}.{revision}";
        }
    }
}
