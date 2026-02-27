using System.Runtime.InteropServices;
using Xunit;

namespace Imazen.Test.Webp
{
    /// <summary>
    /// <see cref="FactAttribute"/> that only runs on Windows.
    /// Skipped on Linux/macOS where System.Drawing (GDI+) is unavailable.
    /// </summary>
    public sealed class WindowsFactAttribute : FactAttribute
    {
        public WindowsFactAttribute()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                Skip = "Requires Windows (System.Drawing/GDI+)";
        }
    }

    /// <summary>
    /// <see cref="TheoryAttribute"/> that only runs on Windows.
    /// Skipped on Linux/macOS where System.Drawing (GDI+) is unavailable.
    /// </summary>
    public sealed class WindowsTheoryAttribute : TheoryAttribute
    {
        public WindowsTheoryAttribute()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                Skip = "Requires Windows (System.Drawing/GDI+)";
        }
    }
}
