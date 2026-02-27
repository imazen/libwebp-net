# Decoding

## Decode to Bitmap

The `SimpleDecoder` class decodes WebP images into `System.Drawing.Bitmap` (32bpp ARGB):

```csharp
using Imazen.WebP;

var decoder = new SimpleDecoder();

// From byte array
byte[] data = File.ReadAllBytes("image.webp");
using Bitmap bmp = decoder.DecodeFromBytes(data, data.LongLength);

// From stream
using var stream = File.OpenRead("image.webp");
using Bitmap bmp = decoder.DecodeFromStream(stream);
```

## Decode to Raw Pixels (Cross-Platform)

The static `WebPDecoder` class decodes to raw pixel arrays without `System.Drawing`:

```csharp
using Imazen.WebP;

byte[] data = File.ReadAllBytes("image.webp");

// Default: BGRA format
byte[] pixels = WebPDecoder.Decode(data, out int width, out int height);

// Specify format
byte[] rgba = WebPDecoder.Decode(data, out int w, out int h, WebPPixelFormat.Rgba);
```

### Decode into Existing Buffer

Avoid allocations by decoding directly into a pre-allocated buffer:

```csharp
byte[] data = File.ReadAllBytes("image.webp");

// Get dimensions first
WebPInfo.TryGetSize(data, out int width, out int height);

// Allocate and decode into buffer
int stride = width * 4;
byte[] output = new byte[stride * height];
WebPDecoder.Decode(data, output, stride, WebPPixelFormat.Bgra);
```

### Decode from Stream

```csharp
using var stream = File.OpenRead("image.webp");
byte[] pixels = WebPDecoder.DecodeFromStream(stream, out int width, out int height, WebPPixelFormat.Rgba);
```

## Format Detection

Check if data is a valid WebP file without decoding:

```csharp
// From byte array
bool isWebP = WebPDecoder.IsWebP(data);

// From seekable stream (position is restored)
using var stream = File.OpenRead("unknown.bin");
bool isWebP = WebPDecoder.IsWebP(stream);
```

## Image Probing

Retrieve image metadata without full decoding using `WebPInfo`:

```csharp
byte[] data = File.ReadAllBytes("image.webp");

// Quick size check
if (WebPInfo.TryGetSize(data, out int width, out int height))
    Console.WriteLine($"{width}x{height}");

// Full feature info
var info = WebPInfo.GetImageInfo(data);
Console.WriteLine($"Alpha: {info.HasAlpha}, Animated: {info.HasAnimation}, Format: {info.Format}");
```
