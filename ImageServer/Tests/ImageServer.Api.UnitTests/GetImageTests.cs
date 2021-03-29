using System;
using System.Threading;
using System.Threading.Tasks;
using ImageServer.Application.Config;
using ImageServer.Application.Enums;
using ImageServer.Application.Handlers.Query.GetImage;
using Moq;
using Xunit;

namespace ImageServer.Api.UnitTests
{
    public class GetImageTests
    {
        private readonly GetImage.Handler _sut;
        private readonly Mock<IImageService> _mockImageService;
        private readonly Mock<IImageCachingService> _mockImageCachingService;
        private readonly Mock<IImageProcessor> _mockImageProcessor;

        public GetImageTests()
        {
            var appSettings = new AppSettings();

            _mockImageService = new Mock<IImageService>();
            _mockImageService.Setup(x => x.FileExists(It.IsAny<string>())).Returns(true);
            _mockImageCachingService = new Mock<IImageCachingService>();
            _mockImageProcessor = new Mock<IImageProcessor>();
            _mockImageProcessor.Setup(x =>
                    x.GetProcessedImage(It.IsAny<GetImageRequest>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new ImageResponse()));

            _sut = new GetImage.Handler(appSettings, new GetImageRequestValidator(), _mockImageService.Object, _mockImageCachingService.Object, _mockImageProcessor.Object);
        }

        [Theory]
        [InlineData("imagefile.png", null, null, "#000000", false)] // Fails if neight max width or max height set (resolution required from spec)
        [InlineData("imagefile.png", 100, -1, "#000000", false)] // Max height must be greater than 0
        [InlineData("imagefile.png", -1, 100, "#000000", false)] // Max width must be greater than 0
        [InlineData("imagefile.png", 200, 200, "invalidhex", false)] // Background must be valid hex
        [InlineData("", 200, 200, "#000000", false)] // Must have file name
        [InlineData("imagefile.png", 200,200, "#000000", true)]
        [InlineData("imagefile.png", 200, 200, "#32a852", true)]
        public async Task GetImage_ValidatesRequest(string fileName, int? maxWith, int? maxHeight, string backgroundColor, bool isValid)
        {
            // Arrange
            var request = new GetImageRequest
            {
                FileName = fileName, MaxWidth = maxWith, MaxHeight = maxHeight, BackgroundColor = backgroundColor
            };

            // Act
            var result = await _sut.Handle(new GetImage.Query(request), CancellationToken.None);

            // Assert
            if (isValid)
            {
                Assert.True(result.ResponseType == ResponseType.Ok);
            }
            else
            {
                Assert.True(result.ResponseType == ResponseType.BadRequest);
            }
        }



        [Fact]
        public async Task GetImage_WhenFileNotFound_NotFoundResultReturned()
        {
            // Arrange
            var request = GetValidRequest();
            _mockImageService.Setup(x => x.FileExists(It.IsAny<string>())).Returns(false);

            // Act
            var result = await _sut.Handle(new GetImage.Query(request), CancellationToken.None);

            // Assert
            Assert.Equal(ResponseType.NotFound, result.ResponseType);
        }

        [Fact]
        public async Task GetImage_WhenExceptionThrownLoadingImage_BadRequestReturned()
        {
            // Arrange
            var request = GetValidRequest();
            _mockImageProcessor.Setup(x => x.GetProcessedImage(It.IsAny<GetImageRequest>(), It.IsAny<CancellationToken>())).Throws(new Exception());
            
            // Act
            var result = await _sut.Handle(new GetImage.Query(request), CancellationToken.None);

            // Assert
            Assert.Equal(ResponseType.ServerError, result.ResponseType);
        }

        private static GetImageRequest GetValidRequest()
        {
            return new()
            {
                FileName = "image.png",
                BackgroundColor = "#000000",
                ImageFileType = ImageFileType.Png,
                MaxHeight = 300,
                MaxWidth = 300,
                WaterMark = ""
            };
        }
    }
}
