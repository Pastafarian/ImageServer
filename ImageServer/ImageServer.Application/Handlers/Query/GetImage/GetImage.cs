using System;
using System.Threading;
using System.Threading.Tasks;
using ImageServer.Application.Config;
using ImageServer.Application.Handlers.Query.GetImage.ImageSavingStrategy;
using ImageServer.Application.Requests;
using MediatR;

namespace ImageServer.Application.Handlers.Query.GetImage
{
    public class GetImage
    {
        public class Query : IRequest<ImageResponse>
        {
            public readonly GetImageRequest Request;

            public Query(GetImageRequest request)
            {
                Request = request;
            }
        }

        public class Handler : IRequestHandler<Query, ImageResponse>
        {
            private readonly AppSettings _appSettings;
            private readonly GetImageRequestValidator _getImageRequestValidator;
            private readonly IGetImageService _getImageService;
            private readonly IImageSavingStrategy _imageSavingStrategy;

            public Handler(AppSettings appSettings, GetImageRequestValidator getImageRequestValidator, IGetImageService getImageService, IImageSavingStrategy imageSavingStrategy)
            {
                _appSettings = appSettings;
                _getImageRequestValidator = getImageRequestValidator;
                _getImageService = getImageService;
                _imageSavingStrategy = imageSavingStrategy;
            }

            public async Task<ImageResponse> Handle(Query query, CancellationToken cancellationToken)
            {
                var validationResult = await _getImageRequestValidator.ValidateAsync(query.Request, cancellationToken);

                if (!validationResult.IsValid) return ImageResponse.GetResponse(ResponseType.BadRequest, validationResult.ToString());
                
                var filePath = $"{_appSettings.ProductImagesPath}{query.Request.FileName}";

                if (!_getImageService.FileExists(filePath)) return ImageResponse.GetResponse(ResponseType.NotFound);
                
                try
                {
                    return await GetImageResponse(query, filePath, cancellationToken);
                    
                }
                catch (Exception)
                {
                    return ImageResponse.GetResponse(ResponseType.ServerError);
                }
            }

            private async Task<ImageResponse> GetImageResponse(Query query, string filePath, CancellationToken cancellationToken)
            {
                var image = await _getImageService.LoadImage(filePath, cancellationToken);
                _getImageService.ConstrainSize(image, query.Request);
                _getImageService.SetBackgroundColor(image, query.Request);
                _getImageService.SetWaterMark(image, query.Request);
                var imageBytes = await _imageSavingStrategy.SaveImage(image, query.Request.ImageFileType, cancellationToken);
                return ImageResponse.GetResponse(ResponseType.Ok, imageBytes);
            }
        }
    }
}

