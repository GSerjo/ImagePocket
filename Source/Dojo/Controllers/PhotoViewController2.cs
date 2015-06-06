using System;
using System.Collections.Generic;
using CoreGraphics;
using Domain;
using Photos;
using UIKit;

namespace Dojo
{
    public class PhotoViewController2 : UIViewController
    {
        private readonly ImageEntity _image;
        private readonly ImageCache _imageCache = ImageCache.Instance;
        private readonly List<ImageEntity> _images;
        private readonly UIScrollView _scrollView;
        private int _currentImageIndex;
        private UIImageView _imageView;

        public PhotoViewController2(ImageEntity image, List<ImageEntity> images)
        {
            Title = "Image";

            _image = image;
            _images = images;
            _currentImageIndex = _images.FindIndex(x => x.Equals(image));

            _scrollView = new UIScrollView
            {
                Delegate = new ScrollViewDelegate(this)
            };
        }

        public override void ViewDidLoad()
        {
            View.BackgroundColor = UIColor.White;

            _imageView = new UIImageView(View.Frame)
            {
                MultipleTouchEnabled = true,
                UserInteractionEnabled = true,
                ContentMode = UIViewContentMode.ScaleAspectFit,
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight
            };
            PHAsset asset = _imageCache.GetAsset(_image.LocalIdentifier);

            UpdateImage(asset);

            _scrollView.AddSubview(_imageView);
            _scrollView.ContentSize = _imageView.Image.Size;

            var doubleTapRecognizer = new UITapGestureRecognizer(ScrollViewDoubleTapped)
            {
                NumberOfTapsRequired = 2,
                NumberOfTouchesRequired = 1
            };
            _scrollView.AddGestureRecognizer(doubleTapRecognizer);

            CenterScrollViewContents();
        }

        private void CenterScrollViewContents()
        {
            CGSize boundsSize = _scrollView.Bounds.Size;
            CGRect contentsFrame = _imageView.Frame;

            if (contentsFrame.Size.Width < boundsSize.Width)
            {
                contentsFrame.X = (nfloat)((boundsSize.Width - contentsFrame.Size.Width) / 2.0);
            }
            else
            {
                contentsFrame.X = 0;
            }

            if (contentsFrame.Size.Height < boundsSize.Height)
            {
                contentsFrame.Y = (nfloat)((boundsSize.Height - contentsFrame.Size.Height) / 2.0);
            }
            else
            {
                contentsFrame.Y = 0;
            }
            _imageView.Frame = contentsFrame;
        }

        private void ReplaseImage(UIImage image)
        {
            UIView.Transition(_imageView, 1, UIViewAnimationOptions.CurveLinear, () => _imageView.Image = image, null);
        }

        private void ScrollViewDoubleTapped(UITapGestureRecognizer recognizer)
        {
            CGPoint pointInView = recognizer.LocationInView(_imageView);

            double newZoomScale = _scrollView.ZoomScale * 1.5;
            newZoomScale = Math.Min(newZoomScale, _scrollView.MaximumZoomScale);

            CGSize scrollViewSize = _scrollView.Bounds.Size;
            double w = scrollViewSize.Width / newZoomScale;
            double h = scrollViewSize.Height / newZoomScale;
            double x = pointInView.X - (w / 2.0);
            double y = pointInView.Y - (h / 2.0);

            var rectToZoomTo = new CGRect(x, y, w, h);

            _scrollView.ZoomToRect(rectToZoomTo, true);
        }

        private void UpdateImage(PHAsset asset)
        {
            PHImageManager.DefaultManager.RequestImageForAsset(asset, View.Frame.Size,
                PHImageContentMode.AspectFit, new PHImageRequestOptions(),
                (image, info) => ReplaseImage(image));
        }


        private class ScrollViewDelegate : UIScrollViewDelegate
        {
            private readonly PhotoViewController2 _parentViewConroller;

            public ScrollViewDelegate(PhotoViewController2 parentViewConroller)
            {
                _parentViewConroller = parentViewConroller;
            }

            public override void DidZoom(UIScrollView scrollView)
            {
                _parentViewConroller.CenterScrollViewContents();
            }

            public override UIView ViewForZoomingInScrollView(UIScrollView scrollView)
            {
                return _parentViewConroller._imageView;
            }
        }
    }
}
