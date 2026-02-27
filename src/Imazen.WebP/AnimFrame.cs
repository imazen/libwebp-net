using System;

namespace Imazen.WebP
{
    /// <summary>
    /// Represents a single frame in an animated WebP image.
    /// </summary>
    public class AnimFrame
    {
        /// <summary>
        /// The decoded pixel data for this frame (BGRA by default).
        /// This is a full-canvas composite — alpha-blended with previous frames.
        /// </summary>
        public byte[] Pixels { get; }

        /// <summary>
        /// Timestamp in milliseconds for when this frame should be displayed.
        /// </summary>
        public int TimestampMs { get; }

        /// <summary>
        /// Canvas width in pixels.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Canvas height in pixels.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Duration of this frame in milliseconds (computed from next frame's timestamp).
        /// -1 if unknown (last frame).
        /// </summary>
        public int DurationMs { get; internal set; }

        /// <summary>
        /// Creates a new animation frame with the given pixel data and timestamp.
        /// </summary>
        /// <param name="pixels">Decoded BGRA pixel data for the full canvas.</param>
        /// <param name="width">Canvas width in pixels.</param>
        /// <param name="height">Canvas height in pixels.</param>
        /// <param name="timestampMs">Frame timestamp in milliseconds.</param>
        public AnimFrame(byte[] pixels, int width, int height, int timestampMs)
        {
            Pixels = pixels ?? throw new ArgumentNullException(nameof(pixels));
            Width = width;
            Height = height;
            TimestampMs = timestampMs;
            DurationMs = -1;
        }
    }
}
