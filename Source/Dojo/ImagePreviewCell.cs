using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.Photos;
using Domain;

namespace Dojo
{
	public sealed class ImagePreviewCell : UICollectionViewCell
	{
		private UIImageView _imageView;
		private readonly ImageCache _imageCache = ImageCache.Instance;

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

		public void SetImage(string localId)
		{
			var asset = _imageCache.GetAsset (localId);
			_imageCache.ImageManager.RequestImageForAsset (asset, _imageView.Frame.Size, 
				PHImageContentMode.AspectFit, null, (img, info) => {
				_imageView.Image = img;
			});
		}

		public new void Select()
		{
			BackgroundColor = UIColor.Black;
			Selected = true;
		}

		public void Unselect()
		{
			BackgroundColor = UIColor.White;
			Selected = false;
		}
	}
}