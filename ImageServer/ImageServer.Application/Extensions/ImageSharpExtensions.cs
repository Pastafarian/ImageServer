using System;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;

namespace ImageServer.Application.Extensions
{
    // ReSharper disable PossibleLossOfFraction
    public static class ImageSharpExtensions
    {
        public static IImageProcessingContext ApplyScalingWaterMark(this IImageProcessingContext processingContext, string text, Color color)
        {
            var (width, height) = processingContext.GetCurrentSize();
            var font = SystemFonts.CreateFont("Arial", 10);
            var size = TextMeasurer.Measure(text, new RendererOptions(font));
            var scalingFactor = Math.Min(width / size.Width, height / size.Height);
            var scaledFont = new Font(font, scalingFactor * font.Size);
            var center = new PointF(width / 2, height / 2);
            var textGraphicOptions = new TextGraphicsOptions
            {
                TextOptions = {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    
                },
                GraphicsOptions =
                {
                    BlendPercentage = (float)0.2,
                    Antialias = true
                }
            };
            
            return processingContext.DrawText(textGraphicOptions, text, scaledFont, color, center);
        }
    }
}
