namespace ImageServer.Application.Config
{
    public class AppSettings
    {
        public string ProductImagesPath { get; set; }
        public long MaximumCacheSizeInKb { get; set; }
        public int CacheExpiryInHours { get; set; }
    }
}