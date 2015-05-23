using System;
using System.Collections.Generic;
using Domain;
using UIKit;

namespace Dojo
{
    public sealed class PageViewController : UIPageViewController
    {
        private static List<ImageEntity> _images;

        public PageViewController(ImageEntity currentImage, List<ImageEntity> images)
            : base(UIPageViewControllerTransitionStyle.Scroll,
                UIPageViewControllerNavigationOrientation.Horizontal,
                UIPageViewControllerSpineLocation.None, 20f)
        {
            _images = images;
            int currentImageIndex = _images.FindIndex(x => x.Equals(currentImage));

            DataSource = new MyDataSource();

            ImageViewController currentViewController = ImageViewController.FromPageIndex(currentImageIndex);
            SetViewControllers(new UIViewController[] { currentViewController }, UIPageViewControllerNavigationDirection.Forward, false, null);
        }


        private sealed class ImageViewController : UIViewController
        {
            private ImageViewController(int pageIndex)
            {
                PageIndex = pageIndex;
            }

            public int PageIndex { get; private set; }

            public static ImageViewController FromPageIndex(int pageIndex)
            {
                if (pageIndex >= 0 && pageIndex < _images.Count)
                {
                    return new ImageViewController(pageIndex);
                }
                else
                {
                    return null;
                }
            }

            public override void LoadView()
            {
                var scrollView = new PhotoScrollView(_images)
                {
                    Index = PageIndex,
                };

                View = scrollView;
            }
        }


        private sealed class MyDataSource : UIPageViewControllerDataSource
        {
            public override UIViewController GetNextViewController(UIPageViewController pageViewController,
                UIViewController referenceViewController)
            {
                int index = ((ImageViewController)referenceViewController).PageIndex;
                return ImageViewController.FromPageIndex(index + 1);
            }

            public override UIViewController GetPreviousViewController(UIPageViewController pageViewController,
                UIViewController referenceViewController)
            {
                int index = ((ImageViewController)referenceViewController).PageIndex;
                return ImageViewController.FromPageIndex(index - 1);
            }
        }
    }
}
