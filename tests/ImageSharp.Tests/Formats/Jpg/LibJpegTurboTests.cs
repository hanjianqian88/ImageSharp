namespace SixLabors.ImageSharp.Tests.Formats.Jpg
{
    using SixLabors.ImageSharp.Formats.Jpeg;
    using SixLabors.ImageSharp.Formats.Jpeg.LibJpegTurbo;
    using SixLabors.ImageSharp.PixelFormats;
    using SixLabors.ImageSharp.Tests.TestUtilities.ImageComparison;

    using Xunit;

    public class LibJpegTurboTests
    {
        public static readonly TheoryData<JpegSubsample, int> BitsPerPixel_Quality =
            new TheoryData<JpegSubsample, int>
                {
                    { JpegSubsample.Ratio420, 40 },
                    { JpegSubsample.Ratio420, 60 },
                    { JpegSubsample.Ratio420, 100 },

                    { JpegSubsample.Ratio444, 40 },
                    { JpegSubsample.Ratio444, 60 },
                    { JpegSubsample.Ratio444, 100 },
                };
        
        [Theory]
        [WithFile(TestImages.Png.CalliphoraPartial, nameof(BitsPerPixel_Quality), PixelTypes.Rgba32)]
        [WithTestPatternImages(nameof(BitsPerPixel_Quality), 73, 71, PixelTypes.Rgba32)]
        [WithTestPatternImages(nameof(BitsPerPixel_Quality), 48, 24, PixelTypes.Rgba32)]
        [WithTestPatternImages(nameof(BitsPerPixel_Quality), 46, 8, PixelTypes.Rgba32)]
        [WithTestPatternImages(nameof(BitsPerPixel_Quality), 51, 7, PixelTypes.Rgba32)]
        [WithSolidFilledImages(nameof(BitsPerPixel_Quality), 1, 1, 255, 100, 50, 255, PixelTypes.Rgba32)]
        [WithTestPatternImages(nameof(BitsPerPixel_Quality), 7, 5, PixelTypes.Rgba32)]
        public void CanEncode<TPixel>(TestImageProvider<TPixel> provider, JpegSubsample subsample, int quality)
            where TPixel : struct, IPixel<TPixel>
        {
            using (var image = provider.GetImage())
            {
                var encoder = new LibJpegTurboEncoder()
                                  {
                                      Subsample = subsample,
                                      Quality = quality
                                  };
                string info = $"{subsample}-Q{quality}";
                ImageComparer comparer = GetComparer(quality, subsample);

                image.Save("/Users/pknopf/test.jpg", encoder);
                
                // Does DebugSave & load reference CompareToReferenceInput():
                image.VerifyEncoder(provider, "jpeg", info, encoder, comparer, referenceImageExtension: "png");
            }
        }
        
        /// <summary>
        /// Anton's SUPER-SCIENTIFIC tolerance threshold calculation
        /// </summary>
        private static ImageComparer GetComparer(int quality, JpegSubsample subsample)
        {
            float tolerance = 0.015f; // ~1.5%

            if (quality < 50)
            {
                tolerance *= 10f;
            }
            else if (quality < 75 || subsample == JpegSubsample.Ratio420)
            {
                tolerance *= 5f;
                if (subsample == JpegSubsample.Ratio420)
                {
                    tolerance *= 2f;
                }
            }

            return ImageComparer.Tolerant(tolerance);
        }
    }
}