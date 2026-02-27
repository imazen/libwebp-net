using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Xunit;
using Imazen.WebP;
using Imazen.WebP.Extern;

namespace Imazen.Test.Webp
{
#if NETCOREAPP
    public class TestNativeExports
    {
        [Fact]
        public void AllWebPFunctionsExist()
        {
            // Force library load via public API
            _ = SimpleDecoder.GetDecoderVersion();

            var handle = GetLibraryHandle("webp");
            if (handle == IntPtr.Zero)
                handle = GetLibraryHandle("libwebp");
            Assert.NotEqual(IntPtr.Zero, handle);

            string[] requiredFunctions = {
                // decode.h
                "WebPGetDecoderVersion",
                "WebPGetInfo",
                "WebPDecodeRGBA",
                "WebPDecodeARGB",
                "WebPDecodeBGRA",
                "WebPDecodeRGB",
                "WebPDecodeBGR",
                "WebPDecodeYUV",
                "WebPDecodeRGBAInto",
                "WebPDecodeARGBInto",
                "WebPDecodeBGRAInto",
                "WebPDecodeRGBInto",
                "WebPDecodeBGRInto",
                "WebPDecodeYUVInto",
                "WebPInitDecBufferInternal",
                "WebPFreeDecBuffer",
                "WebPINewDecoder",
                "WebPIDelete",
                "WebPIAppend",
                "WebPIUpdate",
                "WebPGetFeaturesInternal",
                "WebPInitDecoderConfigInternal",
                "WebPValidateDecoderConfig",
                "WebPDecode",
                // encode.h
                "WebPGetEncoderVersion",
                "WebPEncodeRGB",
                "WebPEncodeBGR",
                "WebPEncodeRGBA",
                "WebPEncodeBGRA",
                "WebPEncodeLosslessRGB",
                "WebPEncodeLosslessBGR",
                "WebPEncodeLosslessRGBA",
                "WebPEncodeLosslessBGRA",
                "WebPConfigInitInternal",
                "WebPConfigLosslessPreset",
                "WebPValidateConfig",
                "WebPMemoryWriterInit",
                "WebPMemoryWriterClear",
                "WebPMemoryWrite",
                "WebPPictureInitInternal",
                "WebPPictureAlloc",
                "WebPPictureFree",
                "WebPPictureCopy",
                "WebPPictureDistortion",
                "WebPPictureCrop",
                "WebPPictureView",
                "WebPPictureIsView",
                "WebPPictureRescale",
                "WebPPictureImportRGB",
                "WebPPictureImportRGBA",
                "WebPPictureImportRGBX",
                "WebPPictureImportBGR",
                "WebPPictureImportBGRA",
                "WebPPictureImportBGRX",
                "WebPPictureARGBToYUVA",
                "WebPPictureARGBToYUVADithered",
                "WebPPictureSharpARGBToYUVA",
                "WebPPictureSmartARGBToYUVA",
                "WebPPictureYUVAToARGB",
                "WebPCleanupTransparentArea",
                "WebPPictureHasTransparency",
                "WebPBlendAlpha",
                "WebPPlaneDistortion",
                "WebPEncode",
                // types.h
                "WebPFree",
                "WebPMalloc",
            };

            var missing = new List<string>();
            foreach (var fn in requiredFunctions)
            {
                if (!NativeLibrary.TryGetExport(handle, fn, out _))
                    missing.Add(fn);
            }

            Assert.True(missing.Count == 0,
                $"Missing exports from libwebp: {string.Join(", ", missing)}");
        }

        [Fact]
        public void AllWebPMuxFunctionsExist()
        {
            // Force library load via public AnimEncoder API
            var opts = new WebPAnimEncoderOptions();
            NativeMethods.WebPAnimEncoderOptionsInitInternal(ref opts, NativeMethods.WEBP_MUX_ABI_VERSION);

            var handle = GetLibraryHandle("webpmux");
            if (handle == IntPtr.Zero)
                handle = GetLibraryHandle("libwebpmux");
            Assert.NotEqual(IntPtr.Zero, handle);

            string[] requiredFunctions = {
                "WebPAnimEncoderOptionsInitInternal",
                "WebPAnimEncoderNewInternal",
                "WebPAnimEncoderAdd",
                "WebPAnimEncoderAssemble",
                "WebPAnimEncoderGetError",
                "WebPAnimEncoderDelete",
            };

            var missing = new List<string>();
            foreach (var fn in requiredFunctions)
            {
                if (!NativeLibrary.TryGetExport(handle, fn, out _))
                    missing.Add(fn);
            }

            Assert.True(missing.Count == 0,
                $"Missing exports from libwebpmux: {string.Join(", ", missing)}");
        }

        [Fact]
        public void AllWebPDemuxFunctionsExist()
        {
            // Force library load
            var opts = new WebPAnimDecoderOptions();
            NativeMethods.WebPAnimDecoderOptionsInitInternal(ref opts, NativeMethods.WEBP_DEMUX_ABI_VERSION);

            var handle = GetLibraryHandle("webpdemux");
            if (handle == IntPtr.Zero)
                handle = GetLibraryHandle("libwebpdemux");
            Assert.NotEqual(IntPtr.Zero, handle);

            string[] requiredFunctions = {
                "WebPAnimDecoderOptionsInitInternal",
                "WebPAnimDecoderNewInternal",
                "WebPAnimDecoderGetInfo",
                "WebPAnimDecoderGetNext",
                "WebPAnimDecoderHasMoreFrames",
                "WebPAnimDecoderReset",
                "WebPAnimDecoderDelete",
            };

            var missing = new List<string>();
            foreach (var fn in requiredFunctions)
            {
                if (!NativeLibrary.TryGetExport(handle, fn, out _))
                    missing.Add(fn);
            }

            Assert.True(missing.Count == 0,
                $"Missing exports from libwebpdemux: {string.Join(", ", missing)}");
        }

        private static IntPtr GetLibraryHandle(string name)
        {
            // Use the assembly-aware overload so it goes through the DllImportResolver
            var asm = typeof(Imazen.WebP.SimpleDecoder).Assembly;
            if (NativeLibrary.TryLoad(name, asm, null, out var handle))
                return handle;
            if (NativeLibrary.TryLoad("lib" + name, asm, null, out handle))
                return handle;
            return IntPtr.Zero;
        }
    }
#endif
}
