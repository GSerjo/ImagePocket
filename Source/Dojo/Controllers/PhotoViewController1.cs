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
        private UIPageViewController _pageViewController;
        private UIPopoverController _shareController;

        public PhotoViewController1(ImageEntity image, List<ImageEntity> images)
        {
            _images = images;
            _currentImageIndex = _images.FindIndex(x => x.Equals(image));

            var tabButton = new UIBarButtonItem("Tag", UIBarButtonItemStyle.Plain, OnTagClicked);
            NavigationItem.RightBarButtonItem = tabButton;

            _btShare = new UIBarButtonItem(UIBarButtonSystemItem.Action, OnShareClicked);
            var btTrash = new UIBarButtonItem(UIBarButtonSystemItem.Trash, OnTrashClicked);
            var deleteSpace = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
            ToolbarItems = new[] { _btShare, deleteSpace, btTrash };
        }

        public override void ViewDidLoad()
        {
            _pageViewController = new UIPageViewController(
                UIPageViewControllerTransitionStyle.Scroll,
                UIPageViewControllerNavigationOrientation.Horizontal,
                UIPageViewControllerSpineLocation.None, 10f)
            {
                GetNextViewController = GetNextViewController,
                GetPreviousViewController = GetPreviousViewController,
            };

            var firstPage = new PhotoPage(this, _currentImageIndex, _images);
            _pageViewController.SetViewControllers(
                new UIViewController[] { firstPage },
                UIPageViewControllerNavigationDirection.Forward,
                false, x => { });

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

            int pageIndex = viewController.PageIndex;
            ImageEntity image = _images[pageIndex];
            return image.ToOption();
        }

        private UIViewController GetNextViewController(UIPageViewController pageController, UIViewController referenceViewController)
        {
            var currentPageController = referenceViewController as PhotoPage;

            if (currentPageController.PageIndex >= (_images.Count - 1))
            {
                return null;
            }
            else
            {
                int nextPageIndex = currentPageController.PageIndex + 1;

                return new PhotoPage(this, nextPageIndex, _images);
            }
        }

        private UIViewController GetPreviousViewController(UIPageViewController pageController, UIViewController referenceViewController)
        {
            var currentPageController = referenceViewController as PhotoPage;
            if (currentPageController.PageIndex <= 0)
            {
                return null;
            }
            else
            {
                int previousPageIndex = currentPageController.PageIndex - 1;

                return new PhotoPage(this, previousPageIndex, _images);
            }
        }

        private void OnDeleteAssetsCompleted(ImageEntity removedImage, bool result, NSError error)
        {
            if (result)
            {
                _images.Remove(removedImage);
                _imageCache.Remove(removedImage);
                //				_currentImageIndex--;
                //				if (_images.IsNullOrEmpty())
                //				{
                //					InvokeOnMainThread(() => NavigationController.PopViewController(true));
                //					return;
                //				}
                //				else if (_currentImageIndex < 0)
                //				{
                //					_currentImageIndex = 0;
                //				}
                //				SwipeImage();
            }
            else
            {
                Console.WriteLine(error);
            }
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

                int pageIndex = viewController.PageIndex;
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
    }
}
