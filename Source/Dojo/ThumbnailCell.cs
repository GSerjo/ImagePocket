
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;

namespace Dojo
{
	public partial class ThumbnailCell : UICollectionViewCell
	{
		private UIImageView _imageView;
		public static readonly UINib Nib = UINib.FromName ("ThumbnailCell", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString ("ThumbnailCell");

		public ThumbnailCell (IntPtr handle) : base (handle)
		{
		}

		[Export("initWithFrame:")]
		public ThumbnailCell(RectangleF frame) : base(frame)
		{
			_imageView = new UIImageView(frame);
			_imageView.Center = ContentView.Center;
			_imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			_imageView.Transform = CGAffineTransform.MakeScale (0.8f, 0.8f);

			ContentView.AddSubview(_imageView);
		}

		public static ThumbnailCell Create ()
		{
			return (ThumbnailCell)Nib.Instantiate (null, null) [0];
		}

		public UIImage Image
		{
			set { _imageView.Image = value; }
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

