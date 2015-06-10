using System;
using Domain;
using UIKit;

namespace Dojo
{
    public sealed class PhotoViewPage1 : UIViewController
    {
        private readonly WeakReference<PhotoViewController4> _parentController;

        private readonly ImageScrollView _scrollView = new ImageScrollView
        {
            AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight,
        };

        private PhotoViewPage1(WeakReference<PhotoViewController4> parentController, ImageEntity image)
        {
            _parentController = parentController;
            ImageEntity = image;
            Title = "Image";
        }

        public ImageEntity ImageEntity { get; private set; }

        public static PhotoViewPage1 ImageViewControllerForPageIndex(WeakReference<PhotoViewController4> parentController, ImageEntity image)
        {
            return new PhotoViewPage1(parentController, image);
        }

        public override void ViewDidDisappear(bool animated)
        {
            _scrollView.ReleaseResources();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            _scrollView.ImageEntity = ImageEntity;
            View = _scrollView;

            var tapGesture = new UITapGestureRecognizer(OnImageTap);
            View.AddGestureRecognizer(tapGesture);
        }

        private void OnImageTap(UITapGestureRecognizer gesture)
        {
            PhotoViewController4 controller;
            if (_parentController.TryGetTarget(out controller))
            {
                controller.FullScreen = !controller.FullScreen;
                controller.NavigationController.SetNavigationBarHidden(controller.FullScreen, false);
                controller.NavigationController.SetToolbarHidden(controller.FullScreen, false);
                View.BackgroundColor = controller.FullScreen ? UIColor.Black : UIColor.White;
            }
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            _scrollView.ResetImage();
        }
    }
}
