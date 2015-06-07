using System;
using System.Collections.Generic;
using Domain;
using UIKit;

namespace Dojo
{
    public sealed class PhotoViewController3 : UIViewController
    {
		private ImageScrollView _scrollView = new ImageScrollView
		{
			AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight,
		};

        private PhotoViewController3(ImageEntity image)
        {
            ImageEntity = image;
            Title = "Image";
        }

        public ImageEntity ImageEntity { get; private set; }

        public static PhotoViewController3 ImageViewControllerForPageIndex(ImageEntity image)
        {
            return new PhotoViewController3(image);
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
			View.Dispose ();
			_scrollView = null;
		}
    }


    public class PhotoViewController4 : UIPageViewController
    {
        private static List<ImageEntity> _images;

        public PhotoViewController4(ImageEntity image, List<ImageEntity> images)
            : base(UIPageViewControllerTransitionStyle.Scroll,
                UIPageViewControllerNavigationOrientation.Horizontal,
                UIPageViewControllerSpineLocation.None, 20f)
        {
            _images = images;

            PhotoViewController3 pageZero = PhotoViewController3.ImageViewControllerForPageIndex(image);

            SetViewControllers(new UIViewController[] { pageZero },
                UIPageViewControllerNavigationDirection.Forward,
                false, null);
            DataSource = new MyDataSource();
        }

        private static int GetIndex(ImageEntity image)
        {
            return _images.FindIndex(x => x.Equals(image));
        }


        private sealed class MyDataSource : UIPageViewControllerDataSource
        {
            public override UIViewController GetNextViewController(UIPageViewController pageViewController,
                UIViewController referenceViewController)
            {
                ImageEntity imageEntity = ((PhotoViewController3)referenceViewController).ImageEntity;
                int index = GetIndex(imageEntity) + 1;
                return index >= 0 && index < _images.Count ?
                    PhotoViewController3.ImageViewControllerForPageIndex(_images[index]) : null;
            }

            public override UIViewController GetPreviousViewController(UIPageViewController pageViewController,
                UIViewController referenceViewController)
            {
                ImageEntity imageEntity = ((PhotoViewController3)referenceViewController).ImageEntity;
                int index = GetIndex(imageEntity) - 1;
                return index >= 0 && index < _images.Count ?
                    PhotoViewController3.ImageViewControllerForPageIndex(_images[index]) : null;
            }
        }
    }
}
