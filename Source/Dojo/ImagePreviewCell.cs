using System;
using System.Drawing;
using Domain;
using MonoTouch.Foundation;
using MonoTouch.Photos;
using MonoTouch.UIKit;

namespace Dojo
{
    public sealed class ImagePreviewCell : UICollectionViewCell
    {
        private static readonly UIImage _selectedImage = UIImage.FromBundle("tick_selected");
        private readonly ImageCache _imageCache = ImageCache.Instance;
        private readonly UIImageView _imageView;
        private readonly UIImageView _overlayView;

        [Export("initWithFrame:")]
        public ImagePreviewCell(RectangleF frame) : base(frame)
        {
            _imageView = new UIImageView
            {
                Frame = Bounds,
                ContentMode = UIViewContentMode.ScaleAspectFit,
                ClipsToBounds = true
            };
            _overlayView = new UIImageView(_selectedImage);
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
                PHImageContentMode.AspectFit, null, UpdateImage);
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
