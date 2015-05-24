using System;
using System.Collections.Generic;
using Domain;
using Photos;
using UIKit;

namespace Dojo
{
    public class Page : UIViewController
    {
        private readonly ImageCache _imageCache = ImageCache.Instance;
        private readonly List<ImageEntity> _images;
        private UIImageView _imageView;

        public Page(int pageIndex, List<ImageEntity> images)
        {
            _images = images;
            PageIndex = pageIndex;
        }

        public int PageIndex { get; private set; }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            _imageView = new UIImageView(View.Frame)
            {
                ContentMode = UIViewContentMode.ScaleAspectFit,
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight
            };
            View.AddSubview(_imageView);
            UpdateImage();
        }

        private void UpdateImage()
        {
            ImageEntity _image = _images[PageIndex];
            PHAsset asset = _imageCache.GetAsset(_image.LocalIdentifier);
            PHImageManager.DefaultManager.RequestImageForAsset(
                asset,
                View.Frame.Size,
                PHImageContentMode.AspectFit,
                new PHImageRequestOptions(), (img, info) => _imageView.Image = img);
        }
    }
}
