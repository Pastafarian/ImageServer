using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ImageServer.Application.Enums;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace ImageServer.Application.Handlers.Query.GetImage.ImageSavingStrategy
{
    public class JpgSaver : IImageSaver
    {
        public async Task<(byte[] content, string contentType)> SaveImage(Image image, CancellationToken cancellationToken)
        {
            await using var memoryStream = new MemoryStream();
            await image.SaveAsJpegAsync(memoryStream, new JpegEncoder { Quality = 90, Subsample = JpegSubsample.Ratio444 }, cancellationToken);
            memoryStream.Position = 0;

            var imageBytes = memoryStream.ToArray();

            return (imageBytes, "image/jpeg");
        }

        public bool ImageFileType(ImageFileType imageFileType)
        {
            return imageFileType == Enums.ImageFileType.Jpg;
        }
    }
}