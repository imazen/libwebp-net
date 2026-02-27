# Getting Started

## Installation

The simplest way to get started is to install the all-in-one package that bundles managed bindings and all native runtimes:

```
dotnet add package Imazen.WebP.AllPlatforms
```

If you want to minimize deployment size, install the core package plus only the runtime(s) you need:

```
dotnet add package Imazen.WebP
dotnet add package Imazen.WebP.NativeRuntime.win-x64
```

See [Platform Support](platform-support.md) for all available runtime packages.

## Your First Encode

```csharp
using Imazen.WebP;

var encoder = new SimpleEncoder();
using var bitmap = new System.Drawing.Bitmap("photo.png");
using var output = File.Create("photo.webp");

// Quality 0–100 for lossy, or -1 for lossless
encoder.Encode(bitmap, output, 80);
```

## Your First Decode

```csharp
using Imazen.WebP;

var decoder = new SimpleDecoder();
byte[] webpData = File.ReadAllBytes("photo.webp");
using var bitmap = decoder.DecodeFromBytes(webpData, webpData.LongLength);
bitmap.Save("photo.png");
```

## Cross-Platform (No System.Drawing)

If you don't want a dependency on `System.Drawing`, use the raw buffer APIs:

```csharp
using Imazen.WebP;

// Decode
byte[] webpData = File.ReadAllBytes("photo.webp");
byte[] pixels = WebPDecoder.Decode(webpData, out int width, out int height, WebPPixelFormat.Rgba);

// Encode
byte[] encoded = WebPEncoder.Encode(pixels, width, height, width * 4, WebPPixelFormat.Rgba, 80);
File.WriteAllBytes("output.webp", encoded);
```

## Next Steps

- [Encoding](encoding.md) — lossy, lossless, advanced config, quality tuning
- [Decoding](decoding.md) — Bitmap decode, raw buffer decode, stream decode, format detection
- [Animation](animation.md) — decode and encode animated WebP images
- [Advanced Configuration](advanced.md) — fluent encoder config, ABI checks, native library loading
