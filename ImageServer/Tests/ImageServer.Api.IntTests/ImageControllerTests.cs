using System;
using System.IO;
using System.Threading.Tasks;
using ImageServer.Api.Controllers;
using ImageServer.Application.Config;
using ImageServer.Application.Enums;
using ImageServer.Application.Handlers.Query.GetImage;
using ImageServer.Application.Handlers.Query.GetImage.ImageSavingStrategy;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SixLabors.ImageSharp;
using Xunit;

namespace ImageServer.Api.IntTests
{
    public class ImageControllerTests
    {
        private readonly ImageController _sut;

        public ImageControllerTests()
        {
            IServiceCollection serviceCollection = new ServiceCollection();

            var appSettings = new AppSettings
            {
                ProductImagesPath = @"C:\Assets\product_images\\"
            };

            serviceCollection.AddSingleton(appSettings);
            serviceCollection.AddMediatR(typeof(GetImage));
            serviceCollection.AddMemoryCache();
            serviceCollection.AddScoped<IImageService, ImageService>();
            serviceCollection.AddScoped<IImageCachingService, ImageCachingService>();
            serviceCollection.AddScoped<IImageSavingStrategy, ImageSavingStrategy>();
            serviceCollection.AddScoped<IImageSaver, JpgSaver>();
            serviceCollection.AddScoped<IImageSaver, PngSaver>();
            serviceCollection.AddScoped<IImageProcessor, ImageProcessor>();
            serviceCollection.AddScoped<GetImageRequestValidator, GetImageRequestValidator>();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            _sut = new ImageController(serviceProvider.GetRequiredService<IMediator>());
        }

        [Fact]
        public async Task GivenInvalidImageName_WhenNoParamsSet_ThenNotFoundResultReturned()
        {
            // Arrange
            var request = new GetImageRequest { FileName = "file-missing.png", MaxWidth = 300};

            // Act
            var result = await _sut.Index(request);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GivenValidImageName_WhenNoParamsSet_ThenMissingResolutionErrorReturned()
        {
            // Arrange
            var request = new GetImageRequest { FileName = "file-missing.png" };

            // Act
            var result = await _sut.Index(request);

            // Verify type
            Assert.IsType<BadRequestObjectResult>(result);

            var message = ((BadRequestObjectResult) result).Value.ToString();

            Assert.Contains("Max with or max height must be specified", message);
        }

        [Fact]
        public async Task GivenValidImageName_WhenMaxWidthSet_ThenImageConstrainedToMaxWith()
        {
            // Arrange
            const int maxWidth = 200;
            var request = new GetImageRequest { FileName = "01_04_2019_001106.png", MaxWidth = maxWidth };


            // Act
            var result = await _sut.Index(request);

            // Verify type
            Assert.IsType<FileContentResult>(result);

            // Load image
            var (image, errorLoading) = await LoadImage(result);

            Assert.False(errorLoading);

            Assert.Equal(maxWidth, image.Width);
        }

        [Fact]
        public async Task GivenValidImageName_WhenMaxHeightSet_ThenImageConstrainedToMaxHeight()
        {
            // Arrange
            const int maxHeight = 100;
            var request = new GetImageRequest { FileName = "01_04_2019_001106.png", MaxHeight = maxHeight };


            // Act
            var result = await _sut.Index(request);

            // Verify type
            Assert.IsType<FileContentResult>(result);

            // Load image
            var (image, errorLoading) = await LoadImage(result);

            Assert.False(errorLoading);

            Assert.Equal(maxHeight, image.Height);
        }

        [Fact]
        public async Task GivenValidImageName_WhenWaterMarkSet_ThenValidImageReturned()
        {
            // Arrange
            const int maxHeight = 100;
            var request = new GetImageRequest { FileName = "01_04_2019_001106.png", MaxHeight = maxHeight, WaterMark = "water mark"};

            // Act
            var result = await _sut.Index(request);

            // Verify type
            Assert.IsType<FileContentResult>(result);

            // Load image
            var (image, errorLoading) = await LoadImage(result);

            Assert.False(errorLoading);

            Assert.Equal(maxHeight, image.Height);
        }

        [Fact]
        public async Task GivenValidImageName_WhenJpgImageSpecified_ThenValidImageReturned()
        {
            // Arrange
            const int maxHeight = 100;
            var request = new GetImageRequest { FileName = "01_04_2019_001106.png", MaxHeight = maxHeight, ImageFileType = ImageFileType.Jpg };

            // Act
            var result = await _sut.Index(request);

            // Verify type
            Assert.IsType<FileContentResult>(result);

            // Load image
            var (image, errorLoading) = await LoadImage(result);

            Assert.False(errorLoading);

            Assert.Equal(maxHeight, image.Height);
        }

        [Fact]
        public async Task GivenValidImageName_WhenBackgroundColourSpecified_ThenValidImageReturned()
        {
            // Arrange
            const int maxHeight = 100;
            var request = new GetImageRequest { FileName = "01_04_2019_001106.png", MaxHeight = maxHeight, BackgroundColor = "#000000"};

            // Act
            var result = await _sut.Index(request);

            // Verify type
            Assert.IsType<FileContentResult>(result);

            // Load image
            var (image, errorLoading) = await LoadImage(result);

            Assert.False(errorLoading);

            Assert.Equal(maxHeight, image.Height);
        }

        private static async Task<(Image image, bool errorLoading)> LoadImage(IActionResult result)
        {
            Image image;

            try
            {
                var fileContentResult = (FileContentResult)result;
                var stream = new MemoryStream(fileContentResult.FileContents);
                image = await Image.LoadAsync(stream);
                await stream.DisposeAsync();
            }
            catch (Exception)
            {
                return (null, true);
            }

            return (image, false);
        }
    }
}
