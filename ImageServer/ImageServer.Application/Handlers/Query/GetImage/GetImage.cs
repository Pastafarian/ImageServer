using System;
using System.Threading;
using System.Threading.Tasks;
using ImageServer.Application.Config;
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
            private readonly IImageService _imageService;
            private readonly IImageCachingService _imageCachingService;
            private readonly IImageProcessor _imageProcessor;

            public Handler(AppSettings appSettings, GetImageRequestValidator getImageRequestValidator, IImageService imageService, 
                IImageCachingService imageCachingService, IImageProcessor imageProcessor)
            {
                _appSettings = appSettings;
                _getImageRequestValidator = getImageRequestValidator;
                _imageService = imageService;
                _imageCachingService = imageCachingService;
                _imageProcessor = imageProcessor;
            }

            public async Task<ImageResponse> Handle(Query query, CancellationToken cancellationToken)
            {
                var validationResult = await _getImageRequestValidator.ValidateAsync(query.Request, cancellationToken);

                if (!validationResult.IsValid)
                {
                    return ImageResponse.GetResponse(ResponseType.BadRequest, validationResult.ToString());
                }

                var imageInCache = _imageCachingService.TryGetValue(query.Request, out var imageResponse);

                if (imageInCache)
                {
                    return imageResponse;
                }

                if (!_imageService.FileExists($"{_appSettings.ProductImagesPath}{query.Request.FileName}"))
                {
                    return ImageResponse.GetResponse(ResponseType.NotFound);
                }
                
                imageResponse = await _imageProcessor.GetProcessedImage(query.Request, cancellationToken);
                
                _imageCachingService.Set(query.Request, imageResponse);

                return imageResponse;
            }
        }
    }

}

