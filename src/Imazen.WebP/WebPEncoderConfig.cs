using System;
using Imazen.WebP.Extern;

namespace Imazen.WebP
{
    /// <summary>
    /// Builder for advanced WebP encoding configuration.
    /// Uses a fluent API pattern.
    /// </summary>
    public class WebPEncoderConfig
    {
        private WebPConfig _config;
        private bool _initialized;

        /// <summary>
        /// Creates a new config with default settings (lossy, quality 75).
        /// </summary>
        public WebPEncoderConfig()
        {
            _config = new WebPConfig();
            if (NativeMethods.WebPConfigInit(ref _config) == 0)
                throw new Exception("WebP version mismatch: failed to initialize config");
            _initialized = true;
        }

        /// <summary>
        /// Creates a new config with a preset and quality level.
        /// </summary>
        public WebPEncoderConfig(WebPPreset preset, float quality)
        {
            _config = new WebPConfig();
            if (NativeMethods.WebPConfigPreset(ref _config, preset, quality) == 0)
                throw new Exception("WebP version mismatch: failed to initialize config with preset");
            _initialized = true;
        }

        /// <summary>
        /// Sets lossy quality (0-100).
        /// </summary>
        public WebPEncoderConfig SetQuality(float quality)
        {
            EnsureInitialized();
            _config.quality = Math.Max(0, Math.Min(100, quality));
            _config.lossless = 0;
            return this;
        }

        /// <summary>
        /// Enables lossless encoding.
        /// </summary>
        public WebPEncoderConfig SetLossless(bool lossless = true)
        {
            EnsureInitialized();
            _config.lossless = lossless ? 1 : 0;
            return this;
        }

        /// <summary>
        /// Sets lossless preset level (0=fastest, 9=best compression).
        /// </summary>
        public WebPEncoderConfig SetLosslessPreset(int level)
        {
            EnsureInitialized();
            _config.lossless = 1;
            NativeMethods.WebPConfigLosslessPreset(ref _config, level);
            return this;
        }

        /// <summary>
        /// Sets quality/speed trade-off (0=fast, 6=slower-better).
        /// </summary>
        public WebPEncoderConfig SetMethod(int method)
        {
            EnsureInitialized();
            _config.method = Math.Max(0, Math.Min(6, method));
            return this;
        }

        /// <summary>
        /// Sets near-lossless quality [0=max loss .. 100=off(default)].
        /// </summary>
        public WebPEncoderConfig SetNearLossless(int level)
        {
            EnsureInitialized();
            _config.near_lossless = Math.Max(0, Math.Min(100, level));
            return this;
        }

        /// <summary>
        /// Sets target file size in bytes (0 = disabled).
        /// </summary>
        public WebPEncoderConfig SetTargetSize(int bytes)
        {
            EnsureInitialized();
            _config.target_size = bytes;
            return this;
        }

        /// <summary>
        /// Sets target PSNR in dB (0 = disabled).
        /// </summary>
        public WebPEncoderConfig SetTargetPSNR(float psnr)
        {
            EnsureInitialized();
            _config.target_PSNR = psnr;
            return this;
        }

        /// <summary>
        /// Enables multi-threaded encoding.
        /// </summary>
        public WebPEncoderConfig SetMultiThreaded(bool enabled = true)
        {
            EnsureInitialized();
            _config.thread_level = enabled ? 1 : 0;
            return this;
        }

        /// <summary>
        /// Sets SNS strength (0=off, 100=maximum).
        /// </summary>
        public WebPEncoderConfig SetSnsStrength(int strength)
        {
            EnsureInitialized();
            _config.sns_strength = Math.Max(0, Math.Min(100, strength));
            return this;
        }

        /// <summary>
        /// Sets filter strength (0=off, 100=strongest).
        /// </summary>
        public WebPEncoderConfig SetFilterStrength(int strength)
        {
            EnsureInitialized();
            _config.filter_strength = Math.Max(0, Math.Min(100, strength));
            return this;
        }

        /// <summary>
        /// Sets alpha quality (0=smallest, 100=lossless).
        /// </summary>
        public WebPEncoderConfig SetAlphaQuality(int quality)
        {
            EnsureInitialized();
            _config.alpha_quality = Math.Max(0, Math.Min(100, quality));
            return this;
        }

        /// <summary>
        /// Sets image hint for better compression.
        /// </summary>
        public WebPEncoderConfig SetImageHint(WebPImageHint hint)
        {
            EnsureInitialized();
            _config.image_hint = hint;
            return this;
        }

        /// <summary>
        /// If set, preserve exact RGB values under transparent area.
        /// </summary>
        public WebPEncoderConfig SetExact(bool exact = true)
        {
            EnsureInitialized();
            _config.exact = exact ? 1 : 0;
            return this;
        }

        /// <summary>
        /// Use sharp (and slow) RGB->YUV conversion.
        /// </summary>
        public WebPEncoderConfig SetSharpYuv(bool enabled = true)
        {
            EnsureInitialized();
            _config.use_sharp_yuv = enabled ? 1 : 0;
            return this;
        }

        /// <summary>
        /// Validates the configuration. Returns true if valid.
        /// </summary>
        public bool Validate()
        {
            EnsureInitialized();
            return NativeMethods.WebPValidateConfig(ref _config) != 0;
        }

        /// <summary>
        /// Returns the internal WebPConfig struct for use with low-level P/Invoke.
        /// </summary>
        public WebPConfig GetNativeConfig()
        {
            EnsureInitialized();
            return _config;
        }

        private void EnsureInitialized()
        {
            if (!_initialized)
                throw new InvalidOperationException("Config not initialized");
        }
    }
}
