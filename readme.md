# libwebp wrapper for .NET 2.0 and higher

Build status for branch 'master': ![build status](http://img.shields.io//appveyor/ci/nathanaeljones/libwebp-net.svg)
Last nuget release: ![Nuget release version](http://img.shields.io/nuget/v/Imazen.WebP.svg)



This library is available on Nuget as [Imazen.WebP](http://nuget.org/packages/Imazen.WebP).

This wrapper was created due to Noesis Innovation's abandonment of http://webp.codeplex.com/

Our goals are also a bit more ambitious:

1. Offer low-level P/Invoke exposure for the full libwebp API (complete, partially tested).
2. Be binary compatible with as-is NMake builds of libwebp (no custom C/C++) (complete).
3. Offer a simple encode/decode API (complete).
4. Offer a more detailed encode/decode API for more complex use cases (not yet completed).


This software is released under the MIT license:

Copyright (c) 2012 Imazen

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
