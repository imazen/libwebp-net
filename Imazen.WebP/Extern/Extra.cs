using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Imazen.WebP.Extern {
    public partial class NativeMethods {

        [DllImportAttribute("libwebp.dll", EntryPoint = "WebPFree")]
        public static extern void WebPFree(IntPtr toDeallocate);

    }
}