# libwebp wrapper for .NET 2.0 and higher

This wrapper was created due to Noesis Innovation's abandonment of http://webp.codeplex.com/

Our goals are also a bit different:

1. Offer low-level P/Invoke exposure for the full libwebp API.
2. Be binary compatible with as-is NMake builds of libwebp (no custom C/C++)
3. Offer a simple encode/decode API
4. Offer a more detailed encode/decode API for more complex use cases.

