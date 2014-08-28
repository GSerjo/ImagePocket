using System;
using MonoTouch.UIKit;
using MonoTouch.Photos;
using System.Drawing;
using Domain;
using Core;
using System.Collections.Generic;
using System.Linq;

namespace Dojo
{
	public sealed class PhotoViewController : UIViewController
	{
		private readonly PHAsset _asset;
		private readonly UIBarButtonItem _tabButton;
		private UIImageView _imageView;
		private bool _fullScreen;
		private ImageEntity _image;
		private AssetRepository _assetRepository = AssetRepository.Instance;
		private readonly ImageCache _imageCache = ImageCache.Instance;

		public PhotoViewController (ImageEntity image)
		{
			Title = "Image";
			_image = image;
			_asset = _assetRepository.GetAsset (image.LocalIdentifier);
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
			var tapGesture = new UITapGestureRecognizer (OnViewTap);
			View.AddGestureRecognizer (tapGesture);
		}

		private void OnViewTap(UITapGestureRecognizer gesture)
		{
			_fullScreen = !_fullScreen;
			NavigationController.SetNavigationBarHidden (_fullScreen, false);
			View.BackgroundColor = _fullScreen ? UIColor.Black : UIColor.White;
		}

		private void OnTagClicked(object sender, EventArgs ea)
		{
			var controller = new TagSelectorViewController (_image)
			{
				ModalPresentationStyle = UIModalPresentationStyle.FormSheet
			};
			controller.Done += OnTagSelectorDone;
			NavigationController.PresentViewController (controller, true, null);
		}

		private void OnTagSelectorDone(object sender, EventArgsOf<List<ImageEntity>> ea)
		{
			_imageCache.SaveOrUpdate (ea.Data);
			_image = ea.Data.First();
		}
	}
}