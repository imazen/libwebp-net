# Platform Support

## Supported Platforms

| OS | Architecture | RID | Runtime Package |
|----|-------------|-----|-----------------|
| Windows | x64 | `win-x64` | `Imazen.WebP.NativeRuntime.win-x64` |
| Windows | x86 | `win-x86` | `Imazen.WebP.NativeRuntime.win-x86` |
| Windows | ARM64 | `win-arm64` | `Imazen.WebP.NativeRuntime.win-arm64` |
| Linux | x64 | `linux-x64` | `Imazen.WebP.NativeRuntime.linux-x64` |
| Linux | ARM64 | `linux-arm64` | `Imazen.WebP.NativeRuntime.linux-arm64` |
| macOS | x64 | `osx-x64` | `Imazen.WebP.NativeRuntime.osx-x64` |
| macOS | ARM64 | `osx-arm64` | `Imazen.WebP.NativeRuntime.osx-arm64` |

## Framework Support

| Target | Bitmap APIs | Raw Buffer APIs |
|--------|------------|-----------------|
| .NET Framework 4.7.2+ | Yes | Yes |
| .NET Standard 2.0 | Yes | Yes |
| .NET 8+ | Yes | Yes |

**Bitmap APIs** (`SimpleEncoder`, `SimpleDecoder`) use `System.Drawing.Bitmap` and require the `System.Drawing.Common` package on non-Windows platforms.

**Raw Buffer APIs** (`WebPEncoder`, `WebPDecoder`, `AnimEncoder`, `AnimDecoder`) work with byte arrays directly and have no `System.Drawing` dependency.

## Package Selection Guide

### Easiest — All Platforms

Install the all-in-one package:

```
dotnet add package Imazen.WebP.AllPlatforms
```

This includes the managed library and all 7 native runtimes. Best for applications where deployment size isn't critical.

### Minimal — Single Platform

Install the core package plus only the runtime you need:

```
dotnet add package Imazen.WebP
dotnet add package Imazen.WebP.NativeRuntime.linux-x64
```

### Multi-Platform Deployment

Install the core package plus each target runtime:

```
dotnet add package Imazen.WebP
dotnet add package Imazen.WebP.NativeRuntime.win-x64
dotnet add package Imazen.WebP.NativeRuntime.linux-x64
dotnet add package Imazen.WebP.NativeRuntime.osx-arm64
```

The correct native library is selected automatically at runtime based on the OS and architecture.

## Native Library Resolution

Native libraries are placed in `runtimes/{rid}/native/` by the NuGet packages and resolved automatically by the .NET runtime. The library also searches:

1. `runtimes/{rid}/native/` under the application base directory
2. Architecture-specific subdirectories (e.g., `x64/`, `arm64/`)
3. The application base directory itself

If the native library cannot be found, a detailed `DllNotFoundException` is thrown listing all paths that were searched.

## System.Drawing.Common on Non-Windows

On Linux and macOS, `System.Drawing.Common` requires `libgdiplus`. If you prefer to avoid this dependency, use the raw buffer APIs (`WebPEncoder`, `WebPDecoder`, `AnimEncoder`, `AnimDecoder`) which have no `System.Drawing` dependency.
