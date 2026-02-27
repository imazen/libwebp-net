# Encoding

## Simple Encoding (Bitmap)

The `SimpleEncoder` class encodes `System.Drawing.Bitmap` objects to WebP:

```csharp
using Imazen.WebP;

var encoder = new SimpleEncoder();
using var bitmap = new Bitmap("input.png");
using var stream = File.Create("output.webp");

// Lossy at quality 80
encoder.Encode(bitmap, stream, 80);
```

Set quality to `-1` for lossless encoding:

```csharp
encoder.Encode(bitmap, stream, -1);
```

## Raw Buffer Encoding (Cross-Platform)

The static `WebPEncoder` class works with raw pixel arrays — no `System.Drawing` required:

```csharp
using Imazen.WebP;

// From RGBA pixel data
byte[] encoded = WebPEncoder.Encode(
    rgbaPixels, width, height, width * 4,
    WebPPixelFormat.Rgba, quality: 80);

// Write directly to a stream
using var stream = File.Create("output.webp");
WebPEncoder.Encode(rgbaPixels, width, height, width * 4,
    WebPPixelFormat.Rgba, 80, stream);
```

Supported pixel formats: `Bgra`, `Rgba`, `Bgr`, `Rgb`.

## Advanced Encoding Config

For fine-grained control, use `WebPEncoderConfig` with its fluent API:

```csharp
using Imazen.WebP;

var config = new WebPEncoderConfig()
    .SetQuality(85)
    .SetMethod(6)           // 0=fast, 6=best compression
    .SetSharpYuv()          // Better RGB→YUV conversion
    .SetSnsStrength(50)     // Spatial noise shaping
    .SetFilterStrength(20)  // Deblocking filter
    .SetAlphaQuality(90);   // Alpha channel quality

// With Bitmap
var encoder = new SimpleEncoder();
encoder.Encode(bitmap, stream, config);

// With raw pixels
byte[] encoded = WebPEncoder.Encode(
    pixels, width, height, stride,
    WebPPixelFormat.Bgra, config);
```

### Lossless Encoding

```csharp
var config = new WebPEncoderConfig()
    .SetLossless()
    .SetMethod(6);  // Best lossless compression

// Or use lossless presets (0=fastest, 9=smallest)
var config = new WebPEncoderConfig()
    .SetLosslessPreset(6);
```

### Near-Lossless

Near-lossless mode pre-processes the image to improve compression while maintaining visual quality:

```csharp
var config = new WebPEncoderConfig()
    .SetLossless()
    .SetNearLossless(60);  // 0=max preprocessing, 100=off
```

### Preset-Based Configuration

Initialize from a built-in preset tuned for specific content types:

```csharp
var config = new WebPEncoderConfig(WebPPreset.WEBP_PRESET_PHOTO, quality: 80);
```

Available presets: `WEBP_PRESET_DEFAULT`, `WEBP_PRESET_PICTURE`, `WEBP_PRESET_PHOTO`, `WEBP_PRESET_DRAWING`, `WEBP_PRESET_ICON`, `WEBP_PRESET_TEXT`.

### Target File Size

Encode to a specific file size instead of a quality level:

```csharp
var config = new WebPEncoderConfig()
    .SetTargetSize(50_000);  // Target ~50KB
```

### Configuration Validation

Always validate before encoding if building configs dynamically:

```csharp
if (!config.Validate())
    throw new Exception("Invalid encoder config");
```
