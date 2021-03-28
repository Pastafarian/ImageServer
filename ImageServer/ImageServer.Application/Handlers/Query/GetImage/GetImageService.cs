using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ImageServer.Application.Extensions;
using ImageServer.Application.Requests;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace ImageServer.Application.Handlers.Query.GetImage
{
    public class GetImageService : IGetImageService
    {
        public bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }


        public void SetBackgroundColor(Image image, GetImageRequest queryRequest)
        {
            if (string.IsNullOrWhiteSpace(queryRequest.BackgroundColor)) return;

            image.Mutate(x => x.BackgroundColor(Color.ParseHex(queryRequest.BackgroundColor)));
        }

        public void SetWaterMark(Image image, GetImageRequest queryRequest)
        {
            if (!queryRequest.WaterMarkRequested()) return;

            image.Mutate(x => x.ApplyScalingWaterMark(queryRequest.WaterMark, Color.Black));
        }


        public void ConstrainSize(Image image, GetImageRequest request)
        {
            var width = request.MaxWidth ?? image.Width;
            var height = request.MaxHeight ?? image.Height;

            image.Mutate(x => x.Resize(new ResizeOptions { Mode = ResizeMode.Max, Size = new Size(width, height) }));
        }

        public async Task<Image> LoadImage(string filePath, CancellationToken cancellationToken)
        {
            var bytes = await File.ReadAllBytesAsync(filePath, cancellationToken);

            var stream = new MemoryStream(bytes);
            var image = await Image.LoadAsync(stream);
            await stream.DisposeAsync();
            
            return image;
        }
    }
}