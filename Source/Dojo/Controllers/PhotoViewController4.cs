﻿using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using CoreGraphics;
using Domain;
using Foundation;
using Photos;
using UIKit;

namespace Dojo
{
    public class PhotoViewController4 : UIPageViewController
    {
        private readonly UIBarButtonItem _btShare;
        private readonly ImageEntity _image;
        private readonly ImageCache _imageCache = ImageCache.Instance;
        private readonly MyDataSource _pageVewDataSource = new MyDataSource();
        private static List<ImageEntity> _images;
        private UIPopoverController _shareController;

        public PhotoViewController4(ImageEntity image, List<ImageEntity> images)
            : base(UIPageViewControllerTransitionStyle.Scroll,
                UIPageViewControllerNavigationOrientation.Horizontal,
                UIPageViewControllerSpineLocation.None, 20f)
        {
            FullScreen = false;
            _image = image;
            _images = images;

            var tabButton = new UIBarButtonItem("Tag", UIBarButtonItemStyle.Plain, OnTagClicked);
            NavigationItem.RightBarButtonItem = tabButton;

            _btShare = new UIBarButtonItem(UIBarButtonSystemItem.Action, OnShareClicked);

            var btTrash = new UIBarButtonItem(UIBarButtonSystemItem.Trash, OnTrashClicked);
            var deleteSpace = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
            ToolbarItems = new[] { _btShare, deleteSpace, btTrash };

            DataSource = _pageVewDataSource;
            AutomaticallyAdjustsScrollViewInsets = false;
        }

        public bool FullScreen { get; set; }

        public override void ViewDidLoad()
        {
            PhotoViewPage1 pageZero = PhotoViewPage1.ImageViewControllerForPageIndex(_image);

            var firstPage = new UIViewController[] { pageZero };
            SetViewControllers(firstPage, UIPageViewControllerNavigationDirection.Forward, false, null);

            var tapGesture = new UITapGestureRecognizer(OnImageTap);
            View.AddGestureRecognizer(tapGesture);
        }

        private static UIImage GetImage(PHAsset asset)
        {
            UIImage result = null;
            var options = new PHImageRequestOptions
            {
                Synchronous = true,
                NetworkAccessAllowed = true
            };
            nfloat scale = UIScreen.MainScreen.Scale;
            var size = new CGSize(UIScreen.MainScreen.Bounds.Size.Width * scale,
                UIScreen.MainScreen.Bounds.Size.Height * scale);
            ImageCache.Instance.GetImage(asset, size, options, x => result = x);
            return result;
        }

        private static int GetImageIndex(ImageEntity image)
        {
            return _images.FindIndex(x => x.Equals(image));
        }

        private Option<ImageEntity> GetCurrentImage()
        {
            var viewController = (PhotoViewPage1)ViewControllers.FirstOrDefault();
            if (viewController == null)
            {
                return Option<ImageEntity>.Empty;
            }

            ImageEntity result = viewController.ImageEntity;
            return result.ToOption();
        }

        private void OnDeleteAssetsCompleted(ImageEntity removedImage, bool result, NSError error)
        {
            if (result == false)
            {
                return;
            }

            int imageIndex = GetImageIndex(removedImage);
            _images.Remove(removedImage);
            _imageCache.Remove(removedImage);

            InvokeOnMainThread(() =>
            {
                ResetDataSource();

                if (_images.IsNullOrEmpty())
                {
                    NavigationController.PopViewController(true);
                    return;
                }

                imageIndex--;
                if (imageIndex < 0)
                {
                    imageIndex = 0;
                }
                PhotoViewPage1 page = PhotoViewPage1.ImageViewControllerForPageIndex(_images[imageIndex]);

                SetViewControllers(new UIViewController[] { page },
                    UIPageViewControllerNavigationDirection.Forward,
                    false, null);
            });
        }

        private void OnImageTap(UITapGestureRecognizer gesture)
        {
            FullScreen = !FullScreen;
            NavigationController.SetNavigationBarHidden(FullScreen, true);
            NavigationController.SetToolbarHidden(FullScreen, true);

            View.BackgroundColor = FullScreen ? UIColor.Black : UIColor.White;
        }

        private void OnShareClicked(object sender, EventArgs ea)
        {
            if (_shareController == null)
            {
                var viewController = (PhotoViewPage1)ViewControllers.FirstOrDefault();
                if (viewController == null)
                {
                    return;
                }
                PHAsset asset = _imageCache.GetAsset(viewController.ImageEntity.LocalIdentifier);
                UIImage image = GetImage(asset);
                var activityController = new UIActivityViewController(new NSObject[] { image }, null);
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
                var viewController = (PhotoViewPage1)ViewControllers.FirstOrDefault();
                if (viewController == null)
                {
                    return;
                }
                PHAsset asset = _imageCache.GetAsset(viewController.ImageEntity.LocalIdentifier);
                PHPhotoLibrary.SharedPhotoLibrary.PerformChanges(
                    () => PHAssetChangeRequest.DeleteAssets(new[] { asset }),
                    (result, error) => OnDeleteAssetsCompleted(viewController.ImageEntity, result, error));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void ResetDataSource()
        {
            DataSource = null;
            DataSource = _pageVewDataSource;
        }


        private sealed class MyDataSource : UIPageViewControllerDataSource
        {
            public override UIViewController GetNextViewController(UIPageViewController pageViewController,
                UIViewController referenceViewController)
            {
                int index = GetPageIndex(referenceViewController) + 1;
                if (index < 0 || index >= _images.Count)
                {
                    return null;
                }
                return PhotoViewPage1.ImageViewControllerForPageIndex(_images[index]);
            }

            public override UIViewController GetPreviousViewController(UIPageViewController pageViewController,
                UIViewController referenceViewController)
            {
                int index = GetPageIndex(referenceViewController) - 1;
                if (index < 0 || index >= _images.Count)
                {
                    return null;
                }
                return PhotoViewPage1.ImageViewControllerForPageIndex(_images[index]);
            }

            private int GetPageIndex(UIViewController controller)
            {
                ImageEntity imageEntity = ((PhotoViewPage1)controller).ImageEntity;
                int result = GetImageIndex(imageEntity);
                return result;
            }
        }
    }
}
