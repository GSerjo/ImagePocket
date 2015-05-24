﻿using System;
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
		private bool _fullScreen;
		PhotoViewController1 _photoViewController;

		public Page(PhotoViewController1 photoViewController, int pageIndex, List<ImageEntity> images)
        {
			_photoViewController = photoViewController;
            _images = images;
            PageIndex = pageIndex;
        }

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
                new PHImageRequestOptions(), (img, info) => _imageView.Image = img);
        }
    }
}
