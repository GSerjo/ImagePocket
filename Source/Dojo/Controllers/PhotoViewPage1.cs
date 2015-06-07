using System;
using Domain;
using UIKit;

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

            _scrollView.ImageEntity = ImageEntity;
            View = _scrollView;
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            View.Dispose();
            _scrollView = null;
        }
    }
}