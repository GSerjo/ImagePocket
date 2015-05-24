using System;
using System.Collections.Generic;
using Domain;
using Photos;
using UIKit;

namespace Dojo
{
    public class Page : UIViewController
    {
        private readonly List<ImageEntity> _images;
        private readonly ImageCache _imageCache = ImageCache.Instance;
        private readonly UIImageView _imageView;

        public Page(int pageIndex, List<ImageEntity> images)
        {
            _images = images;
            PageIndex = pageIndex;

            _imageView = new UIImageView(View.Frame)
            {
                MultipleTouchEnabled = true,
                UserInteractionEnabled = true,
                ContentMode = UIViewContentMode.ScaleAspectFit,
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight
            };
        }

        public int PageIndex { get; private set; }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var _image = _images[PageIndex];
            PHAsset asset = _imageCache.GetAsset(_image.LocalIdentifier);
            InvokeOnMainThread(() => UpdateImage(asset));

            // Perform any additional setup after loading the view, typically from a nib.
            //            this.imgView.Image = UIImage.FromFile(string.Format("images/{0}.jpg", PageIndex + 1));
            //            this.lblPageNumber.Text = string.Format("Page {0}", PageIndex + 1);
        }

        private void UpdateImage(PHAsset asset)
        {
            PHImageManager.DefaultManager.RequestImageForAsset(
                asset,
                View.Frame.Size,
                PHImageContentMode.AspectFit,
                new PHImageRequestOptions(),
                (img, info) => UIView.Transition(_imageView, 0.5, UIViewAnimationOptions.CurveLinear, () => _imageView.Image = img, null));
        }
    }
}
