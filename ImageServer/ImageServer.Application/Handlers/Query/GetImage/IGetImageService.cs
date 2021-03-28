using System.Threading;
using System.Threading.Tasks;
using ImageServer.Application.Requests;
using SixLabors.ImageSharp;

namespace ImageServer.Application.Handlers.Query.GetImage
{
    public interface IGetImageService
    {
        bool FileExists(string filePath);
        void ConstrainSize(Image image, GetImageRequest request);
        void SetBackgroundColor(Image image, GetImageRequest request);
        void SetWaterMark(Image image, GetImageRequest request);
        Task<Image> LoadImage(string filePath, CancellationToken cancellationToken);
    }
}