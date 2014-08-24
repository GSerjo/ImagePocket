using System;
using System.Drawing;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Collections.ObjectModel;
using MonoTouch.CoreGraphics;

namespace Bodo
{
	public partial class DetailViewController : UIViewController
	{
		UIPopoverController masterPopoverController;
		private static NSString _cellId = new NSString ("ImageCell");
		private static readonly ImageRepository _imageRepository = new ImageRepository();

		public DetailViewController (IntPtr handle) : base (handle)
		{
		}

		private void ConfigureView ()
		{
			CollectionView.RegisterClassForCell (typeof(ImpagePreviewCell), _cellId);
			var layout = new UICollectionViewFlowLayout
			{
				MinimumInteritemSpacing = 10.0f,
				SectionInset = new UIEdgeInsets (10, 15, 10, 15),
				ItemSize = new SizeF(120, 120)
			};
			CollectionView.CollectionViewLayout = layout;

			CollectionView.Source = new CollectionSource (this);
			CollectionView.Delegate = new CollectionDelegate ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			ConfigureView ();
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
		}

		[Export ("splitViewController:willHideViewController:withBarButtonItem:forPopoverController:")]
		public void WillHideViewController (UISplitViewController splitController, UIViewController viewController, UIBarButtonItem barButtonItem, UIPopoverController popoverController)
		{
			barButtonItem.Title = NSBundle.MainBundle.LocalizedString ("Master", "Master");
			NavigationItem.SetLeftBarButtonItem (barButtonItem, true);
			masterPopoverController = popoverController;
		}

		[Export ("splitViewController:willShowViewController:invalidatingBarButtonItem:")]
		public void WillShowViewController (UISplitViewController svc, UIViewController vc, UIBarButtonItem button)
		{
			// Called when the view is shown again in the split view, invalidating the button and popover controller.
			NavigationItem.SetLeftBarButtonItem (null, true);
			masterPopoverController = null;
		}
			
		private sealed class CollectionSource : UICollectionViewSource
		{

			private DetailViewController _controller;

			public CollectionSource (DetailViewController controller)
			{
				_controller = controller;
			}

			public override int GetItemsCount (UICollectionView collectionView, int section)
			{
				// NOTE: Don't call the base implementation on a Model class
				// see http://docs.xamarin.com/guides/ios/application_fundamentals/delegates,_protocols,_and_events
				return _imageRepository.ImageCount;
			}

			public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
			{
				var imageCell = (ImpagePreviewCell)collectionView.DequeueReusableCell (_cellId, indexPath);
				var image = _imageRepository.GetImage (indexPath.Item);
				imageCell.Image = image;
				return imageCell;
			}

			public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
			{
				// NOTE: Don't call the base implementation on a Model class
				// see http://docs.xamarin.com/guides/ios/application_fundamentals/delegates,_protocols,_and_events
				var asset = _imageRepository.GetAsset (indexPath.Item);
				var photoController = new PhotoViewController (asset);
				_controller.NavigationController.PushViewController (photoController, true);
			}
		}

		private sealed class CollectionDelegate : UICollectionViewDelegate
		{
			public override bool ShouldSelectItem (UICollectionView collectionView, NSIndexPath indexPath)
			{
				return true;
			}

			public override bool ShouldDeselectItem (UICollectionView collectionView, NSIndexPath indexPath)
			{
				return true;
			}

			public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
			{
				var cell = collectionView.CellForItem (indexPath);
				cell.BackgroundColor = UIColor.Black;
			}
		}
	}
}

