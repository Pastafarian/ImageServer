using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ImageServer.Application.Enums;
using SixLabors.ImageSharp;

namespace ImageServer.Application.Handlers.Query.GetImage.ImageSavingStrategy
{
    public class ImageSavingStrategy : IImageSavingStrategy
    {
        private readonly IEnumerable<IImageSaver> _imageSavers;

        public ImageSavingStrategy(IEnumerable<IImageSaver> imageSavers)
        {
            _imageSavers = imageSavers;
        }

        public Task<byte[]> SaveImage(Image image, ImageFileType imageFileType, CancellationToken cancellationToken)
        {
            var imageSaver = _imageSavers.SingleOrDefault(x => x.ImageFileType(imageFileType));

            if (imageSaver == null) throw new Exception($"Unable to find image saving strategy for image file type {imageFileType}.");

            return imageSaver.SaveImage(image, cancellationToken);
        }
    }
}
