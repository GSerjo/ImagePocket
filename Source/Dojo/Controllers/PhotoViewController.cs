using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Domain;
using MonoTouch.Photos;
using MonoTouch.UIKit;

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
        private PHAsset _asset;

        public PhotoViewController(ImageEntity image, List<ImageEntity> images)
        {
            Title = "Image";
            _image = image;
            _images = images;
            _currentImageIndex = _images.FindIndex(x => x.Equals(image));
            _asset = _imageCache.GetAsset(_image.LocalIdentifier);

            var tabButton = new UIBarButtonItem("Tag", UIBarButtonItemStyle.Plain, OnTagClicked);
            NavigationItem.RightBarButtonItem = tabButton;
        }

        public override void ViewDidLoad()
        {
            View.BackgroundColor = UIColor.White;
            View.UserInteractionEnabled = true;

            _imageView = CreateImageView();

            View.AddSubview(_imageView);
            AddGestures();
        }

        private void AddGestures()
        {
            var tapGesture = new UITapGestureRecognizer(OnViewTap);
            View.AddGestureRecognizer(tapGesture);

            var leftSwipe = new UISwipeGestureRecognizer(OnLeftSwipe)
            {
                NumberOfTouchesRequired = 1,
                Direction = UISwipeGestureRecognizerDirection.Left
            };
            View.AddGestureRecognizer(leftSwipe);
        }

        private UIImageView CreateImageView()
        {
            var imageView = new UIImageView(View.Frame);
            PHImageManager.DefaultManager.RequestImageForAsset(_asset, View.Frame.Size,
                PHImageContentMode.AspectFit, new PHImageRequestOptions(), (img, info) =>
                {
                    imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
                    imageView.Image = img;
                });
            return imageView;
        }

        private void OnLeftSwipe(UISwipeGestureRecognizer gesture)
        {
            Console.WriteLine("Swipe to the left");

            if (_currentImageIndex >= _images.Count)
            {
                return;
            }

            _currentImageIndex++;
            _image = _images[_currentImageIndex];
            _asset = _imageCache.GetAsset(_image.LocalIdentifier);

            var imageView = CreateImageView();
            _imageView.RemoveFromSuperview();
            _imageView = imageView;
            View.AddSubview(_imageView);
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

        private void OnViewTap(UITapGestureRecognizer gesture)
        {
            _fullScreen = !_fullScreen;
            NavigationController.SetNavigationBarHidden(_fullScreen, false);
            View.BackgroundColor = _fullScreen ? UIColor.Black : UIColor.White;
        }
    }
}
