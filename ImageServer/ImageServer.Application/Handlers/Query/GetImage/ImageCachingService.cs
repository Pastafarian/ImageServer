using System;
using ImageServer.Application.Config;
using Microsoft.Extensions.Caching.Memory;

namespace ImageServer.Application.Handlers.Query.GetImage
{
    public class ImageCachingService : IImageCachingService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly AppSettings _appSettings;

        public ImageCachingService(IMemoryCache memoryCache, AppSettings appSettings)
        {
            _memoryCache = memoryCache;
            _appSettings = appSettings;
        }
        public bool TryGetValue(GetImageRequest request, out ImageResponse imageResponse)
        {
            return _memoryCache.TryGetValue(request, out imageResponse);
        }

        public void Set(GetImageRequest request, ImageResponse imageResponse)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSize(imageResponse.Content.Length / 1024)
                .SetSlidingExpiration(TimeSpan.FromHours(_appSettings.CacheExpiryInHours));

            _memoryCache.Set(request, imageResponse, cacheEntryOptions);
        }
    }
}