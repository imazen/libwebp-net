# Animation

Imazen.WebP supports both decoding and encoding animated WebP images using the libwebp demux/mux APIs.

## Decoding Animated WebP

### Decode All Frames

```csharp
using Imazen.WebP;

byte[] data = File.ReadAllBytes("animation.webp");
using var decoder = new AnimDecoder(data);

// Animation metadata
AnimInfo info = decoder.Info;
Console.WriteLine($"{info.FrameCount} frames, {info.Width}x{info.Height}, loops={info.LoopCount}");

// Decode all frames at once
List<AnimFrame> frames = decoder.DecodeAllFrames();
foreach (var frame in frames)
{
    // frame.Pixels — BGRA byte array for the full canvas
    // frame.Width, frame.Height — canvas dimensions
    // frame.TimestampMs — when this frame should be displayed
    // frame.DurationMs — how long to display this frame
}
```

### Frame-by-Frame Iteration

For memory-efficient decoding, iterate one frame at a time:

```csharp
using var decoder = new AnimDecoder(data);

while (decoder.HasMoreFrames())
{
    AnimFrame? frame = decoder.GetNextFrame();
    if (frame == null) break;

    ProcessFrame(frame.Pixels, frame.Width, frame.Height);
}
```

### Decode from Stream

```csharp
using var stream = File.OpenRead("animation.webp");
using var decoder = new AnimDecoder(stream);
var frames = decoder.DecodeAllFrames();
```

### Multi-Threaded Decoding

Enable threading for faster decode of large animations:

```csharp
using var decoder = new AnimDecoder(data, useThreads: true);
```

## Encoding Animated WebP

### Basic Animation

```csharp
using Imazen.WebP;

// All frames must have the same canvas dimensions
using var encoder = new AnimEncoder(width, height);

// Add frames with timestamps (milliseconds)
encoder.AddFrame(frame1Bgra, timestampMs: 0, quality: 80);
encoder.AddFrame(frame2Bgra, timestampMs: 100, quality: 80);
encoder.AddFrame(frame3Bgra, timestampMs: 200, quality: 80);

// Assemble and write
byte[] animatedWebP = encoder.Assemble();
File.WriteAllBytes("output.webp", animatedWebP);
```

### Custom Animation Options

```csharp
using var encoder = new AnimEncoder(
    width, height,
    loopCount: 0,               // 0 = infinite loop
    backgroundColor: 0x00000000, // Transparent
    allowMixed: true,            // Mix lossy and lossless frames
    minimizeSize: true);         // Optimize for smallest output
```

### Lossless Animation

Use quality `-1` for lossless frames:

```csharp
encoder.AddFrame(frameBgra, timestampMs: 0, quality: -1);
```

### Different Pixel Formats

Frames can be provided in any supported format:

```csharp
encoder.AddFrame(rgbaPixels, stride, WebPPixelFormat.Rgba, timestampMs: 0, quality: 80);
```

### Advanced Config per Frame

Apply a `WebPEncoderConfig` to individual frames:

```csharp
var config = new WebPEncoderConfig()
    .SetQuality(90)
    .SetMethod(4)
    .SetSharpYuv();

encoder.AddFrame(pixels, stride, WebPPixelFormat.Bgra, timestampMs: 0, config);
```

### Write to Stream

```csharp
using var output = File.Create("animation.webp");
encoder.Assemble(output);
```

## Important Notes

- All frames in an animation must share the same canvas dimensions.
- `AnimDecoder` and `AnimEncoder` implement `IDisposable` — always wrap in `using`.
- Decoded frame pixels are full-canvas BGRA composites (already alpha-blended with previous frames).
- Timestamps are cumulative from the start of the animation, not per-frame durations.
