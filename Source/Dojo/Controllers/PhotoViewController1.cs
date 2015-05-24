using System;
using System.Collections.Generic;
using Domain;
using UIKit;

namespace Dojo
{
    public sealed class PhotoViewController1 : UIViewController
    {
        private readonly int _currentImageIndex;
        private readonly List<ImageEntity> _images;
        private UIPageViewController _pageViewController;

        public PhotoViewController1(ImageEntity image, List<ImageEntity> images)
        {
            _images = images;
            _currentImageIndex = _images.FindIndex(x => x.Equals(image));
        }

        public override void ViewDidLoad()
        {
            var firstPage = new Page(_currentImageIndex, _images);
            _pageViewController = new UIPageViewController(
                UIPageViewControllerTransitionStyle.Scroll,
                UIPageViewControllerNavigationOrientation.Horizontal,
                UIPageViewControllerSpineLocation.None, 10f)
            {
                GetNextViewController = GetNextViewController,
                GetPreviousViewController = GetPreviousViewController,
                View = { Frame = View.Bounds }
            };

            _pageViewController.SetViewControllers(
                new UIViewController[] { firstPage },
                UIPageViewControllerNavigationDirection.Forward,
                false, x => { });

            View.AddSubview(_pageViewController.View);
        }

        private UIViewController GetNextViewController(UIPageViewController pageController, UIViewController referenceViewController)
        {
            var currentPageController = referenceViewController as Page;

            if (currentPageController.PageIndex >= (_images.Count - 1))
            {
                return null;
            }
            else
            {
                int nextPageIndex = currentPageController.PageIndex + 1;

                return new Page(nextPageIndex, _images);
            }
        }

        private UIViewController GetPreviousViewController(UIPageViewController pageController, UIViewController referenceViewController)
        {
            var currentPageController = referenceViewController as Page;
            if (currentPageController.PageIndex <= 0)
            {
                return null;
            }
            else
            {
                int previousPageIndex = currentPageController.PageIndex - 1;

                return new Page(previousPageIndex, _images);
            }
        }
    }
}
