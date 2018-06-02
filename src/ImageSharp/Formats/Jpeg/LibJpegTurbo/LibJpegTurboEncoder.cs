using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

using SixLabors.ImageSharp.PixelFormats;

using TurboJpegWrapper;

namespace SixLabors.ImageSharp.Formats.Jpeg.LibJpegTurbo
{
    /// <summary>
    /// dfsdf
    /// </summary>
    public class LibJpegTurboEncoder : IImageEncoder, IJpegEncoderOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether the metadata should be ignored when the image is being decoded.
        /// </summary>
        public bool IgnoreMetadata { get; set; }

        /// <summary>
        /// Gets or sets the quality, that will be used to encode the image. Quality
        /// index must be between 0 and 100 (compression from max to min).
        /// Defaults to <value>75</value>.
        /// </summary>
        public int Quality { get; set; } = 75;

        /// <summary>
        /// Gets or sets the subsample ration, that will be used to encode the image.
        /// </summary>
        public JpegSubsample? Subsample { get; set; }

        /// <summary>
        /// Encodes the image to the specified stream from the <see cref="Image{TPixel}"/>.
        /// </summary>
        /// <typeparam name="TPixel">The pixel format.</typeparam>
        /// <param name="image">The <see cref="Image{TPixel}"/> to encode from.</param>
        /// <param name="stream">The <see cref="Stream"/> to encode the image data to.</param>
        public void Encode<TPixel>(Image<TPixel> image, Stream stream)
            where TPixel : struct, IPixel<TPixel>
        {
            var subsample = this.Subsample ?? (this.Quality >= 91 ? JpegSubsample.Ratio444 : JpegSubsample.Ratio420);
            var bytes = MemoryMarshal.AsBytes(image.Frames.RootFrame.PixelBuffer.Span);
            var compressor = new TJCompressor();
            var result = compressor.Compress(
                bytes.ToArray(),
                image.Width * 4,
                image.Width,
                image.Height,
                PixelFormat.Format32bppArgb,
                subsample == JpegSubsample.Ratio420 ? TJSubsamplingOptions.TJSAMP_420 : TJSubsamplingOptions.TJSAMP_422,
                this.Quality,
                TJFlags.FASTDCT);
            stream.Write(result, 0, result.Length);
        }
    }
}