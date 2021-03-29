using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ImageServer.Application.Config;
using ImageServer.Application.Handlers.Query.GetImage.ImageSavingStrategy;

namespace ImageServer.Application.Handlers.Query.GetImage
{
    public class ImageProcessor : IImageProcessor
    {
        private readonly IImageService _imageService;
        private readonly AppSettings _appSettings;
        private readonly IImageSavingStrategy _imageSavingStrategy;

        public ImageProcessor(IImageService imageService, AppSettings appSettings, IImageSavingStrategy imageSavingStrategy)
        {
            _imageService = imageService;
            _appSettings = appSettings;
            _imageSavingStrategy = imageSavingStrategy;
        }

        public async Task<ImageResponse> GetProcessedImage(GetImageRequest request, CancellationToken cancellationToken)
        {
            var image = await _imageService.LoadImage($"{_appSettings.ProductImagesPath}{request.FileName}", cancellationToken);

            _imageService.ConstrainSize(image, request);
            _imageService.SetBackgroundColor(image, request);
            _imageService.SetWaterMark(image, request);

            var (bytes, contentType) = await _imageSavingStrategy.SaveImage(image, request.ImageFileType, cancellationToken);

            return ImageResponse.GetResponse(ResponseType.Ok, bytes, contentType);
        }
    }
}