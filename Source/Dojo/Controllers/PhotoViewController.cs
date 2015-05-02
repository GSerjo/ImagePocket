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

        public PhotoViewController(ImageEntity image, List<ImageEntity> images)
        {
            Title = "Image";
            _image = image;
            _images = images;
            _currentImageIndex = _images.FindIndex(x => x.Equals(image));
            
            var tabButton = new UIBarButtonItem("Tag", UIBarButtonItemStyle.Plain, OnTagClicked);
            NavigationItem.RightBarButtonItem = tabButton;
        }

        public override void ViewDidLoad()
        {
            View.BackgroundColor = UIColor.White;

			_imageView = new UIImageView (View.Frame)
			{
				MultipleTouchEnabled = true,
				UserInteractionEnabled = true
			};

			var tapGesture = new UITapGestureRecognizer(OnImageTap);
			_imageView.AddGestureRecognizer(tapGesture);

			var leftSwipe = new UISwipeGestureRecognizer(OnImageSwipe)
			{
				NumberOfTouchesRequired = 1,
				Direction = UISwipeGestureRecognizerDirection.Left
			};
			_imageView.AddGestureRecognizer(leftSwipe);
            View.AddSubview(_imageView);

			var asset = _imageCache.GetAsset(_image.LocalIdentifier);
			UpdateImage (asset);
        }

		private void UpdateImage(PHAsset asset)
        {
            PHImageManager.DefaultManager.RequestImageForAsset(asset, View.Frame.Size,
                PHImageContentMode.AspectFit, new PHImageRequestOptions(), (img, info) =>
                {
                    _imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
                    _imageView.Image = img;
                });
        }

        private void OnImageSwipe(UISwipeGestureRecognizer gesture)
        {
            Console.WriteLine("Swipe to the left");

			if (gesture.Direction == UISwipeGestureRecognizerDirection.Left)
			{

				if (_currentImageIndex >= _images.Count)
				{
					return;
				}

				_currentImageIndex++;
				_image = _images [_currentImageIndex];
				var asset = _imageCache.GetAsset (_image.LocalIdentifier);

				UpdateImage (asset);
			}
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

        private void OnImageTap(UITapGestureRecognizer gesture)
        {
            _fullScreen = !_fullScreen;
            NavigationController.SetNavigationBarHidden(_fullScreen, false);
            View.BackgroundColor = _fullScreen ? UIColor.Black : UIColor.White;
        }
    }
}
