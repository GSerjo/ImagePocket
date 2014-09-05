using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.Foundation;

namespace Dojo
{
	public sealed class ImagePreviewCell : UICollectionViewCell
	{
		private UIImageView _imageView;

		[Export("initWithFrame:")]
		public ImagePreviewCell(RectangleF frame) : base(frame)
		{
			_imageView = new UIImageView(frame);
//			_imageView.Center = ContentView.Center;
//			_imageView.ContentMode = UIViewContentMode.ScaleAspectFit;


			_imageView.ContentMode = UIViewContentMode.ScaleAspectFill;
			_imageView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;
			_imageView.ClipsToBounds = true;
			ContentView.AddSubview(_imageView);
		}

		public UIImage Image
		{
			set
			{
				_imageView.Frame = new RectangleF (0, 0, ContentView.Bounds.Width, ContentView.Bounds.Height);
				_imageView.Image = value;
			}
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