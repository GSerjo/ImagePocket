using System;
using System.Collections.Generic;
using Domain;
using Photos;
using UIKit;

namespace Dojo
{
    public sealed class PhotoPage : UIViewController
    {
        private readonly ImageCache _imageCache = ImageCache.Instance;
        private readonly List<ImageEntity> _images;
        private readonly PhotoViewController1 _photoViewController;
        private bool _fullScreen;
        private UIImageView _imageView;

        public PhotoPage(PhotoViewController1 photoViewController, int pageIndex, List<ImageEntity> images)
        {
            _photoViewController = photoViewController;
            _images = images;
            PageIndex = pageIndex;
        }

        public UIImage Image { get; private set; }
        public int PageIndex { get; private set; }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            _imageView = new UIImageView(View.Frame)
            {
                MultipleTouchEnabled = true,
                UserInteractionEnabled = true,
                ContentMode = UIViewContentMode.ScaleAspectFit,
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight
            };
            var tapGesture = new UITapGestureRecognizer(OnImageTap);
            _imageView.AddGestureRecognizer(tapGesture);

            View.AddSubview(_imageView);
            UpdateImage();
        }

		public void SetImage(ImageEntity image)
		{
			var imageInadex = _images.FindIndex(x => x.Equals(image));
			PageIndex = imageInadex;
			PHAsset asset = _imageCache.GetAsset(image.LocalIdentifier);
			InvokeOnMainThread(() => UpdateImage(asset));
		}

		private void UpdateImage(PHAsset asset)
		{
			PHImageManager.DefaultManager.RequestImageForAsset(asset, View.Frame.Size,
				PHImageContentMode.AspectFit, new PHImageRequestOptions(), (img, info) =>
			{
				UIView.Transition(_imageView, 0.3, UIViewAnimationOptions.CurveLinear, () => _imageView.Image = img, null);
			});
		}

        private void OnImageTap(UITapGestureRecognizer gesture)
        {
            _fullScreen = !_fullScreen;
            _photoViewController.NavigationController.SetNavigationBarHidden(_fullScreen, false);
            _photoViewController.NavigationController.SetToolbarHidden(_fullScreen, false);
            View.BackgroundColor = _fullScreen ? UIColor.Black : UIColor.White;
        }

        private void UpdateImage()
        {
            ImageEntity _image = _images[PageIndex];
            PHAsset asset = _imageCache.GetAsset(_image.LocalIdentifier);
            PHImageManager.DefaultManager.RequestImageForAsset(
                asset,
                View.Frame.Size,
                PHImageContentMode.AspectFit,
                new PHImageRequestOptions(), (img, info) =>
                {
                    _imageView.Image = img;
                    Image = img;
                });
        }
    }
}
