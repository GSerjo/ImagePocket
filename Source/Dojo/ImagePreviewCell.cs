using System;
using System.Drawing;
using Domain;
using MonoTouch.Foundation;
using MonoTouch.Photos;
using MonoTouch.UIKit;

namespace Dojo
{
//    public sealed class ImagePreviewCell : UICollectionViewCell
//    {
//        private readonly ImageCache _imageCache = ImageCache.Instance;
//        private readonly UIImageView _imageView;
//		private readonly UIImageView _checkView;
//
//        [Export("initWithFrame:")]
//        public ImagePreviewCell(RectangleF frame) : base(frame)
//        {
//            _imageView = new UIImageView(frame)
//            {
//                Center = ContentView.Center,
//                ContentMode = UIViewContentMode.ScaleAspectFit,
//                AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth,
//                ClipsToBounds = true
//            };
//
//			_checkView = new UIImageView (new UIImage ("Checked"));
//
//            //			_imageView.ContentMode = UIViewContentMode.ScaleAspectFill;
//            ContentView.AddSubview(_imageView);
//            ContentView.AddSubview(_checkView);
//        }
//
//        public override void LayoutSubviews()
//        {
//            base.LayoutSubviews();
////            _imageView.Frame = Bounds;
//			var location = new PointF (ContentView.Bounds.Width - _checkView.Bounds.Width, 50);
//			_checkView.Frame = new RectangleF (location, _checkView.Frame.Size);
////			checkView.Frame.Location = new PointF(ContentView.Bounds.Width - checkView.Bounds.Width, 0);
//        }
//
//        public void SelectCell()
//        {
//            //            BackgroundColor = UIColor.Black;
//            Selected = true;
//			_checkView.Hidden = false;
//        }
//
//        public void SetImage(string localId)
//        {
//            PHAsset asset = _imageCache.GetAsset(localId);
//            _imageCache.ImageManager.RequestImageForAsset(
//				asset,
//				_imageView.Frame.Size,
//				PHImageContentMode.AspectFit, null, UpdateImage);
//        }
//
//		private void UpdateImage(UIImage image, NSDictionary imageInfo)
//		{
//			_imageView.Image = image;
//		}
//
//        public void UnselectCell()
//        {
//            //            BackgroundColor = UIColor.White;
//            Selected = false;
//			_checkView.Hidden = true;
//        }
//    }
//}

    public sealed class ImagePreviewCell : UICollectionViewCell
    {
        private readonly ImageCache _imageCache = ImageCache.Instance;
        private readonly UIImageView _imageView;
		private readonly UIImageView _overlayView = new UIImageView();
		private readonly UIImage _selectedImage = UIImage.FromBundle("tick_selected");
		private readonly UIImage _unselectedImage = UIImage.FromBundle("tick_deselected");

        [Export("initWithFrame:")]
        public ImagePreviewCell(RectangleF frame) : base(frame)
        {
            _imageView = new UIImageView()
            {
				Frame = Bounds,
                ContentMode = UIViewContentMode.ScaleAspectFit,
                ClipsToBounds = true
            };

            ContentView.AddSubview(_imageView);
			_overlayView.Frame = Bounds;
			ContentView.AddSubview(_overlayView);
        }

        public void SelectCell()
        {
            Selected = true;
			_overlayView.Image = _selectedImage;
        }

        public void SetImage(string localId)
        {
            PHAsset asset = _imageCache.GetAsset(localId);
            _imageCache.ImageManager.RequestImageForAsset(
				asset,
				_imageView.Frame.Size,
				PHImageContentMode.AspectFit, null, UpdateImage);
        }

		private void UpdateImage(UIImage image, NSDictionary imageInfo)
		{
			_imageView.Image = image;
		}

        public void UnselectCell()
        {
            Selected = false;
			_overlayView.Image = _unselectedImage;
        }
    }
}
