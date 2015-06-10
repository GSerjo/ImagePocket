using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Domain;
using Foundation;
using Photos;
using UIKit;

namespace Dojo
{
    public sealed class PhotoViewController1 : UIViewController
    {
        private readonly UIBarButtonItem _btShare;
        private readonly int _currentImageIndex;
        private readonly ImageCache _imageCache = ImageCache.Instance;
        private readonly List<ImageEntity> _images;
        private MyPageViewController _pageViewController;
        //        private UIPageViewControllerDataSource _pageViewDataSource;
        private UIPopoverController _shareController;

        public PhotoViewController1(ImageEntity image, List<ImageEntity> images)
        {
            _images = images;
            _currentImageIndex = GetImageIndex(image);

            var tabButton = new UIBarButtonItem("Tag", UIBarButtonItemStyle.Plain, OnTagClicked);
            NavigationItem.RightBarButtonItem = tabButton;

            _btShare = new UIBarButtonItem(UIBarButtonSystemItem.Action, OnShareClicked);
            var btTrash = new UIBarButtonItem(UIBarButtonSystemItem.Trash, OnTrashClicked);
            var deleteSpace = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
            ToolbarItems = new[] { _btShare, deleteSpace, btTrash };
        }

        public override void ViewDidLoad()
        {
            _pageViewController = new MyPageViewController(this);
            //            {
            //                GetNextViewController = GetNextViewController,
            //                GetPreviousViewController = GetPreviousViewController,
            //            };
            //            _pageViewDataSource = new MyDataSource(this);
            //            _pageViewController.DataSource = _pageViewDataSource;

            var firstPage = new PhotoPage(this, _currentImageIndex, _images);
            _pageViewController.SetViewControllers(new UIViewController[] { firstPage },
                UIPageViewControllerNavigationDirection.Forward,
                false, null);

            _pageViewController.View.Frame = View.Bounds;
            View.AddSubview(_pageViewController.View);
        }

        private Option<ImageEntity> GetCurrentImage()
        {
            var viewController = (PhotoPage)_pageViewController.ViewControllers.FirstOrDefault();
            if (viewController == null)
            {
                return Option<ImageEntity>.Empty;
            }

            int pageIndex = viewController.ImageIndex;
            ImageEntity image = _images[pageIndex];
            return image.ToOption();
        }

        private int GetImageIndex(ImageEntity image)
        {
            return _images.FindIndex(x => x.Equals(image));
        }

        private UIViewController GetNextViewController(UIPageViewController pageController, UIViewController referenceViewController)
        {
            var currentPageController = referenceViewController as PhotoPage;

            if (currentPageController.ImageIndex >= (_images.Count - 1))
            {
                return null;
            }
            else
            {
                int nextPageIndex = currentPageController.ImageIndex + 1;

                return new PhotoPage(this, nextPageIndex, _images);
            }
        }

        private UIViewController GetPreviousViewController(UIPageViewController pageController, UIViewController referenceViewController)
        {
            var currentPageController = referenceViewController as PhotoPage;
            if (currentPageController.ImageIndex <= 0)
            {
                return null;
            }
            else
            {
                int previousPageIndex = currentPageController.ImageIndex - 1;

                return new PhotoPage(this, previousPageIndex, _images);
            }
        }

        private void OnDeleteAssetsCompleted(ImageEntity removedImage, bool result, NSError error)
        {
            if (result == false)
            {
                return;
            }
            else
            {
                Console.WriteLine(error);
            }
            int imageIndex = GetImageIndex(removedImage);
            _images.Remove(removedImage);
            _imageCache.Remove(removedImage);

            InvokeOnMainThread(() =>
            {
                _pageViewController.ResetDataSource();

                if (_images.IsNullOrEmpty())
                {
                    NavigationController.PopViewController(true);
                    return;
                }
                var page = new PhotoPage(this, _currentImageIndex, _images);

                imageIndex--;
                if (imageIndex < 0)
                {
                    imageIndex = 0;
                }
                _pageViewController.SetViewControllers(new UIViewController[] { page },
                    UIPageViewControllerNavigationDirection.Forward,
                    false, null);
            });
        }

        private void OnShareClicked(object sender, EventArgs ea)
        {
            if (_shareController == null)
            {
                var viewController = (PhotoPage)_pageViewController.ViewControllers.FirstOrDefault();
                if (viewController == null)
                {
                    return;
                }
                var activityController = new UIActivityViewController(new NSObject[] { viewController.Image }, null);
                _shareController = new UIPopoverController(activityController);
                _shareController.DidDismiss += (s, e) => _shareController = null;
                _shareController.PresentFromBarButtonItem(_btShare, UIPopoverArrowDirection.Up, true);
            }
            else
            {
                _shareController.Dismiss(true);
                _shareController = null;
            }
        }

        private void OnTagClicked(object sender, EventArgs ea)
        {
            Option<ImageEntity> currentImage = GetCurrentImage();
            if (currentImage.HasNoValue)
            {
                return;
            }
            var controller = new TagSelectorViewController(currentImage.Value)
            {
                ModalPresentationStyle = UIModalPresentationStyle.FormSheet
            };
            controller.Done += OnTagSelectorDone;
            NavigationController.PresentViewController(controller, true, null);
        }

        private void OnTagSelectorDone(object sender, EventArgsOf<List<ImageEntity>> ea)
        {
            _imageCache.SaveOrUpdate(ea.Data);
        }

        private void OnTrashClicked(object sender, EventArgs ea)
        {
            try
            {
                var viewController = (PhotoPage)_pageViewController.ViewControllers.FirstOrDefault();
                if (viewController == null)
                {
                    return;
                }

                int pageIndex = viewController.ImageIndex;
                ImageEntity image = _images[pageIndex];
                PHAsset asset = _imageCache.GetAsset(image.LocalIdentifier);
                PHPhotoLibrary.SharedPhotoLibrary.PerformChanges(
                    () => PHAssetChangeRequest.DeleteAssets(new[] { asset }),
                    (result, error) => OnDeleteAssetsCompleted(image, result, error));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }


        private class MyDataSource : UIPageViewControllerDataSource
        {
            private readonly PhotoViewController1 _parentController;

            public MyDataSource(PhotoViewController1 parentController)
            {
                _parentController = parentController;
            }

            public override UIViewController GetNextViewController(UIPageViewController pageViewController,
                UIViewController referenceViewController)
            {
                var currentPage = referenceViewController as PhotoPage;

                if (currentPage.ImageIndex >= (_parentController._images.Count - 1))
                {
                    return null;
                }
                else
                {
                    int nextImageIndex = currentPage.ImageIndex + 1;

                    return new PhotoPage(null, nextImageIndex, _parentController._images);
                }
            }

            public override UIViewController GetPreviousViewController(UIPageViewController pageViewController,
                UIViewController referenceViewController)
            {
                var currentPage = referenceViewController as PhotoPage;
                if (currentPage.ImageIndex <= 0)
                {
                    return null;
                }
                else
                {
                    int previousImageIndex = currentPage.ImageIndex - 1;

                    return new PhotoPage(null, previousImageIndex, _parentController._images);
                }
            }
        }


        private sealed class MyPageViewController : UIPageViewController
        {
            private readonly UIPageViewControllerDataSource _pageViewDataSource;

            public MyPageViewController(PhotoViewController1 parentController) : base(
                UIPageViewControllerTransitionStyle.Scroll,
                UIPageViewControllerNavigationOrientation.Horizontal,
                UIPageViewControllerSpineLocation.None, 10f)
            {
                _pageViewDataSource = new MyDataSource(parentController);
                DataSource = _pageViewDataSource;
            }

            public override void DidReceiveMemoryWarning()
            {
                base.DidReceiveMemoryWarning();
                DisposeChildViewControllers();
                ResetDataSource();
            }

            public void ResetDataSource()
            {
                DataSource = null;
                DataSource = _pageViewDataSource;
            }

            public override void SetViewControllers(
                UIViewController[] viewControllers,
                UIPageViewControllerNavigationDirection direction,
                bool animated,
                UICompletionHandler completionHandler)
            {
                DisposeChildViewControllers();
                base.SetViewControllers(viewControllers, direction, animated, completionHandler);
            }

            private void DisposeChildViewControllers()
            {
                UIViewController[] childControllers = ChildViewControllers;
                foreach (UIViewController child in childControllers)
                {
                    child.SafeDispose();
                    child.RemoveFromParentViewController();
                }
            }
        }
    }
}
