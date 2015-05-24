using System;
using CoreGraphics;
using UIKit;

namespace Dojo
{
    public abstract class ImageScrollView : UIScrollView
    {
        private CGSize _imageSize;
        private int _index;
        // if tiling this contains a very low-res placeholder image. otherwise it contains the full image.
        private CGPoint _pointToCenterAfterResize;
        private nfloat _scaleToRestoreAfterResize;
        private UIImageView zoomView;

        protected ImageScrollView()
        {
            ShowsVerticalScrollIndicator = false;
            ShowsHorizontalScrollIndicator = false;
            BouncesZoom = true;
            DecelerationRate = DecelerationRateFast;

            // Return the view to use when zooming
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

        public int Index
        {
            get { return _index; }
            set
            {
                _index = value;
                DisplayImage(_index);
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            //center the zoom view as it becomes smaller than the size of the screen
            CGSize boundsSize = Bounds.Size;
            CGRect frameToCenter = zoomView.Frame;

            //center horizontally
            if (frameToCenter.Size.Width < boundsSize.Width)
            {
                frameToCenter.X = (boundsSize.Width - frameToCenter.Size.Width) / 2;
            }
            else
            {
                frameToCenter.X = 0;
            }

            //center vertically
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

        protected abstract void DisplayImage(int imageIndex);

        protected void DisplayImage(UIImageView image)
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

            zoomView = image;
            AddSubview(zoomView);
            ConfigureForImageSize(zoomView.Image.Size);
        }

        protected void DisplayImage(UIImage image)
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

            zoomView = new UIImageView(image)
            {
                ContentMode = UIViewContentMode.ScaleAspectFit,
                Frame = UIScreen.MainScreen.Bounds,
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight,
            };
            AddSubview(zoomView);
            ConfigureForImageSize(zoomView.Frame.Size);
        }

        private void ConfigureForImageSize(CGSize imageSize)
        {
            _imageSize = imageSize;
            ContentSize = imageSize;
            SetMaxMinZoomScalesForCurrentBounds();
            ZoomScale = MinimumZoomScale;
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
            // If we're at the minimum zoom scale, preserve that by returning 0, which will be converted to the minimum
            // allowable scale when the scale is restored.
            if (_scaleToRestoreAfterResize <= MinimumZoomScale + float.Epsilon)
            {
                _scaleToRestoreAfterResize = 0;
            }
        }

        private void RecoverFromResizing()
        {
            SetMaxMinZoomScalesForCurrentBounds();

            //Step 1: restore zoom scale, first making sure it is within the allowable range;
            ZoomScale = NMath.Min(MaximumZoomScale, NMath.Max(MinimumZoomScale, _scaleToRestoreAfterResize));

            // Step 2: restore center point, first making sure it is within the allowable range.
            // 2a: convert our desired center point back to our own coordinate space
            CGPoint boundsCenter = ConvertPointFromView(_pointToCenterAfterResize, zoomView);
            // 2b: calculate the content offset that would yield that center point
            var offset = new CGPoint(boundsCenter.X - Bounds.Size.Width / 2.0f, boundsCenter.Y - Bounds.Size.Height / 2.0f);
            // 2c: restore offset, adjusted to be within the allowable range
            CGPoint maxOffset = MaximumContentOffset();
            CGPoint minOffset = MinimumContentOffset();
            offset.X = NMath.Max(minOffset.X, NMath.Min(maxOffset.X, offset.X));
            offset.Y = NMath.Max(minOffset.Y, NMath.Min(maxOffset.Y, offset.Y));
            ContentOffset = offset;
        }

        private void SetMaxMinZoomScalesForCurrentBounds()
        {
            CGSize boundsSize = Bounds.Size;

            //calculate min/max zoomscale
            nfloat xScale = boundsSize.Width / _imageSize.Width; //scale needed to perfectly fit the image width-wise
            nfloat yScale = boundsSize.Height / _imageSize.Height; //scale needed to perfectly fit the image height-wise

            //fill width if the image and phone are both portrait or both landscape; otherwise take smaller scale
            bool imagePortrait = _imageSize.Height > _imageSize.Width;
            bool phonePortrait = boundsSize.Height > boundsSize.Width;
            nfloat minScale = imagePortrait == phonePortrait ? xScale : NMath.Min(xScale, yScale);

            //on high resolution screens we have double the pixel density, so we will be seeing every pixel if we limit the maximum zoom scale to 0.5
            nfloat maxScale = 1 / UIScreen.MainScreen.Scale;

            if (minScale > maxScale)
            {
                minScale = maxScale;
            }

            // don't let minScale exceed maxScale. (If the image is smaller than the screen, we don't want to force it to be zoomed.)
            MaximumZoomScale = maxScale;
            MinimumZoomScale = minScale;
        }
    }
}
