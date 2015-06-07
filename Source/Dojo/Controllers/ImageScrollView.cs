using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using CoreGraphics;
using Domain;
using Photos;
using UIKit;

namespace Dojo
{
    public class ImageScrollView : UIScrollView
    {
        // turn on to use tiled images, if off, we use whole images
        private bool TileImagesMode = true;

        private CGSize _imageSize;
        //        private int _index;
        // if tiling this contains a very low-res placeholder image. otherwise it contains the full image.
        private CGPoint _pointToCenterAfterResize;
        private nfloat _scaleToRestoreAfterResize;
        //        private TilingView tilingView;
        private UIImageView zoomView;
        private ImageEntity _imageEntity;
        private readonly ImageCache _imageCache = ImageCache.Instance;

        public ImageScrollView()
        {
            ShowsVerticalScrollIndicator = false;
            ShowsHorizontalScrollIndicator = false;
            BouncesZoom = true;
            DecelerationRate = DecelerationRateFast;

            // Return the view to use when zooming
            ViewForZoomingInScrollView = sv => zoomView;
        }

        //        public static int ImageCount
        //        {
        //            get { return ImageData.Count; }
        //        }

        //        private static List<ImageDetails> ImageData
        //        {
        //            get
        //            {
        //                data = data ?? FetchImageData();
        //                return data;
        //            }
        //        }

        public ImageEntity ImageEntity
        {
            get { return _imageEntity; }
            set
            {
                _imageEntity = value;
                var asset = _imageCache.GetAsset(_imageEntity.LocalIdentifier);
                DisplayImage(GetImage(asset));
            }
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

        //        public int Index
        //        {
        //            get { return _index; }
        //            set
        //            {
        //                _index = value;
        //
        ////                if (TileImagesMode)
        ////                    DisplayTiledImageNamed(ImageNameAtIndex(_index), ImageSizeAtIndex(_index));
        ////                else
        //                    DisplayImage(ImageAtIndex(_index));
        //            }
        //        }

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

        private static List<ImageDetails> FetchImageData()
        {
            List<ImageDetails> result = null;
            string path = Path.Combine("Image", "ImageDetails.xml");

            try
            {
                using (TextReader reader = new StreamReader(path))
                {
                    var serializer = new XmlSerializer(typeof(List<ImageDetails>));
                    result = (List<ImageDetails>)serializer.Deserialize(reader);
                }
            }
            catch (XmlException e)
            {
                Console.WriteLine(e);
            }

            return result;
        }

        private static UIImage PlaceholderImageNamed(string name)
        {
            string placeholderName = string.Format("{0}_Placeholder", name);
            string placeholderNameWithExt = Path.ChangeExtension(placeholderName, "png");
            string fullName = Path.Combine("Image", "PlaceholderImages", placeholderNameWithExt);

            UIImage img = UIImage.FromBundle(fullName);
            return img;
        }

        // - Configure scrollView to display new image (tiled or not)

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

        private void DisplayTiledImageNamed(string imageName, CGSize image_Size)
        {
            //clear views for the previous image
            if (zoomView != null)
            {
                zoomView.RemoveFromSuperview();
                zoomView = null;
                //                tilingView = null;
            }
            ZoomScale = 1.0f;

            //make views to display the new image
            zoomView = new UIImageView(new CGRect(CGPoint.Empty, image_Size))
            {
                Image = PlaceholderImageNamed(imageName)
            };

            AddSubview(zoomView);
            //            tilingView = new TilingView(imageName, image_Size)
            //            {
            //                Frame = zoomView.Bounds
            //            };
            //
            //            zoomView.AddSubview(tilingView);
            ConfigureForImageSize(image_Size);
        }

        private UIImage GetImage(PHAsset asset)
        {
            UIImage result = null;
            var options = new PHImageRequestOptions
            {
                Synchronous = true,
            };
            //var size = new CGSize(asset.PixelWidth, asset.PixelHeight);
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

        //        private static UIImage ImageAtIndex(int index)
        //        {
        //            string imageName = ImageNameAtIndex(index);
        //            string imageNameWithExt = Path.ChangeExtension(imageName, "jpg");
        //            string fullImage = Path.Combine("Image", "FullImages", imageNameWithExt);
        //
        //            UIImage img = UIImage.FromBundle(fullImage);
        //            return img;
        //        }
    }
}
