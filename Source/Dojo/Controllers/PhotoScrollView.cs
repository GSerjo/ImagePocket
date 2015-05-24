using System;
using System.Collections.Generic;
using Domain;
using Photos;
using UIKit;
using CoreGraphics;

namespace Dojo
{
    public sealed class PhotoScrollView : ImageScrollView
    {
        private readonly ImageCache _imageCache = ImageCache.Instance;
        private readonly List<ImageEntity> _images;
		private readonly PHImageRequestOptions _imageRequestOptions = new PHImageRequestOptions ();
		private readonly UIScreen _mainScreen = UIScreen.MainScreen;

        public PhotoScrollView(List<ImageEntity> images)
        {
            _images = images;
            AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
        }

        protected override void DisplayImage(int imageIndex)
        {
            ImageEntity image = _images[imageIndex];
            PHAsset asset = _imageCache.GetAsset(image.LocalIdentifier);
			var targetImageSize = GetTargetImageSize ();
			var imageView = new UIImageView (new CGRect (new CGPoint (), targetImageSize));

			PHImageManager.DefaultManager.RequestImageForAsset(asset, targetImageSize,
				PHImageContentMode.AspectFit, _imageRequestOptions, (img, info) => 
			{
				imageView.Image = img;
				imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
				DisplayImage(imageView);
//				DisplayImage(img);
			});
        }

		private CGSize GetTargetImageSize()
		{
			CGSize mainSize = _mainScreen.Bounds.Size;
			var result = new CGSize (mainSize.Width * _mainScreen.Scale, mainSize.Height * _mainScreen.Scale);
			return result;
		}
    }
}
