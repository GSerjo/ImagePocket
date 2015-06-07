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
        private UIImageView _imageView;
        private UIScrollView _scrollView;

        public PhotoViewController2(ImageEntity image, List<ImageEntity> images)
        {
            Title = "Image";

            _image = image;
        }

        public override void ViewDidLoad()
        {
            View.BackgroundColor = UIColor.White;
            PHAsset asset = _imageCache.GetAsset(_image.LocalIdentifier);

            UIImage image = GetImage(asset);

            _imageView = new UIImageView(image)
            {
                MultipleTouchEnabled = true,
                UserInteractionEnabled = true,
                ContentMode = UIViewContentMode.ScaleAspectFit,
                Frame = new CGRect(new CGPoint(0, 0), image.Size),
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight
            };

            _scrollView = new UIScrollView
            {
                //				Delegate = new ScrollViewDelegate(this),
                Frame = View.Frame,
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight
            };
            _scrollView.ContentSize = image.Size;

            _scrollView.ViewForZoomingInScrollView += ScrollView_ViewForZoomingInScrollView;
            _scrollView.DidZoom += ScrollView_DidZoom;

            _scrollView.AddSubview(_imageView);
            View.AddSubview(_scrollView);

            //			UpdateImage(asset);

            var doubleTapRecognizer = new UITapGestureRecognizer(ScrollViewDoubleTapped)
            {
                NumberOfTapsRequired = 2,
                NumberOfTouchesRequired = 1
            };
            _scrollView.AddGestureRecognizer(doubleTapRecognizer);

            CGRect scrollViewFrame = _scrollView.Frame;
            nfloat scaleWidth = scrollViewFrame.Size.Width / _scrollView.ContentSize.Width;
            nfloat scaleHeight = scrollViewFrame.Size.Height / _scrollView.ContentSize.Height;
            double minScale = Math.Min(scaleWidth, scaleHeight);
            _scrollView.MinimumZoomScale = (nfloat)minScale;

            _scrollView.MaximumZoomScale = 1;
            _scrollView.ZoomScale = (nfloat)minScale;

            //CenterScrollViewContents();
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
                nfloat t = NavigationController.NavigationBar.Bounds.Size.Height;
                nfloat t1 = UIApplication.SharedApplication.StatusBarFrame.Size.Height;
                contentsFrame.Y = (nfloat)((boundsSize.Height - t - t1 - 30 - contentsFrame.Size.Height) / 2.0);
            }
            else
            {
                contentsFrame.Y = 0;
            }
            _imageView.Frame = contentsFrame;
        }

        private UIImage GetImage(PHAsset asset)
        {
            UIImage result = null;
            var options = new PHImageRequestOptions
            {
                Synchronous = true,
            };
            var size = new CGSize(asset.PixelWidth, asset.PixelHeight);
            PHImageManager.DefaultManager.RequestImageForAsset(asset, size,
                PHImageContentMode.AspectFit, options,
                (image, info) => result = image);
            return result;
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
            if (_scrollView.ContentSize.Height > View.Frame.Height && _scrollView.ContentSize.Width > View.Frame.Width)
            {
                _scrollView.ZoomToRect(View.Frame, true);
            }
            else
            {
                _scrollView.ZoomToRect(rectToZoomTo, true);
            }
        }

        private void ScrollView_DidZoom(object sender, EventArgs e)
        {
            CenterScrollViewContents();
        }

        private UIView ScrollView_ViewForZoomingInScrollView(UIScrollView scrollView)
        {
            return _imageView;
        }

        private void UpdateImage(PHAsset asset)
        {
            var options = new PHImageRequestOptions
            {
                Synchronous = true,
            };
            var size = new CGSize(asset.PixelWidth, asset.PixelHeight);
            PHImageManager.DefaultManager.RequestImageForAsset(asset, size,
                PHImageContentMode.AspectFit, options,
                (image, info) => ReplaseImage(image));
        }

        //        private class ScrollViewDelegate : UIScrollViewDelegate
        //        {
        //            private readonly PhotoViewController2 _parentViewConroller;
        //
        //            public ScrollViewDelegate(PhotoViewController2 parentViewConroller)
        //            {
        //                _parentViewConroller = parentViewConroller;
        //            }
        //
        //            public override void DidZoom(UIScrollView scrollView)
        //            {
        //                _parentViewConroller.CenterScrollViewContents();
        //            }
        //
        //            public override UIView ViewForZoomingInScrollView(UIScrollView scrollView)
        //            {
        //                return _parentViewConroller._imageView;
        //            }
        //        }
    }
}
