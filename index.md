---
_layout: landing
---

# Imazen.WebP

.NET bindings for Google's [libwebp](https://developers.google.com/speed/webp). Encode, decode, and animate WebP images from any .NET platform.

## Get Started

```
dotnet add package Imazen.WebP.AllPlatforms
```

Then follow the [Getting Started](docs/getting-started.md) guide.

## Key Capabilities

| Feature | API |
|---------|-----|
| Encode Bitmap to WebP | [SimpleEncoder](api/Imazen.WebP.SimpleEncoder.yml) |
| Decode WebP to Bitmap | [SimpleDecoder](api/Imazen.WebP.SimpleDecoder.yml) |
| Raw pixel encode (cross-platform) | [WebPEncoder](api/Imazen.WebP.WebPEncoder.yml) |
| Raw pixel decode (cross-platform) | [WebPDecoder](api/Imazen.WebP.WebPDecoder.yml) |
| Advanced encoding config | [WebPEncoderConfig](api/Imazen.WebP.WebPEncoderConfig.yml) |
| Animation decode | [AnimDecoder](api/Imazen.WebP.AnimDecoder.yml) |
| Animation encode | [AnimEncoder](api/Imazen.WebP.AnimEncoder.yml) |
| Image probing | [WebPInfo](api/Imazen.WebP.WebPInfo.yml) |

## Platform Support

Native binaries are provided for 7 platforms: Windows (x64, x86, ARM64), Linux (x64, ARM64), and macOS (x64, ARM64). See [Platform Support](docs/platform-support.md) for details.

## License

[MIT](https://github.com/imazen/libwebp-net/blob/main/LICENSE.txt) — Copyright 2012–2026 Imazen LLC
