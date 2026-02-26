using System;
using System.Runtime.InteropServices;

namespace Imazen.WebP.Extern {
    public partial class NativeMethods {

        public static void WebPSafeFree(IntPtr toDeallocate)
        {
            NativeLibraryLoader.FixDllNotFoundException("webp", () => { WebPFree(toDeallocate); return 0; });
        }

        [DllImportAttribute("libwebp", EntryPoint = "WebPFree", CallingConvention = CallingConvention.Cdecl)]
        public static extern void WebPFree(IntPtr toDeallocate);

        [DllImportAttribute("libwebp", EntryPoint = "WebPMalloc", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WebPMalloc(UIntPtr size);
    }
}
