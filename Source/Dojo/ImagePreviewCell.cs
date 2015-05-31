using System;
using System.Drawing;
using Domain;
using Foundation;
using Photos;
using UIKit;

namespace Dojo
{
    public sealed class ImagePreviewCell : UICollectionViewCell
    {
        private readonly ImageCache _imageCache = ImageCache.Instance;
        private readonly UIImageView _imageView;
        private readonly UIImageView _overlayView;
		private static PHImageRequestOptions _options = new PHImageRequestOptions
		{
			NetworkAccessAllowed = true
		};
			
        [Export("initWithFrame:")]
        public ImagePreviewCell(RectangleF frame) : base(frame)
        {
            _imageView = new UIImageView
            {
                Frame = Bounds,
                ContentMode = UIViewContentMode.ScaleAspectFit,
                ClipsToBounds = true
            };
            _overlayView = new UIImageView(Resources.SelectImage);
            ContentView.AddSubview(_imageView);
            _overlayView.Frame = Bounds;
            ContentView.AddSubview(_overlayView);
        }

        public void SelectCell()
        {
            Selected = true;
            _overlayView.Hidden = false;
            _imageView.Alpha = 0.5f;
        }

        public void SetImage(string localId)
        {
            PHAsset asset = _imageCache.GetAsset(localId);
            _imageCache.ImageManager.RequestImageForAsset(
                asset,
                _imageView.Frame.Size,
				PHImageContentMode.AspectFit, _options, UpdateImage);
        }

        public void UnselectCell()
        {
            Selected = false;
            _overlayView.Hidden = true;
            _imageView.Alpha = 1f;
        }

        private void UpdateImage(UIImage image, NSDictionary imageInfo)
        {
            _imageView.Image = image;
        }
    }
}
