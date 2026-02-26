using System;

namespace Imazen.WebP.Extern
{
    /// <summary>
    /// Legacy compatibility shim. The new NativeLibraryLoading handles all library resolution.
    /// This class is preserved so existing user code calling LoadLibrary.LoadWebPOrFail() still compiles.
    /// </summary>
    [Obsolete("Use NativeLibraryLoader.FixDllNotFoundException instead. Library loading is now automatic.")]
    public static class LoadLibrary
    {
        /// <summary>
        /// No-op. Library loading is now handled automatically by NativeLibraryLoader on first P/Invoke call.
        /// Kept for backward compatibility.
        /// </summary>
        public static void LoadWebPOrFail()
        {
            // Trigger a version check which will force library load via FixDllNotFoundException
            NativeLibraryLoader.FixDllNotFoundException("webp", () => NativeMethods.WebPGetDecoderVersion());
        }

        /// <summary>
        /// No-op. Library loading is now handled automatically.
        /// </summary>
        [Obsolete]
        public static bool AutoLoadNearby(string name, bool throwFailure)
        {
            LoadWebPOrFail();
            return true;
        }
    }
}
