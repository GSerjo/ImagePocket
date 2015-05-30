using System;
using System.Collections.Generic;
using Core;
using Domain;
using Foundation;
using Photos;
using UIKit;

namespace Dojo
{
    public sealed class PhotoPage : UIViewController
    {
        private static readonly PHImageRequestOptions _imageRequestOptions = new PHImageRequestOptions();
        private readonly ImageCache _imageCache = ImageCache.Instance;
        private readonly List<ImageEntity> _images;
        private readonly PhotoViewController1 _photoViewController;
        private bool _fullScreen;
        private UIImageView _imageView;

        public PhotoPage(PhotoViewController1 photoViewController, int imageIndex, List<ImageEntity> images)
        {
            _photoViewController = photoViewController;
            _images = images;
            ImageIndex = imageIndex;
        }

        public int Id { get; set; }

        public UIImage Image
        {
            get { return _imageView.Image; }
        }

        public int ImageIndex { get; set; }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            if (_imageView != null)
            {
                _imageView.SafeDispose();
            }
        }

        public override void ViewDidLoad()
        {
            //            base.ViewDidLoad();
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

        private void OnImageTap(UITapGestureRecognizer gesture)
        {
            _fullScreen = !_fullScreen;
            _photoViewController.NavigationController.SetNavigationBarHidden(_fullScreen, false);
            _photoViewController.NavigationController.SetToolbarHidden(_fullScreen, false);
            View.BackgroundColor = _fullScreen ? UIColor.Black : UIColor.White;
        }

        private void SetImage(UIImage image, NSDictionary info)
        {
            _imageView.Image = image;
        }

        private void UpdateImage()
        {
            ImageEntity image = _images[ImageIndex];
            PHAsset asset = _imageCache.GetAsset(image.LocalIdentifier);
            PHImageManager.DefaultManager.RequestImageForAsset(
                asset,
                View.Frame.Size,
                PHImageContentMode.AspectFit,
                _imageRequestOptions, SetImage);
        }
    }
}
