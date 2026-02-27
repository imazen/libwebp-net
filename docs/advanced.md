# Advanced Configuration

## WebPEncoderConfig Fluent API

`WebPEncoderConfig` provides a fluent builder for all libwebp encoding parameters:

```csharp
using Imazen.WebP;

var config = new WebPEncoderConfig()
    .SetQuality(85)           // Lossy quality 0–100
    .SetMethod(6)             // Compression effort 0–6
    .SetSharpYuv()            // Better chroma downsampling
    .SetSnsStrength(50)       // Spatial noise shaping 0–100
    .SetFilterStrength(20)    // Deblocking filter 0–100
    .SetAlphaQuality(90)      // Alpha quality 0–100
    .SetMultiThreaded()       // Multi-threaded encoding
    .SetExact();              // Preserve RGB under transparent areas
```

### All Available Settings

| Method | Description | Range |
|--------|-------------|-------|
| `SetQuality(float)` | Lossy quality level | 0–100 |
| `SetLossless(bool)` | Enable lossless mode | — |
| `SetLosslessPreset(int)` | Lossless preset level | 0–9 |
| `SetMethod(int)` | Compression speed/quality tradeoff | 0–6 |
| `SetNearLossless(int)` | Near-lossless preprocessing | 0–100 |
| `SetTargetSize(int)` | Target file size in bytes | — |
| `SetTargetPSNR(float)` | Target PSNR in dB | — |
| `SetMultiThreaded(bool)` | Multi-threaded encoding | — |
| `SetSnsStrength(int)` | Spatial noise shaping | 0–100 |
| `SetFilterStrength(int)` | Deblocking filter | 0–100 |
| `SetAlphaQuality(int)` | Alpha channel quality | 0–100 |
| `SetImageHint(WebPImageHint)` | Content type hint | — |
| `SetExact(bool)` | Preserve RGB under transparency | — |
| `SetSharpYuv(bool)` | Sharp RGB→YUV conversion | — |

### Validation

Validate your configuration before encoding:

```csharp
if (!config.Validate())
    throw new ArgumentException("Invalid encoder config");
```

### Native Config Access

For direct P/Invoke scenarios, get the underlying `WebPConfig` struct:

```csharp
WebPConfig nativeConfig = config.GetNativeConfig();
```

## ABI Version Checks

The library automatically validates ABI compatibility when the native library is first loaded. You can also check manually:

```csharp
using Imazen.WebP;

// Throws NotSupportedException if incompatible
AbiVersionCheck.ValidateOrThrow();

// Get version string
string version = AbiVersionCheck.GetVersionString();
// e.g. "decoder=1.5.0, encoder=1.5.0"

// Individual version strings
string decoderVer = SimpleDecoder.GetDecoderVersion();
string encoderVer = SimpleEncoder.GetEncoderVersion();
```

## Native Library Loading

Imazen.WebP automatically locates and loads native libraries using this search order:

1. `runtimes/{rid}/native/` — standard .NET runtime package location
2. Architecture subdirectory (`x64/`, `arm64/`, etc.)
3. Application base directory

The library resolves the correct platform and architecture at runtime via `RuntimeInformation`. Supported platforms:

- **Windows**: `webp.dll`, `webpdemux.dll`, `webpmux.dll`
- **Linux**: `libwebp.so`, `libwebpdemux.so`, `libwebpmux.so`
- **macOS**: `libwebp.dylib`, `libwebpdemux.dylib`, `libwebpmux.dylib`

If loading fails, a detailed `DllNotFoundException` is thrown with diagnostics showing all paths attempted.

## WebPInfo — Image Probing

Query image metadata without full decoding:

```csharp
using Imazen.WebP;

byte[] data = File.ReadAllBytes("image.webp");

// Quick size check (returns false for invalid data)
if (WebPInfo.TryGetSize(data, out int width, out int height))
    Console.WriteLine($"{width}x{height}");

// Full bitstream features
WebPImageInfo info = WebPInfo.GetImageInfo(data);
// info.Width, info.Height
// info.HasAlpha — true if image has transparency
// info.HasAnimation — true if animated WebP
// info.Format — 0=undefined/mixed, 1=lossy, 2=lossless
```
