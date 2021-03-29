namespace ImageServer.Application.Handlers.Query.GetImage
{
    public interface IImageCachingService
    {
        bool TryGetValue(GetImageRequest request, out ImageResponse imageResponse);
        void Set(GetImageRequest request, ImageResponse imageResponse);
    }
}