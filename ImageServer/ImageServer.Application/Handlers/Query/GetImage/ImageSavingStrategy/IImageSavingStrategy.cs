using System.Threading;
using System.Threading.Tasks;
using ImageServer.Application.Enums;
using SixLabors.ImageSharp;

namespace ImageServer.Application.Handlers.Query.GetImage.ImageSavingStrategy
{
    public interface IImageSavingStrategy
    {
        Task<(byte[] content, string contentType)> SaveImage(Image image, ImageFileType imageFileType, CancellationToken cancellationToken);
    }
}