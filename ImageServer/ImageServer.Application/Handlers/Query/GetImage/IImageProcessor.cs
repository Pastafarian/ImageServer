using System.Threading;
using System.Threading.Tasks;

namespace ImageServer.Application.Handlers.Query.GetImage
{
    public interface IImageProcessor
    {
        Task<ImageResponse> GetProcessedImage(GetImageRequest request, CancellationToken cancellationToken);
    }
}