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
            _imageView = new UIImageView(frame)
            {
                Center = ContentView.Center,
                ContentMode = UIViewContentMode.ScaleAspectFit,
                AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth,
                ClipsToBounds = true
            };

            //			_imageView.ContentMode = UIViewContentMode.ScaleAspectFill;
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

        public void Unselect()
        {
            BackgroundColor = UIColor.White;
            Selected = false;
        }
    }
}
