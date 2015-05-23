using System;
using System.Collections.Generic;
using Domain;
using Photos;
using UIKit;

namespace Dojo
{
    public sealed class PhotoScrollView : ImageScrollView
    {
        private readonly ImageCache _imageCache = ImageCache.Instance;
        private readonly List<ImageEntity> _images;

        public PhotoScrollView(List<ImageEntity> images)
        {
            _images = images;
            AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
        }

        protected override void DisplayImage(int imageIndex)
        {
            ImageEntity image = _images[imageIndex];
            PHAsset asset = _imageCache.GetAsset(image.LocalIdentifier);

            PHImageManager.DefaultManager.RequestImageForAsset(asset, Frame.Size,
                PHImageContentMode.AspectFit, new PHImageRequestOptions(), (img, info) => DisplayImage(img));
        }
    }
}
