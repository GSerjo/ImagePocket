using System;
using System.Drawing;
using CoreGraphics;
using Domain;
using Foundation;
using Photos;
using UIKit;

namespace Dojo
{
    public partial class ThumbnailCell : UICollectionViewCell
    {
        public static readonly NSString Key = new NSString("ThumbnailCell");
        public static readonly UINib Nib = UINib.FromName("ThumbnailCell", NSBundle.MainBundle);
        private readonly ImageCache _imageCache = ImageCache.Instance;
        private readonly UIImageView _imageView;

        public ThumbnailCell(IntPtr handle) : base(handle)
        {
        }

        [Export("initWithFrame:")]
        public ThumbnailCell(CGRect frame): base(frame)
        {
            frame = new CGRect(0, 0, ContentView.Bounds.Width, ContentView.Bounds.Height);
            _imageView = new UIImageView(frame)
            {
                AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth,
                ClipsToBounds = true, ContentMode = UIViewContentMode.ScaleAspectFit
            };
            ContentView.AddSubview(_imageView);
        }

        public static ThumbnailCell Create()
        {
            return (ThumbnailCell)Nib.Instantiate(null, null)[0];
        }

        public new void Select()
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
