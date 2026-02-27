# Imazen.WebP — .NET Bindings for libwebp

[![NuGet](https://img.shields.io/nuget/v/Imazen.WebP.svg)](https://www.nuget.org/packages/Imazen.WebP)
[![Build](https://github.com/imazen/libwebp-net/actions/workflows/dotnet.yml/badge.svg)](https://github.com/imazen/libwebp-net/actions/workflows/dotnet.yml)
[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE.txt)

Encode, decode, and animate WebP images from .NET. Supports both `System.Drawing.Bitmap` and raw pixel buffer APIs for cross-platform use.

## Features

- **Encode & Decode** — lossy, lossless, and advanced config encoding; decode to Bitmap or raw byte arrays
- **Animation** — decode animated WebP to individual frames; encode frames into animated WebP
- **Cross-Platform** — raw buffer APIs (`WebPEncoder`, `WebPDecoder`) work everywhere without `System.Drawing`
- **7 Platform Runtimes** — native libwebp binaries for `win-x64`, `win-x86`, `win-arm64`, `linux-x64`, `linux-arm64`, `osx-x64`, `osx-arm64`
- **Multi-Framework** — targets `net48`, `netstandard2.0`, and `net8.0`
- **Advanced Config** — fluent `WebPEncoderConfig` builder for quality, method, lossless presets, near-lossless, alpha quality, sharp YUV, and more
- **Probing** — `WebPInfo` retrieves dimensions, alpha, animation, and format without full decoding

## Quick Start

### Install

```
dotnet add package Imazen.WebP.AllPlatforms
```

Or install only the runtime you need (see [Packages](#packages) below).

### Encode a Bitmap

```csharp
using Imazen.WebP;

var encoder = new SimpleEncoder();
using var bitmap = new Bitmap("input.png");
using var stream = File.Create("output.webp");

// Lossy encoding (quality 0–100), or -1 for lossless
encoder.Encode(bitmap, stream, 80);
```

### Decode a Bitmap

```csharp
using Imazen.WebP;

var decoder = new SimpleDecoder();
byte[] webpBytes = File.ReadAllBytes("image.webp");
using Bitmap bitmap = decoder.DecodeFromBytes(webpBytes, webpBytes.LongLength);
```

### Decode to Raw Pixels (Cross-Platform)

```csharp
using Imazen.WebP;

byte[] webpBytes = File.ReadAllBytes("image.webp");
byte[] pixels = WebPDecoder.Decode(webpBytes, out int width, out int height, WebPPixelFormat.Rgba);
// pixels is now width * height * 4 bytes of RGBA data
```

### Encode from Raw Pixels (Cross-Platform)

```csharp
using Imazen.WebP;

byte[] encoded = WebPEncoder.Encode(rgbaPixels, width, height, width * 4, WebPPixelFormat.Rgba, quality: 80);
File.WriteAllBytes("output.webp", encoded);
```

### Advanced Encoding Config

```csharp
using Imazen.WebP;

var config = new WebPEncoderConfig()
    .SetQuality(85)
    .SetMethod(6)           // Slowest, best compression
    .SetSharpYuv()          // Better color fidelity
    .SetSnsStrength(50)
    .SetFilterStrength(20);

var encoder = new SimpleEncoder();
using var bitmap = new Bitmap("input.png");
using var stream = File.Create("output.webp");
encoder.Encode(bitmap, stream, config);
```

### Decode Animation

```csharp
using Imazen.WebP;

byte[] animatedWebP = File.ReadAllBytes("animation.webp");
using var decoder = new AnimDecoder(animatedWebP);
Console.WriteLine($"{decoder.Info.FrameCount} frames, {decoder.Info.Width}x{decoder.Info.Height}");

foreach (var frame in decoder.DecodeAllFrames())
{
    // frame.Pixels = BGRA byte array for the full canvas
    // frame.TimestampMs, frame.DurationMs
}
```

### Encode Animation

```csharp
using Imazen.WebP;

using var encoder = new AnimEncoder(width, height, loopCount: 0);
encoder.AddFrame(frame1Bgra, timestampMs: 0, quality: 80);
encoder.AddFrame(frame2Bgra, timestampMs: 100, quality: 80);
encoder.AddFrame(frame3Bgra, timestampMs: 200, quality: 80);

byte[] animatedWebP = encoder.Assemble();
File.WriteAllBytes("animation.webp", animatedWebP);
```

### Probe Image Info

```csharp
using Imazen.WebP;

byte[] data = File.ReadAllBytes("image.webp");
var info = WebPInfo.GetImageInfo(data);
Console.WriteLine($"{info.Width}x{info.Height}, alpha={info.HasAlpha}, animated={info.HasAnimation}");
```

## Packages

| Package | Description |
|---------|-------------|
| [`Imazen.WebP`](https://www.nuget.org/packages/Imazen.WebP) | Managed bindings (requires a runtime package) |
| [`Imazen.WebP.AllPlatforms`](https://www.nuget.org/packages/Imazen.WebP.AllPlatforms) | Managed bindings + all 7 native runtimes |
| `Imazen.WebP.NativeRuntime.win-x64` | Windows x64 native libraries |
| `Imazen.WebP.NativeRuntime.win-x86` | Windows x86 native libraries |
| `Imazen.WebP.NativeRuntime.win-arm64` | Windows ARM64 native libraries |
| `Imazen.WebP.NativeRuntime.linux-x64` | Linux x64 native libraries |
| `Imazen.WebP.NativeRuntime.linux-arm64` | Linux ARM64 native libraries |
| `Imazen.WebP.NativeRuntime.osx-x64` | macOS x64 native libraries |
| `Imazen.WebP.NativeRuntime.osx-arm64` | macOS ARM64 (Apple Silicon) native libraries |

## Platform Support

| OS | Architecture | .NET Framework 4.8+ | .NET Standard 2.0 | .NET 8+ |
|----|-------------|---------------------|--------------------|---------|
| Windows | x64 | Yes | Yes | Yes |
| Windows | x86 | Yes | Yes | Yes |
| Windows | ARM64 | Yes (.NET 4.8.1+) | Yes | Yes |
| Linux | x64 | — | Yes | Yes |
| Linux | ARM64 | — | Yes | Yes |
| macOS | x64 | — | Yes | Yes |
| macOS | ARM64 | — | Yes | Yes |

## API Reference

Full API documentation is available at [imazen.github.io/libwebp-net](https://imazen.github.io/libwebp-net/).

## License

[MIT](LICENSE.txt) — Copyright 2012–2026 Imazen LLC
