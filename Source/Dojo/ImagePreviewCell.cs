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
        private readonly UIImageView checkView = new UIImageView(new UIImage("Checked"));

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
            ContentView.AddSubview(checkView);
        }

        public override bool Selected
        {
            get { return base.Selected; }
            set
            {
                base.Selected = value;
                checkView.Hidden = !Selected;
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            _imageView.Frame = Bounds;
            checkView.Frame = new RectangleF(new PointF(ContentView.Bounds.Width - checkView.Bounds.Width, 0), new SizeF(10, 10));
        }

        public void SelectCell()
        {
            //            BackgroundColor = UIColor.Black;
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
            //            BackgroundColor = UIColor.White;
            Selected = false;
        }
    }
}
