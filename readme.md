# libwebp wrapper for .NET

Build status for branch 'master': ![build status](http://img.shields.io/appveyor/ci/imazen/libwebp-net.svg)
Last nuget release: ![Nuget release version](http://img.shields.io/nuget/v/Imazen.WebP.svg)

This library is available on Nuget as [Imazen.WebP](http://nuget.org/packages/Imazen.WebP).

This library offers P/Invoke exposure for webp/decode.h and webp/encode.h, but not demux.h and mux.h.

## Key APIs

* `new Imazen.WebP.SimpleDecoder().DecodeFromBytes(byte[] data, long length)` -> `System.Drawing.Bitmap`
* `new Imazen.WebP.SimpleEncoder().Encode(Bitmap from, Stream to, float quality)`
* `Imazen.WebP.SimpleEncoder.GetEncoderVersion() -> String`, `Imazen.WebP.SimpleDecoder.GetDecoderVersion() -> String`

## Improvements we're very interested in

* Expose the power of Imazen.WebP.Extern.WebPConfig and Imazen.WebP.Extern.WebPPreset for better encoding. 
* Consider using WebPDecode for better decode error details. 
* Animation support
* Make LoadLibrary cross-platform (although it is not strictly neccessary)
* Add .NET Core support. This will likely require introducing a PixelBuffer{width, height, ptr, stride} interface that can wrap System.Drawing.Bitmap via adapter. System.Drawing.Bitmap is currently directly used by the `SimpleDecoder` and `SimpleEncoder` classes, but these total < 200 lines of code. Or, leave the core API as P/Invoke only. libwebp doesn't have a bad API at all. 

## Windows builds of libwebp 0.5.2 can be found here:

https://s3.amazonaws.com/resizer-dynamic-downloads/webp/0.5.2/x86_64/libwebp.dll
https://s3.amazonaws.com/resizer-dynamic-downloads/webp/0.5.2/x86/libwebp.dll

This library is binary compatible with as-is NMake builds of libwebp (no custom C/C++).

## License

This software is released under the MIT license:

Copyright (c) 2012 Imazen

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
