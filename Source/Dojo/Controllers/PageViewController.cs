using System;
using System.Collections.Generic;
using Core;
using Domain;
using Foundation;
using Photos;
using UIKit;

namespace Dojo
{
    public sealed class PageViewController : UIPageViewController
    {
        private readonly ImageCache _imageCache = ImageCache.Instance;
        private static int _currentImageIndex;
        private static List<ImageEntity> _images;

        public PageViewController(ImageEntity currentImage, List<ImageEntity> images)
            : base(UIPageViewControllerTransitionStyle.Scroll,
                UIPageViewControllerNavigationOrientation.Horizontal,
                UIPageViewControllerSpineLocation.None, 10f)
        {
            _images = images;
            _currentImageIndex = _images.FindIndex(x => x.Equals(currentImage));

            ImageViewController currentViewController = ImageViewController.FromPageIndex(_currentImageIndex);
            SetViewControllers(new UIViewController[] { currentViewController }, UIPageViewControllerNavigationDirection.Forward, false, null);

            var tabButton = new UIBarButtonItem("Tag", UIBarButtonItemStyle.Plain, OnTagClicked);
            NavigationItem.RightBarButtonItem = tabButton;

            var btTrash = new UIBarButtonItem(UIBarButtonSystemItem.Trash, OnTrashClicked);
            var deleteSpace = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
            ToolbarItems = new[] { deleteSpace, btTrash };

			DataSource = new MyDataSource();

//			GetNextViewController = NextViewController;
//			GetPreviousViewController = PreviousViewController;
        }

		private UIViewController NextViewController(UIPageViewController pageViewController, 
			UIViewController referenceViewController)
		{
            int index = ((ImageViewController)referenceViewController).PageIndex;
            _currentImageIndex = index + 1;
            return ImageViewController.FromPageIndex(_currentImageIndex);
		}

		private UIViewController PreviousViewController(UIPageViewController pageViewController,
            UIViewController referenceViewController)
        {
            int index = ((ImageViewController)referenceViewController).PageIndex;
            _currentImageIndex = index - 1;
			return ImageViewController.FromPageIndex(_currentImageIndex);
        }

        private ImageEntity GetCurrentImage()
        {
            ImageEntity result = _images[_currentImageIndex];
            return result;
        }

        private void OnDeleteAssetsCompleted(ImageEntity removedImage, bool result, NSError error)
        {
            if (result)
            {
                _images.Remove(removedImage);
                _imageCache.Remove(removedImage);
                _currentImageIndex--;
                if (_images.IsNullOrEmpty())
                {
                    InvokeOnMainThread(() => NavigationController.PopViewController(true));
                }
                else if (_currentImageIndex < 0)
                {
                    _currentImageIndex = 0;
                }
                SwipeImage();
            }
            else
            {
                Console.WriteLine(error);
            }
        }

        private void OnTagClicked(object sender, EventArgs ea)
        {
            var controller = new TagSelectorViewController(GetCurrentImage())
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
                PHAsset asset = _imageCache.GetAsset(GetCurrentImage().LocalIdentifier);
                PHPhotoLibrary.SharedPhotoLibrary.PerformChanges(
                    () => PHAssetChangeRequest.DeleteAssets(new[] { asset }),
                    (result, error) => OnDeleteAssetsCompleted(GetCurrentImage(), result, error));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void SwipeImage()
        {
            ImageEntity image = _images[_currentImageIndex];
            PHAsset asset = _imageCache.GetAsset(image.LocalIdentifier);
            InvokeOnMainThread(() => UpdateImage(asset));
        }

        private void UpdateImage(PHAsset asset)
        {
            PHImageManager.DefaultManager.RequestImageForAsset(asset, View.Frame.Size,
                PHImageContentMode.AspectFit, new PHImageRequestOptions(), (img, info) =>
                {
                    //					UIView.Animate(3, 0, UIViewAnimationOptions.CurveEaseInOut, () => _imageView.Image = img, ()=>{});
                    //                    UIView.Transition(_imageView, 0.5, UIViewAnimationOptions.CurveLinear, () => _imageView.Image = img, null);
                });
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
                return null;
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
				var resultIndex = index + 1;
				return ImageViewController.FromPageIndex(resultIndex);
            }

            public override UIViewController GetPreviousViewController(UIPageViewController pageViewController,
                UIViewController referenceViewController)
            {
                int index = ((ImageViewController)referenceViewController).PageIndex;
				var resultIndex = index - 1;
				return ImageViewController.FromPageIndex(resultIndex);
            }
        }
    }
}
