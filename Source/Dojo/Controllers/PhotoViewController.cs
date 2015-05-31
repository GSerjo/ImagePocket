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
    public sealed class PhotoViewController : UIViewController
    {
        private readonly ImageCache _imageCache = ImageCache.Instance;
        private readonly List<ImageEntity> _images;
        private int _currentImageIndex;
        private bool _fullScreen;
        private ImageEntity _image;
        private UIImageView _imageView;

        public PhotoViewController(ImageEntity image, List<ImageEntity> images)
        {
            Title = "Image";
            _image = image;
            _images = images;
            _currentImageIndex = _images.FindIndex(x => x.Equals(image));

            var tabButton = new UIBarButtonItem("Tag", UIBarButtonItemStyle.Plain, OnTagClicked);
            NavigationItem.RightBarButtonItem = tabButton;

            var btTrash = new UIBarButtonItem(UIBarButtonSystemItem.Trash, OnTrashClicked);
            var deleteSpace = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
            ToolbarItems = new[] { deleteSpace, btTrash };
        }

        public override void ViewDidLoad()
        {
            View.BackgroundColor = UIColor.White;

            _imageView = new UIImageView(View.Frame)
            {
                MultipleTouchEnabled = true,
                UserInteractionEnabled = true,
                ContentMode = UIViewContentMode.ScaleAspectFit,
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight
            };

            var tapGesture = new UITapGestureRecognizer(OnImageTap);
            _imageView.AddGestureRecognizer(tapGesture);

            var leftSwipe = new UISwipeGestureRecognizer(OnImageSwipe)
            {
                Direction = UISwipeGestureRecognizerDirection.Left
            };
            _imageView.AddGestureRecognizer(leftSwipe);

            var rigthSwipe = new UISwipeGestureRecognizer(OnImageSwipe)
            {
                Direction = UISwipeGestureRecognizerDirection.Right
            };
            _imageView.AddGestureRecognizer(rigthSwipe);
            View.AddSubview(_imageView);

            PHAsset asset = _imageCache.GetAsset(_image.LocalIdentifier);
            UpdateImage(asset);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            NavigationController.SetToolbarHidden(false, true);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            NavigationController.SetToolbarHidden(true, true);
        }

        private bool CanSwipeLeft()
        {
            return _currentImageIndex < _images.Count - 1;
        }

        private bool CanSwipeRight()
        {
            return _currentImageIndex > 0;
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
                    return;
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

        private void OnImageSwipe(UISwipeGestureRecognizer gesture)
        {
            switch (gesture.Direction)
            {
                case UISwipeGestureRecognizerDirection.Left:
                    if (CanSwipeLeft())
                    {
                        _currentImageIndex++;
                        SwipeImage();
                        break;
                    }
                    return;
                case UISwipeGestureRecognizerDirection.Right:
                    if (CanSwipeRight())
                    {
                        _currentImageIndex--;
                        SwipeImage();
                        break;
                    }
                    return;
                default:
                    return;
            }
        }

        private void OnImageTap(UITapGestureRecognizer gesture)
        {
            _fullScreen = !_fullScreen;
            NavigationController.SetNavigationBarHidden(_fullScreen, false);
            NavigationController.SetToolbarHidden(_fullScreen, false);
            View.BackgroundColor = _fullScreen ? UIColor.Black : UIColor.White;
        }

        private void OnTagClicked(object sender, EventArgs ea)
        {
            var controller = new TagSelectorViewController(_image)
            {
                ModalPresentationStyle = UIModalPresentationStyle.FormSheet
            };
            controller.Done += OnTagSelectorDone;
            NavigationController.PresentViewController(controller, true, null);
        }

        private void OnTagSelectorDone(object sender, EventArgsOf<List<ImageEntity>> ea)
        {
            _imageCache.SaveOrUpdate(ea.Data);
            _image = ea.Data.First();
        }

        private void OnTrashClicked(object sender, EventArgs ea)
        {
            try
            {
                PHAsset asset = _imageCache.GetAsset(_image.LocalIdentifier);
                PHPhotoLibrary.SharedPhotoLibrary.PerformChanges(
                    () => PHAssetChangeRequest.DeleteAssets(new[] { asset }),
                    (result, error) => OnDeleteAssetsCompleted(_image, result, error));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void SwipeImage()
        {
            _image = _images[_currentImageIndex];
            PHAsset asset = _imageCache.GetAsset(_image.LocalIdentifier);
            InvokeOnMainThread(() => UpdateImage(asset));
        }

        private void UpdateImage(PHAsset asset)
        {
            PHImageManager.DefaultManager.RequestImageForAsset(asset, View.Frame.Size,
                PHImageContentMode.AspectFit, new PHImageRequestOptions(), (img, info) =>
                {
                    //					UIView.Animate(3, 0, UIViewAnimationOptions.CurveEaseInOut, () => _imageView.Image = img, ()=>{});
                    UIView.Transition(_imageView, 0.3, UIViewAnimationOptions.CurveEaseIn, () => _imageView.Image = img, null);
                });
        }
    }
}
