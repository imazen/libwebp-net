using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Reflection;
#if NETCOREAPP
using System.Runtime.CompilerServices;
#endif

namespace Imazen.WebP
{
    internal class LoadLogger : ILibraryLoadLogger
    {
        internal string Verb = "loaded";
        internal string Filename = $"{RuntimeFileLocator.SharedLibraryPrefix.Value}webp.{RuntimeFileLocator.SharedLibraryExtension.Value}";

        internal Exception? FirstException;
        internal Exception? LastException;

        private readonly List<LogEntry> _log = new List<LogEntry>(7);

        private struct LogEntry
        {
            internal string Basename;
            internal string? FullPath;
            internal bool FileExists;
            internal bool PreviouslyLoaded;
            internal int? LoadErrorCode;
        }

        public void NotifyAttempt(string basename, string? fullPath, bool fileExists, bool previouslyLoaded,
            int? loadErrorCode)
        {
            _log.Add(new LogEntry
            {
                Basename = basename,
                FullPath = fullPath,
                FileExists = fileExists,
                PreviouslyLoaded = previouslyLoaded,
                LoadErrorCode = loadErrorCode
            });
        }

        internal void RaiseException()
        {
            var sb = new StringBuilder();
            sb.AppendFormat(CultureInfo.InvariantCulture,
                "Looking for \"{0}\" RID=\"{1}-{2}\", IsUnix={3}, IsDotNetCore={4} RelativeSearchPath=\"{5}\"\n",
                Filename,
                RuntimeFileLocator.PlatformRuntimePrefix.Value,
                RuntimeFileLocator.ArchitectureSubdir.Value, RuntimeFileLocator.IsUnix,
                RuntimeFileLocator.IsDotNetCore.Value,
                AppDomain.CurrentDomain.RelativeSearchPath);
            if (FirstException != null)
            {
                sb.AppendFormat(CultureInfo.InvariantCulture, "Before searching: {0}\n", FirstException.Message);
            }

            foreach (var e in _log)
            {
                if (e.PreviouslyLoaded)
                {
                    sb.AppendFormat(CultureInfo.InvariantCulture, "\"{0}\" is already {1}", e.Basename, Verb);
                }
                else if (!e.FileExists)
                {
                    sb.AppendFormat(CultureInfo.InvariantCulture, "File not found: {0}", e.FullPath);
                }
                else if (e.LoadErrorCode.HasValue)
                {
                    string errorCode = e.LoadErrorCode.Value < 0
                        ? string.Format(CultureInfo.InvariantCulture, "0x{0:X8}", e.LoadErrorCode.Value)
                        : e.LoadErrorCode.Value.ToString(CultureInfo.InvariantCulture);

                    sb.AppendFormat(CultureInfo.InvariantCulture, "Error \"{0}\" ({1}) loading {2} from {3}",
                        new Win32Exception(e.LoadErrorCode.Value).Message,
                        errorCode,
                        e.Basename, e.FullPath);

                    if (e.LoadErrorCode.Value == 193 &&
                        RuntimeFileLocator.PlatformRuntimePrefix.Value == "win")
                    {
                        var installed = Environment.Is64BitProcess ? "32-bit (x86)" : "64-bit (x86_64)";
                        var needed = Environment.Is64BitProcess ? "64-bit (x86_64)" : "32-bit (x86)";

                        sb.AppendFormat(CultureInfo.InvariantCulture,
                            "\n> You have installed a {0} copy of libwebp but need the {1} version",
                            installed, needed);
                    }

                    if (e.LoadErrorCode.Value == 126 &&
                        RuntimeFileLocator.PlatformRuntimePrefix.Value == "win")
                    {
                        var crtLink = "https://aka.ms/vs/17/release/vc_redist."
                                      + (Environment.Is64BitProcess ? "x64.exe" : "x86.exe");

                        sb.AppendFormat(CultureInfo.InvariantCulture, "\n> You may need to install the C Runtime from {0}",
                            crtLink);
                    }
                }
                else
                {
                    sb.AppendFormat(CultureInfo.InvariantCulture, "{0} {1} in {2}", Verb, e.Basename, e.FullPath);
                }

                sb.Append('\n');
            }

            if (LastException != null)
            {
                sb.AppendLine(LastException.Message);
            }

            var stackTrace = (FirstException ?? LastException)?.StackTrace;
            if (stackTrace != null)
            {
                sb.AppendLine(stackTrace);
            }

            throw new DllNotFoundException(sb.ToString());
        }
    }

    internal static class RuntimeFileLocator
    {
        internal static bool IsUnix => RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
#if NETCOREAPP
                                       || RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD)
#endif
                                       || RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        internal static readonly Lazy<string> SharedLibraryPrefix =
                new Lazy<string>(() => IsUnix ? "lib" : "", LazyThreadSafetyMode.PublicationOnly);

#if NETCOREAPP
        internal static readonly Lazy<bool> IsDotNetCore = new Lazy<bool>(() =>
                true
            , LazyThreadSafetyMode.PublicationOnly);
#else
        internal static readonly Lazy<bool> IsDotNetCore = new Lazy<bool>(() =>
        {
            try
            {
                return typeof(System.Runtime.GCSettings).GetTypeInfo().Assembly.CodeBase.Contains("Microsoft.NETCore.App");
            }
            catch
            {
                return false;
            }
        }, LazyThreadSafetyMode.PublicationOnly);
#endif

        internal static readonly Lazy<string> PlatformRuntimePrefix = new Lazy<string>(() =>
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return "osx";
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return "linux";
            }
#if NETCOREAPP
            if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
            {
                return "freebsd";
            }
#endif
            return "win";
        }, LazyThreadSafetyMode.PublicationOnly);

        internal static readonly Lazy<string> SharedLibraryExtension = new Lazy<string>(() =>
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return "dylib";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return "so";
            }
#if NETCOREAPP
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
            {
                return "so";
            }
#endif
            else
            {
                return "dll";
            }
        }, LazyThreadSafetyMode.PublicationOnly);

        internal static readonly Lazy<string> ArchitectureSubdir = new Lazy<string>(() =>
        {
            var processArchitecture = RuntimeInformation.ProcessArchitecture;
            switch (processArchitecture)
            {
                case Architecture.X86: return "x86";
                case Architecture.X64: return "x64";
                case Architecture.Arm: return "arm";
                case Architecture.Arm64: return "arm64";
            }

            // Fallback to environment variable
            var envArchitecture = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
            if (envArchitecture != null)
            {
                switch (envArchitecture.ToUpperInvariant())
                {
                    case "AMD64": return "x64";
                    case "IA64": return "ia64";
                    case "ARM64": return "arm64";
                    case "EM64T": return "x64";
                    case "X86": return "x86";
                }
            }

            return Environment.Is64BitProcess ? "x64" : "x86";
        }, LazyThreadSafetyMode.PublicationOnly);

        private static IEnumerable<Tuple<bool, string>> BaseFolders(IEnumerable<string>? customSearchDirectories = null)
        {
            // Prioritize user suggestions
            if (customSearchDirectories != null)
            {
                foreach (var d in customSearchDirectories)
                {
                    yield return Tuple.Create(true, d);
                }
            }

            // First look in AppDomain.CurrentDomain.RelativeSearchPath
            if (!string.IsNullOrEmpty(AppDomain.CurrentDomain.RelativeSearchPath) &&
                AppDomain.CurrentDomain.RelativeSearchPath!.StartsWith(AppDomain.CurrentDomain.BaseDirectory))
            {
                yield return Tuple.Create(true, AppDomain.CurrentDomain.RelativeSearchPath!);
            }

            // System.AppContext.BaseDirectory
            if (!string.IsNullOrEmpty(AppContext.BaseDirectory))
            {
                yield return Tuple.Create(true, AppContext.BaseDirectory);
            }

            // AppDomain base directory
            yield return Tuple.Create(true, AppDomain.CurrentDomain.BaseDirectory);

            // Azure Functions workaround: if BaseDirectory is /bin/, look one step outside
            if (AppDomain.CurrentDomain.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                .EndsWith("bin"))
            {
                var dir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory);
                if (dir != null)
                {
                    yield return Tuple.Create(false, Path.Combine(dir.FullName,
                        "runtimes", PlatformRuntimePrefix.Value + "-" + ArchitectureSubdir.Value, "native"));
                }
            }

#if !NETCOREAPP
            string? assemblyLocation = null;
            try
            {
                assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            }
            catch (NotImplementedException)
            {
                // ignored
            }
            if (!string.IsNullOrEmpty(assemblyLocation))
            {
                yield return Tuple.Create(true, assemblyLocation!);
            }
#endif
        }

        internal static IEnumerable<string> SearchPossibilitiesForFile(string filename,
            IEnumerable<string>? customSearchDirectories = null)
        {
            var attemptedPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var t in BaseFolders(customSearchDirectories))
            {
                if (string.IsNullOrEmpty(t.Item2))
                {
                    continue;
                }

                var directory = Path.GetFullPath(t.Item2);
                var searchSubDirs = t.Item1;
                string path;

                if (searchSubDirs)
                {
                    // runtimes/[RID]/native/ — standard .NET Core packaging location
                    path = Path.Combine(directory, "runtimes",
                        PlatformRuntimePrefix.Value + "-" + ArchitectureSubdir.Value, "native", filename);
                    if (attemptedPaths.Add(path))
                    {
                        yield return path;
                    }
                }

                if (searchSubDirs)
                {
                    // Legacy arch subdir (x86/, x64/)
                    path = Path.Combine(directory, ArchitectureSubdir.Value, filename);
                    if (attemptedPaths.Add(path))
                    {
                        yield return path;
                    }
                }

                // Folder itself
                path = Path.Combine(directory, filename);
                if (attemptedPaths.Add(path))
                {
                    yield return path;
                }
            }
        }
    }

    internal interface ILibraryLoadLogger
    {
        void NotifyAttempt(string basename, string? fullPath, bool fileExists, bool previouslyLoaded, int? loadErrorCode);
    }

    internal static class NativeLibraryLoader
    {
        internal static string GetFilenameWithoutDirectory(string basename) =>
            $"{RuntimeFileLocator.SharedLibraryPrefix.Value}{basename}.{RuntimeFileLocator.SharedLibraryExtension.Value}";

        /// <summary>
        /// Attempts to resolve DllNotFoundException and BadImageFormatExceptions
        /// by searching known directories and loading the library explicitly.
        /// </summary>
        public static T? FixDllNotFoundException<T>(string basename, Func<T> invokingOperation,
            IEnumerable<string>? customSearchDirectories = null)
        {
            Exception? caughtException;
            try
            {
                return invokingOperation();
            }
            catch (BadImageFormatException a)
            {
                caughtException = a;
            }
            catch (DllNotFoundException b)
            {
                caughtException = b;
            }

            var logger = new LoadLogger
            {
                FirstException = caughtException,
                Filename = GetFilenameWithoutDirectory(basename)
            };
            if (TryLoadByBasename(basename, logger, out _, customSearchDirectories))
            {
                try
                {
                    return invokingOperation();
                }
                catch (DllNotFoundException last)
                {
                    logger.LastException = last;
                }
            }

            logger.RaiseException();
            return default;
        }

        private static readonly Lazy<ConcurrentDictionary<string, IntPtr>> LibraryHandlesByBasename =
            new Lazy<ConcurrentDictionary<string, IntPtr>>(
                () => new ConcurrentDictionary<string, IntPtr>(StringComparer.OrdinalIgnoreCase),
                LazyThreadSafetyMode.PublicationOnly);

        public static bool TryLoadByBasename(string basename, ILibraryLoadLogger log, out IntPtr handle,
            IEnumerable<string>? customSearchDirectories = null)
        {
            if (string.IsNullOrEmpty(basename))
            {
                throw new ArgumentNullException(nameof(basename));
            }

            if (LibraryHandlesByBasename.Value.TryGetValue(basename, out handle))
            {
                log.NotifyAttempt(basename, null, true, true, 0);
                return true;
            }

            lock (LibraryHandlesByBasename)
            {
                if (LibraryHandlesByBasename.Value.TryGetValue(basename, out handle))
                {
                    log.NotifyAttempt(basename, null, true, true, 0);
                    return true;
                }

                var success = TryLoadByBasenameInternal(basename, log, out handle, customSearchDirectories);
                if (success)
                {
                    LibraryHandlesByBasename.Value[basename] = handle;
                    // Validate ABI compatibility once after the main webp library loads
                    if (string.Equals(basename, "webp", StringComparison.OrdinalIgnoreCase))
                    {
                        AbiVersionCheck.ValidateOrThrow();
                    }
                }

                return success;
            }
        }

        private static bool TryLoadByBasenameInternal(string basename, ILibraryLoadLogger log, out IntPtr handle,
            IEnumerable<string>? customSearchDirectories = null)
        {
            // Try the platform-conventional name first, then lib-prefixed (cmake produces lib-prefixed DLLs on Windows)
            var filename = GetFilenameWithoutDirectory(basename);
            var filenames = new List<string> { filename };
            if (!RuntimeFileLocator.IsUnix)
            {
                var libPrefixed = $"lib{basename}.{RuntimeFileLocator.SharedLibraryExtension.Value}";
                if (!string.Equals(filename, libPrefixed, StringComparison.OrdinalIgnoreCase))
                    filenames.Add(libPrefixed);
            }

            foreach (var fn in filenames)
            {
                foreach (var path in RuntimeFileLocator.SearchPossibilitiesForFile(fn, customSearchDirectories))
                {
                    if (!File.Exists(path))
                    {
                        log.NotifyAttempt(basename, path, false, false, 0);
                    }
                    else
                    {
                        var success = LoadLibrary(path, out handle, out var errorCode);
                        log.NotifyAttempt(basename, path, true, false, errorCode);
                        if (success)
                        {
                            return true;
                        }
                    }
                }
            }

            handle = IntPtr.Zero;
            return false;
        }

        private static bool LoadLibrary(string fullPath, out IntPtr handle, out int? errorCode)
        {
#if NETCOREAPP
            if (NativeLibrary.TryLoad(fullPath, out handle))
            {
                errorCode = null;
                return true;
            }
            errorCode = Marshal.GetLastWin32Error();
            return false;
#else
            handle = RuntimeFileLocator.IsUnix ? UnixLoadLibrary.Execute(fullPath) : WindowsLoadLibrary.Execute(fullPath);
            if (handle == IntPtr.Zero)
            {
                errorCode = Marshal.GetLastWin32Error();
                return false;
            }

            errorCode = null;
            return true;
#endif
        }
    }

#if !NETCOREAPP
    [SuppressUnmanagedCodeSecurity]
    [SecurityCritical]
    internal static class WindowsLoadLibrary
    {
        [DllImport("kernel32", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Unicode,
            SetLastError = true)]
        private static extern IntPtr LoadLibraryEx(string fileName, IntPtr reservedNull, uint flags);

        public static IntPtr Execute(string fileName)
        {
            const uint loadWithAlteredSearchPath = 0x00000008;
            return LoadLibraryEx(fileName, IntPtr.Zero, loadWithAlteredSearchPath);
        }
    }

    [SuppressUnmanagedCodeSecurity]
    [SecurityCritical]
    internal static class UnixLoadLibrary
    {
        // Modern Linux (glibc 2.34+ / Ubuntu 24.04+) merged libdl into libc;
        // the standalone libdl.so symlink no longer exists. Use libdl.so.2 on
        // Linux, fall back to libdl (works on macOS and older Linux).
        [DllImport("libdl.so.2", EntryPoint = "dlopen", SetLastError = true, CharSet = CharSet.Ansi)]
        private static extern IntPtr dlopen_libdl2(string fileName, int flags);

        [DllImport("libdl", EntryPoint = "dlopen", SetLastError = true, CharSet = CharSet.Ansi)]
        private static extern IntPtr dlopen_libdl(string fileName, int flags);

        private static volatile bool _preferLibdl2 = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        public static IntPtr Execute(string fileName)
        {
            const int rtldNow = 2;
            if (_preferLibdl2)
            {
                try { return dlopen_libdl2(fileName, rtldNow); }
                catch (DllNotFoundException) { _preferLibdl2 = false; }
            }
            return dlopen_libdl(fileName, rtldNow);
        }
    }
#endif

#if NETCOREAPP
    /// <summary>
    /// Module initializer that registers a DllImportResolver for this assembly.
    /// This ensures that P/Invoke calls for libwebp, libwebpdemux, and libwebpmux
    /// are resolved using our custom search logic (runtimes/{rid}/native/, arch subdirs, etc.)
    /// without requiring callers to go through FixDllNotFoundException.
    /// </summary>
    internal static class NativeLibraryBootstrap
    {
        [ModuleInitializer]
        internal static void Initialize()
        {
            try
            {
                NativeLibrary.SetDllImportResolver(
                    typeof(NativeLibraryBootstrap).Assembly,
                    ResolveDllImport);
            }
            catch (InvalidOperationException)
            {
                // Resolver already registered
            }
        }

        private static IntPtr ResolveDllImport(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
            // Try default resolution first (handles system libraries like kernel32)
            if (NativeLibrary.TryLoad(libraryName, assembly, searchPath, out var handle))
                return handle;

            // Map DllImport library names to our basenames
            string? basename = libraryName switch
            {
                "libwebp" or "webp" => "webp",
                "libwebpdemux" or "webpdemux" => "webpdemux",
                "libwebpmux" or "webpmux" => "webpmux",
                _ => null
            };

            if (basename == null)
                return IntPtr.Zero;

            // Use our custom search logic (runtimes/{rid}/native/, arch subdirs, etc.)
            var filename = NativeLibraryLoader.GetFilenameWithoutDirectory(basename);
            foreach (var path in RuntimeFileLocator.SearchPossibilitiesForFile(filename))
            {
                if (File.Exists(path) && NativeLibrary.TryLoad(path, out handle))
                    return handle;
            }

            // Also try lib-prefixed on Windows (cmake produces lib-prefixed DLLs)
            if (!RuntimeFileLocator.IsUnix)
            {
                var libPrefixed = $"lib{basename}.{RuntimeFileLocator.SharedLibraryExtension.Value}";
                if (!string.Equals(filename, libPrefixed, StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var path in RuntimeFileLocator.SearchPossibilitiesForFile(libPrefixed))
                    {
                        if (File.Exists(path) && NativeLibrary.TryLoad(path, out handle))
                            return handle;
                    }
                }
            }

            return IntPtr.Zero;
        }
    }
#endif
}
