using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.Foundation;
using Domain;
using System.Reactive.Linq;
using Core;
using System.Collections.Generic;

namespace Dojo
{
	public sealed class HomeViewController : UICollectionViewController
	{
		private static NSString _cellId = new NSString ("ImageCell");
		private static readonly AssetRepository _assetRepository = new AssetRepository();
		private UIBarButtonItem _btSelect, _btCancel, _btOpenMenu, _btTag;
		private const string RootTitle = "ImagePocket";
		private const string SelectImagesTitle = "Select images";
		private ImageRepository _imageRpository = new ImageRepository();
		private Bag<TagEntity> _currentTag = Bag<TagEntity>.Empty;
		private bool _shouldSelectItem;
		private List<ImageEntity> _images = new List<ImageEntity> ();

		public HomeViewController (UICollectionViewLayout layout) : base(layout)
		{
			Title = RootTitle;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			ConfigureView ();
			ConfigureToolbar ();
			_imageRpository.LoadAll ();
		}

		public void SetTag (TagEntity entity)
		{
			_currentTag = entity.ToBag();
			FilterImages ();
		}

		private void FilterImages()
		{
			if (_currentTag.HasNoValue)
			{
				return;
			}
			_images = _imageRpository.GetByTag (_currentTag.Value);
		}

		private void ConfigureView ()
		{
			CollectionView.BackgroundColor = UIColor.White;
			CollectionView.RegisterClassForCell (typeof(ImpagePreviewCell), _cellId);
		}

		private void ConfigureToolbar ()
		{
			_btSelect = new UIBarButtonItem ("Select", UIBarButtonItemStyle.Plain, OnSelectClicked);
			NavigationItem.RightBarButtonItem = _btSelect;

			_btCancel = new UIBarButtonItem (UIBarButtonSystemItem.Cancel);
			_btCancel.Clicked += OnCancelClicked;

			_btTag = new UIBarButtonItem ("Tag", UIBarButtonItemStyle.Plain, OnTagClicked);
		}

		private void OnTagClicked(object sender, EventArgs ea)
		{

		}

		private void OnSelectClicked(object sender, EventArgs ea)
		{
			Title = SelectImagesTitle;
			NavigationItem.RightBarButtonItem = _btCancel;
			_btOpenMenu = NavigationItem.LeftBarButtonItem;
			NavigationItem.LeftBarButtonItem = _btTag;
			_shouldSelectItem = true;
		}

		private void OnCancelClicked(object sender, EventArgs ea)
		{
			Title = RootTitle;
			NavigationItem.RightBarButtonItem = _btSelect;
			NavigationItem.LeftBarButtonItem = _btOpenMenu;
			_shouldSelectItem = false;
		}

		public override int GetItemsCount (UICollectionView collectionView, int section)
		{
			return _images.Count;
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var imageCell = (ImpagePreviewCell)collectionView.DequeueReusableCell (_cellId, indexPath);
			var image = _assetRepository.GetImage (indexPath.Item);
			imageCell.Image = image;
			return imageCell;
		}

		public override bool ShouldSelectItem (UICollectionView collectionView, NSIndexPath indexPath)
		{
			return _shouldSelectItem;
		}

		public override bool ShouldDeselectItem (UICollectionView collectionView, NSIndexPath indexPath)
		{
			return _shouldSelectItem;
		}

		public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = collectionView.CellForItem (indexPath);
			cell.BackgroundColor = UIColor.Black;
		}

		private sealed class CollectionSource : UICollectionViewSource
		{
			private HomeViewController _controller;

			public CollectionSource (HomeViewController controller)
			{
				_controller = controller;
			}

			public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
			{
				// NOTE: Don't call the base implementation on a Model class
				// see http://docs.xamarin.com/guides/ios/application_fundamentals/delegates,_protocols,_and_events
				var asset = _assetRepository.GetAsset (indexPath.Item);
				Console.WriteLine (indexPath.Item);
				var photoController = new PhotoViewController (asset);
				_controller.NavigationController.PushViewController (photoController, true);
			}
		}
	}
}