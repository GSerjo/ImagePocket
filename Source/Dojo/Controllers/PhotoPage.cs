using System;
using System.Collections.Generic;
using Core;
using Domain;
using Photos;
using UIKit;

namespace Dojo
{
    public sealed class PhotoPage : UIViewController
    {
        private static readonly PHImageRequestOptions _imageRequestOptions = new PHImageRequestOptions
        {
            Synchronous = false,
            NetworkAccessAllowed = true,
        };

        private readonly ImageCache _imageCache = ImageCache.Instance;
        private readonly List<ImageEntity> _images;
        private readonly WeakReference<PhotoViewController1> _photoViewController;
        //        private readonly PhotoViewController1 _photoViewController;
        private bool _fullScreen;
        private UIImageView _imageView;

        public PhotoPage(PhotoViewController1 photoViewController, int imageIndex, List<ImageEntity> images)
        {
            //            _photoViewController = photoViewController;
            _photoViewController = new WeakReference<PhotoViewController1>(photoViewController);
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
                _imageView.Image = null;
                _imageView.SafeDispose();
                _imageView.RemoveFromSuperview();
                _imageView = null;
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            if (_imageView != null)
            {
                _imageView.RemoveFromSuperview();
                _imageView = null;
            }

            _imageView = new UIImageView(View.Frame)
            {
                MultipleTouchEnabled = true,
                UserInteractionEnabled = true,
                ContentMode = UIViewContentMode.ScaleAspectFit,
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight
            };

            var tapGesture = new UITapGestureRecognizer(OnImageTap);
            View.AddGestureRecognizer(tapGesture);
            View.AddSubview(_imageView);

            UpdateImage();
        }

        //        public override void ViewWillAppear(bool animated)
        //        {
        //            //			base.ViewWillAppear (animated);
        //            _imageView = new UIImageView(View.Frame)
        //            {
        //                MultipleTouchEnabled = true,
        //                UserInteractionEnabled = true,
        //                ContentMode = UIViewContentMode.ScaleAspectFit,
        //                AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight
        //            };
        //            var tapGesture = new UITapGestureRecognizer(OnImageTap);
        //            _imageView.AddGestureRecognizer(tapGesture);
        //            View.AddSubview(_imageView);
        //
        //            UpdateImage();
        //        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (_imageView != null)
            {
                _imageView.SafeDispose();
                _imageView.RemoveFromSuperview();
                _imageView = null;
            }
        }

        private void OnImageTap(UITapGestureRecognizer gesture)
        {
            PhotoViewController1 controller;
            if (_photoViewController.TryGetTarget(out controller))
            {
                _fullScreen = !_fullScreen;
                controller.NavigationController.SetNavigationBarHidden(_fullScreen, false);
                controller.NavigationController.SetToolbarHidden(_fullScreen, false);
                View.BackgroundColor = _fullScreen ? UIColor.Black : UIColor.White;
            }
        }

        private void SetImage(UIImage image)
        {
            if (_imageView == null)
            {
                return;
            }

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
                _imageRequestOptions, (img, y) => { _imageView.Image = img; });
        }
    }
}
