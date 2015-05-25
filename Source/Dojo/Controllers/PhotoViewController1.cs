﻿using System;
using System.Collections.Generic;
using Domain;
using UIKit;
using System.Linq;
using Core;
using Photos;
using Foundation;

namespace Dojo
{
    public sealed class PhotoViewController1 : UIViewController
    {
        private readonly int _currentImageIndex;
        private readonly List<ImageEntity> _images;
        private UIPageViewController _pageViewController;
		private readonly ImageCache _imageCache = ImageCache.Instance;
		UIPopoverController _shareController;

        public PhotoViewController1(ImageEntity image, List<ImageEntity> images)
        {
            _images = images;
            _currentImageIndex = _images.FindIndex(x => x.Equals(image));

            var tabButton = new UIBarButtonItem("Tag", UIBarButtonItemStyle.Plain, OnTagClicked);
            NavigationItem.RightBarButtonItem = tabButton;

			var btShare = new UIBarButtonItem (UIBarButtonSystemItem.Action, OnShareClicked);
            var btTrash = new UIBarButtonItem(UIBarButtonSystemItem.Trash, OnTrashClicked);
            var deleteSpace = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
			ToolbarItems = new[] {btShare, deleteSpace, btTrash };
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

            var firstPage = new Page(this, _currentImageIndex, _images);
            _pageViewController.SetViewControllers(
                new UIViewController[] { firstPage },
                UIPageViewControllerNavigationDirection.Forward,
                false, x => { });

            _pageViewController.View.Frame = View.Bounds;
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

                return new Page(this, nextPageIndex, _images);
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

                return new Page(this, previousPageIndex, _images);
            }
        }

        private void OnTagClicked(object sender, EventArgs ea)
        {
			var viewController = (Page)_pageViewController.ViewControllers.FirstOrDefault ();
			if (viewController == null)
			{
				return;
			}

			var pageIndex = viewController.PageIndex;
			var image = _images [pageIndex];
			var controller = new TagSelectorViewController(image)
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

		private void OnShareClicked(object sender, EventArgs ea)
		{
//			var viewController = (Page)_pageViewController.ViewControllers.FirstOrDefault ();
//			if (viewController == null)
//			{
//				return;
//			}
//
//			var pageIndex = viewController.PageIndex;
//			var image = _images [pageIndex];
//
//			UIActivityViewController activityController = new UIActivityViewController(image, null);
//
//			_shareController = new UIPopoverController (activityController);
//			PresentViewController (_shareController, true, () =>
//			{
//			});
		}

        private void OnTrashClicked(object sender, EventArgs ea)
        {
			try
			{
				var viewController = (Page)_pageViewController.ViewControllers.FirstOrDefault ();
				if (viewController == null)
				{
					return;
				}

				var pageIndex = viewController.PageIndex;
				var image = _images [pageIndex];
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
    }
}
