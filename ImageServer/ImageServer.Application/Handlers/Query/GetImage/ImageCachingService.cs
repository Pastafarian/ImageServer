using System;
using Microsoft.Extensions.Caching.Memory;

namespace ImageServer.Application.Handlers.Query.GetImage
{
    public class ImageCachingService : IImageCachingService
    {
        private readonly IMemoryCache _memoryCache;

        public ImageCachingService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        public bool TryGetValue(GetImageRequest request, out ImageResponse imageResponse)
        {
            return _memoryCache.TryGetValue(request, out imageResponse);
        }

        public void Set(GetImageRequest request, ImageResponse imageResponse)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSize(imageResponse.Content.Length / 1024)
                .SetSlidingExpiration(TimeSpan.FromHours(24));

            _memoryCache.Set(request, imageResponse, cacheEntryOptions);
        }
    }
}