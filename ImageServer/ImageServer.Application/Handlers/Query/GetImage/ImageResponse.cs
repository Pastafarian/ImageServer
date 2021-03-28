namespace ImageServer.Application.Handlers.Query.GetImage
{
    public class ImageResponse
    {
        public ResponseType ResponseType { get; set; }
        public string Message { get; set; }
        public byte[] Content { get; set; }

        public static ImageResponse GetResponse(ResponseType responseType, string message = "") 
        {
            return new() { ResponseType = responseType, Message = message};
        }

        public static ImageResponse GetResponse(ResponseType responseType, byte[] content)
        {
            return new() { ResponseType = responseType, Content = content };
        }
    }
}