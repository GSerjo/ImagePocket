using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.Foundation;

namespace Dojo
{
	public class HomeViewController : UICollectionViewController
	{
		private static NSString _cellId = new NSString ("ImageCell");
		private static readonly ImageRepository _imageRepository = new ImageRepository();

		public HomeViewController (UICollectionViewLayout layout) : base(layout)
		{
			Title = "ImagePacket";
		}

		private void ConfigureView ()
		{
			CollectionView.RegisterClassForCell (typeof(ImpagePreviewCell), _cellId);
			CollectionView.Source = new CollectionSource (this);
			CollectionView.Delegate = new CollectionDelegate ();
		}

		private sealed class CollectionSource : UICollectionViewSource
		{

			private HomeViewController _controller;

			public CollectionSource (HomeViewController controller)
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
				Console.WriteLine (indexPath.Item);
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

