using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ImageServer.Application.Enums;
using SixLabors.ImageSharp;

namespace ImageServer.Application.Handlers.Query.GetImage.ImageSavingStrategy
{
    public class PngSaver : IImageSaver
    {
        public async Task<byte[]> SaveImage(Image image, CancellationToken cancellationToken)
        {
            await using var memoryStream = new MemoryStream();
            await image.SaveAsPngAsync(memoryStream, cancellationToken);
            memoryStream.Position = 0;

            var imageBytes = memoryStream.ToArray();

            return imageBytes;
        }

        public bool ImageFileType(ImageFileType imageFileType)
        {
            return imageFileType == Enums.ImageFileType.Default || imageFileType == Enums.ImageFileType.Png;
        }
    }
}