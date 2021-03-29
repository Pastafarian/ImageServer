using ImageServer.Application.Enums;

namespace ImageServer.Application.Handlers.Query.GetImage
{
    public record GetImageRequest
    {
        public string FileName { get; set; }
        public int? MaxWidth { get; set; }
        public int? MaxHeight { get; set; }
        public string BackgroundColor { get; set; }
        public string WaterMark { get; set; }
        public ImageFileType ImageFileType { get; set; } = ImageFileType.Default;

        public bool WaterMarkRequested()
        {
            return !string.IsNullOrWhiteSpace(WaterMark);
        }

    }
}