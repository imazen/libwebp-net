using System.Runtime.InteropServices;

// Restrict the default DLL search to the assembly directory and Windows safe
// directories. This prevents the legacy fallback to the current working
// directory (and other unsafe paths) when the runtime resolves libwebp via
// DllImport before our DllImportResolver / FixDllNotFoundException logic runs.
//
// AssemblyDirectory: equivalent to LOAD_LIBRARY_SEARCH_DLL_LOAD_DIR for the
// loading assembly.
// SafeDirectories:   equivalent to LOAD_LIBRARY_SEARCH_SYSTEM32 plus the
// user-added DLL directories.
//
// Available since .NET Framework 4.0; honored on .NET Core / .NET 5+ as well.
[assembly: DefaultDllImportSearchPaths(
    DllImportSearchPath.AssemblyDirectory | DllImportSearchPath.SafeDirectories)]
