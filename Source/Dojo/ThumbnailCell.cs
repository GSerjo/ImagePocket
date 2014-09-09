
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using Domain;
using MonoTouch.Photos;

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
			frame = new RectangleF (0, 0, ContentView.Bounds.Width, ContentView.Bounds.Height);
			_imageView = new UIImageView(frame);
			_imageView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;
			_imageView.ClipsToBounds = true;
			_imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			ContentView.AddSubview (_imageView);
		}

		public static ThumbnailCell Create ()
		{
			return (ThumbnailCell)Nib.Instantiate (null, null) [0];
		}

		public void SetImage(string localId, PHCachingImageManager manager)
		{
			var asset = AssetRepository.Instance.GetAsset (localId);
			manager.RequestImageForAsset (asset, _imageView.Frame.Size, 
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

