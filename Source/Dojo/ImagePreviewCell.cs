using System;
using CoreGraphics;
using Domain;
using Foundation;
using Photos;
using UIKit;

namespace Dojo
{
    public sealed class ImagePreviewCell : UICollectionViewCell
    {
        private static readonly PHImageRequestOptions _options = new PHImageRequestOptions
        {
            NetworkAccessAllowed = true,
            Synchronous = false
        };

        private readonly ImageCache _imageCache = ImageCache.Instance;
        private readonly UIImageView _imageView;
        private readonly UIImageView _overlayView;
        //		private readonly SelectView _selectView = new SelectView ();

        [Export("initWithFrame:")]
        public ImagePreviewCell(CGRect frame) : base(frame)
        {
            _imageView = new UIImageView
            {
                Frame = Bounds,
                ContentMode = UIViewContentMode.ScaleAspectFit,
                ClipsToBounds = true
            };
            _overlayView = new UIImageView(Resources.SelectImage)
            {
                Frame = Bounds
            };
            ContentView.AddSubview(_imageView);

            //			ContentView.AddSubview (_selectView);
            ContentView.AddSubview(_overlayView);
        }

        public void SelectCell()
        {
            Selected = true;
            _overlayView.Hidden = false;
            _imageView.Alpha = 0.6f;
            //			_selectView.Update (true);
        }

        //		public override void LayoutSubviews ()
        //		{
        //			base.LayoutSubviews ();
        //
        //			_selectView.Frame = ContentView.Bounds;
        //			_imageView.Frame = Bounds;
        //			_selectView.SetNeedsDisplay ();
        //		}

        public void SetImage(string localId)
        {
            PHAsset asset = _imageCache.GetAsset(localId);
            _imageCache.GetCachingImage(asset, _imageView.Frame.Size, _options, x => _imageView.Image = x);
        }

        public void UnselectCell()
        {
            Selected = false;
            _overlayView.Hidden = true;
            _imageView.Alpha = 1f;
            //			_selectView.Update (false);
        }

        private void UpdateImage(UIImage image, NSDictionary imageInfo)
        {
            _imageView.Image = image;
        }


        private class SelectView : UIView
        {
            private static readonly CGGradient _gradient;
            private bool _selected;

            static SelectView()
            {
                using (CGColorSpace colorSpace = CGColorSpace.CreateDeviceRGB())
                {
                    _gradient = new CGGradient(colorSpace, new nfloat[] { .52f, .69f, .96f, 1, .12f, .31f, .67f, 1 }, null);
                }
            }

            public SelectView()
            {
                BackgroundColor = UIColor.Clear;
                Layer.BorderWidth = 1;
                Layer.BorderColor = UIColor.White.CGColor;
            }

            public override void Draw(CGRect rect)
            {
                if (_selected)
                {
                    CGContext context = UIGraphics.GetCurrentContext();
                    context.SaveState();
                    context.AddEllipseInRect(new CGRect(10, 25, 30, 30));
                    context.Clip();

                    context.DrawLinearGradient(_gradient, new CGPoint(10, 25), new CGPoint(22, 44),
                        CGGradientDrawingOptions.DrawsAfterEndLocation);

                    context.RestoreState();
                }
            }

            public void Update(bool select)
            {
                _selected = select;
                SetNeedsDisplay();
            }
        }
    }
}
