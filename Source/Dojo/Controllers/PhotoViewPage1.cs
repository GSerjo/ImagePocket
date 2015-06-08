using System;
using Domain;
using UIKit;
using Core;

namespace Dojo
{
    public sealed class PhotoViewPage1 : UIViewController
    {
        private ImageScrollView _scrollView = new ImageScrollView
        {
            AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight,
        };

        private PhotoViewPage1(ImageEntity image)
        {
            ImageEntity = image;
            Title = "Image";
        }

        public ImageEntity ImageEntity { get; private set; }

        public static PhotoViewPage1 ImageViewControllerForPageIndex(ImageEntity image)
        {
            return new PhotoViewPage1(image);
        }

        public override void LoadView()
        {
            //            var scrollView = new ImageScrollView
            //            {
            //                ImageEntity = ImageEntity,
            //                AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight,
            //            };
            //			View = scrollView;

//            _scrollView.ImageEntity = ImageEntity;
//            View = _scrollView;
        }

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			_scrollView.ImageEntity = ImageEntity;
			View = _scrollView;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
//			if (_scrollView != null)
//			{
//				return;
//			}
//
//			_scrollView = new ImageScrollView
//			{
//				AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight,
//			};
//			_scrollView.ImageEntity = ImageEntity;
//
//			_scrollView.ImageEntity = ImageEntity;
//			View = _scrollView;
			_scrollView.ResetImage();
		}

        public override void ViewDidDisappear(bool animated)
        {
//            base.ViewDidDisappear(animated);
			_scrollView.ReleaseResources ();
//			_scrollView.ImageEntity = null;
//			_scrollView = null;
//			View.SafeDispose ();
        }
    }
}