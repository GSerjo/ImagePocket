using System;
using CoreGraphics;
using Domain;
using Photos;
using UIKit;

namespace Dojo
{
    public class ImageScrollView : UIScrollView
    {
        private readonly ImageCache _imageCache = ImageCache.Instance;
        private ImageEntity _imageEntity;

        private CGSize _imageSize;
        private CGPoint _pointToCenterAfterResize;
        private nfloat _scaleToRestoreAfterResize;
        private UIImageView zoomView;

        public ImageScrollView()
        {
            ShowsVerticalScrollIndicator = false;
            ShowsHorizontalScrollIndicator = false;
            BouncesZoom = true;
            DecelerationRate = DecelerationRateFast;

            ViewForZoomingInScrollView = sv => zoomView;
        }

        public override CGRect Frame
        {
            get { return base.Frame; }
            set
            {
                bool sizeChanging = Frame.Size != value.Size;
                if (sizeChanging)
                {
                    PrepareToResize();
                }

                base.Frame = value;

                if (sizeChanging)
                {
                    RecoverFromResizing();
                }
            }
        }

        public ImageEntity ImageEntity
        {
            get { return _imageEntity; }
            set
            {
                _imageEntity = value;
                PHAsset asset = _imageCache.GetAsset(_imageEntity.LocalIdentifier);
                DisplayImage(GetImage(asset));
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            CGSize boundsSize = Bounds.Size;
            CGRect frameToCenter = zoomView.Frame;

            if (frameToCenter.Size.Width < boundsSize.Width)
            {
                frameToCenter.X = (boundsSize.Width - frameToCenter.Size.Width) / 2;
            }
            else
            {
                frameToCenter.X = 0;
            }

            if (frameToCenter.Size.Height < boundsSize.Height)
            {
                frameToCenter.Y = (boundsSize.Height - frameToCenter.Size.Height) / 2;
            }
            else
            {
                frameToCenter.Y = 0;
            }
            zoomView.Frame = frameToCenter;
        }

        private void ConfigureForImageSize(CGSize imageSize)
        {
            _imageSize = imageSize;
            ContentSize = imageSize;
            SetMaxMinZoomScalesForCurrentBounds();
            ZoomScale = MinimumZoomScale;
        }

        private void DisplayImage(UIImage image)
        {
            if (image == null)
            {
                throw new ArgumentNullException("image");
            }

            if (zoomView != null)
            {
                zoomView.RemoveFromSuperview();
                zoomView = null;
                ZoomScale = 1.0f;
            }

            zoomView = new UIImageView(image);
            AddSubview(zoomView);
            ConfigureForImageSize(image.Size);
        }

        private UIImage GetImage(PHAsset asset)
        {
            UIImage result = null;
            var options = new PHImageRequestOptions
            {
                Synchronous = true,
            };
            var size = new CGSize(UIScreen.MainScreen.Bounds.Size.Width, UIScreen.MainScreen.Bounds.Size.Height);
            PHImageManager.DefaultManager.RequestImageForAsset(asset, size,
                PHImageContentMode.AspectFit, options,
                (image, info) => result = image);
            return result;
        }

        private CGPoint MaximumContentOffset()
        {
            CGSize contentSize = ContentSize;
            CGSize boundsSize = Bounds.Size;
            return new CGPoint(contentSize.Width - boundsSize.Width, contentSize.Height - boundsSize.Height);
        }

        private CGPoint MinimumContentOffset()
        {
            return CGPoint.Empty;
        }

        private void PrepareToResize()
        {
            var boundsCenter = new CGPoint(Bounds.GetMidX(), Bounds.GetMidY());
            _pointToCenterAfterResize = ConvertPointToView(boundsCenter, zoomView);
            _scaleToRestoreAfterResize = ZoomScale;
            if (_scaleToRestoreAfterResize <= MinimumZoomScale + float.Epsilon)
            {
                _scaleToRestoreAfterResize = 0;
            }
        }

        private void RecoverFromResizing()
        {
            SetMaxMinZoomScalesForCurrentBounds();

            ZoomScale = NMath.Min(MaximumZoomScale, NMath.Max(MinimumZoomScale, _scaleToRestoreAfterResize));

            CGPoint boundsCenter = ConvertPointFromView(_pointToCenterAfterResize, zoomView);
            var offset = new CGPoint(boundsCenter.X - Bounds.Size.Width / 2.0f, boundsCenter.Y - Bounds.Size.Height / 2.0f);
            CGPoint maxOffset = MaximumContentOffset();
            CGPoint minOffset = MinimumContentOffset();
            offset.X = NMath.Max(minOffset.X, NMath.Min(maxOffset.X, offset.X));
            offset.Y = NMath.Max(minOffset.Y, NMath.Min(maxOffset.Y, offset.Y));
            ContentOffset = offset;
        }

        private void SetMaxMinZoomScalesForCurrentBounds()
        {
            CGSize boundsSize = Bounds.Size;
            nfloat xScale = boundsSize.Width / _imageSize.Width;
            nfloat yScale = boundsSize.Height / _imageSize.Height;
            bool imagePortrait = _imageSize.Height > _imageSize.Width;
            bool phonePortrait = boundsSize.Height > boundsSize.Width;
            nfloat minScale = imagePortrait == phonePortrait ? xScale : NMath.Min(xScale, yScale);

            nfloat maxScale = 1 / UIScreen.MainScreen.Scale;

            if (minScale > maxScale)
            {
                minScale = maxScale;
            }
            MaximumZoomScale = maxScale;
            MinimumZoomScale = minScale;
        }
    }
}
