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
        private readonly ImageCache _imageCache = ImageCache.Instance;
        private readonly UIImageView _imageView;

        [Export("initWithFrame:")]
        public ImagePreviewCell(RectangleF frame) : base(frame)
        {
            _imageView = new UIImageView(frame);
            _imageView.Center = ContentView.Center;
            _imageView.ContentMode = UIViewContentMode.ScaleAspectFit;

            //			_imageView.ContentMode = UIViewContentMode.ScaleAspectFill;
            _imageView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;
            _imageView.ClipsToBounds = true;
            ContentView.AddSubview(_imageView);
        }

        public void SelectCell()
        {
            BackgroundColor = UIColor.Black;
            Selected = true;
        }

        public void SetImage(string localId)
        {
            PHAsset asset = _imageCache.GetAsset(localId);
            _imageCache.ImageManager.RequestImageForAsset(asset, _imageView.Frame.Size,
                PHImageContentMode.AspectFit, null, (img, info) => { _imageView.Image = img; });
        }

        public void UnselectCell()
        {
            BackgroundColor = UIColor.White;
            Selected = false;
        }
    }
}
