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
        private readonly PHAsset _asset;
        private readonly ImageCache _imageCache = ImageCache.Instance;
        private bool _fullScreen;
        private ImageEntity _image;
        private UIImageView _imageView;

        public PhotoViewController(ImageEntity image)
        {
            Title = "Image";
            _image = image;
            _asset = _imageCache.GetAsset(image.LocalIdentifier);
            var tabButton = new UIBarButtonItem("Tag", UIBarButtonItemStyle.Plain, OnTagClicked);
            NavigationItem.RightBarButtonItem = tabButton;
        }

        public override void ViewDidLoad()
        {
            View.BackgroundColor = UIColor.White;
            _imageView = new UIImageView(View.Frame);
            PHImageManager.DefaultManager.RequestImageForAsset(_asset, View.Frame.Size,
                PHImageContentMode.AspectFit, new PHImageRequestOptions(), (img, info) =>
                {
                    _imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
                    _imageView.Image = img;
                });
            View.AddSubview(_imageView);

            AddGestures();
        }

        private void AddGestures()
        {
            var tapGesture = new UITapGestureRecognizer(OnViewTap);
            View.AddGestureRecognizer(tapGesture);

            var rightSwipe = new UISwipeGestureRecognizer(OnRightSwipe)
            {
                NumberOfTouchesRequired = 1,
                Direction = UISwipeGestureRecognizerDirection.Right
            };
            View.AddGestureRecognizer(rightSwipe);
        }

        private void OnRightSwipe(UISwipeGestureRecognizer gesture)
        {
            Console.WriteLine("Swipe to the right");
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
