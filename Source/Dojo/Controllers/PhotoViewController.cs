using System;
using MonoTouch.UIKit;
using MonoTouch.Photos;
using System.Drawing;

namespace Dojo
{
	public class PhotoViewController : UIViewController
	{
		private readonly PHAsset _asset;
		private readonly UIBarButtonItem _tabButton;
		private UIImageView _imageView;

		public PhotoViewController (PHAsset asset)
		{
			_asset = asset;
			_tabButton = new UIBarButtonItem ("Tag", UIBarButtonItemStyle.Plain, OnTagClicked);
			NavigationItem.RightBarButtonItem = _tabButton;
		}

		public override void ViewDidLoad ()
		{
			View.BackgroundColor = UIColor.White;
			_imageView = new UIImageView (View.Frame);
			PHImageManager.DefaultManager.RequestImageForAsset (_asset, View.Frame.Size, 
				PHImageContentMode.AspectFit, new PHImageRequestOptions (), (img, info) => {
					_imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
					_imageView.Image = img;
			});
			View.AddSubview (_imageView);
		}

		private void OnTagClicked(object sender, EventArgs ea)
		{
		}

	}
}

