using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.Foundation;
using Domain;
using System.Reactive.Linq;
using Core;
using System.Collections.Generic;
using MonoTouch.CoreFoundation;

namespace Dojo
{
	public sealed class HomeViewController : UICollectionViewController
	{
		private static NSString _cellId = new NSString ("ImageCell");
		private UIBarButtonItem _btSelect, _btCancel, _btOpenMenu, _btTag;
		private const string RootTitle = "Bodo Title";
		private const string SelectImagesTitle = "Select images";
		private TagEntity _currentTag = TagEntity.All;
		private bool _shouldSelectItem;
		private List<ImageEntity> _images = new List<ImageEntity> ();
		private readonly ImageCache _imageCache = new ImageCache();
		private Dictionary<string, ImageEntity> _selectedImages = new Dictionary<string, ImageEntity> ();

		public HomeViewController (UICollectionViewLayout layout) : base(layout)
		{
			Title = RootTitle;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			ConfigureView ();
			ConfigureToolbar ();
			FilterImages ();
		}

		public void SetTag (TagEntity entity)
		{
			_currentTag = entity;
			FilterImages ();
			ReloadData ();
		}

		private void FilterImages()
		{
			_images = _imageCache.GetImages (_currentTag);
		}

		private void ReloadData()
		{
			DispatchQueue.MainQueue.DispatchAsync (() => CollectionView.ReloadData ());
		}

		private void ConfigureView ()
		{
			CollectionView.BackgroundColor = UIColor.White;
			CollectionView.RegisterClassForCell (typeof(ImagePreviewCell), _cellId);
		}

		private void ConfigureToolbar ()
		{
			_btSelect = new UIBarButtonItem ("Select", UIBarButtonItemStyle.Plain, OnBatchSelect);
			NavigationItem.RightBarButtonItem = _btSelect;

			_btCancel = new UIBarButtonItem (UIBarButtonSystemItem.Cancel);
			_btCancel.Clicked += OnBatchSelectCancel;

			_btTag = new UIBarButtonItem ("Tag", UIBarButtonItemStyle.Plain, OnTagClicked);
		}

		private void OnTagClicked(object sender, EventArgs ea)
		{
			var controller = new TagSelectorViewController ()
			{
				ModalPresentationStyle = UIModalPresentationStyle.FormSheet
			};
			controller.Closed += OnTagSelectorCancel;
			//controller.Done += OnTagSelectorDone;
			NavigationController.PresentViewController (controller, true, null);
//			NavigationController.PresentViewController (new UINavigationController (controller)
//				{
//					ModalPresentationStyle = UIModalPresentationStyle.FormSheet
//				}, true, null);
		}

		private void OnBatchSelect(object sender, EventArgs ea)
		{
			SetSelectMode ();
		}

		private void OnBatchSelectCancel(object sender, EventArgs ea)
		{
			SetReadMode ();
		}

		private void SetSelectMode()
		{
			Title = SelectImagesTitle;
			NavigationItem.RightBarButtonItem = _btCancel;
			_btOpenMenu = NavigationItem.LeftBarButtonItem;
			NavigationItem.LeftBarButtonItem = _btTag;
			_shouldSelectItem = true;
		}

		private void SetReadMode()
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
			var imageCell = (ImagePreviewCell)collectionView.DequeueReusableCell (_cellId, indexPath);
			ImageEntity imageEntiy = _images [indexPath.Item];
			UIImage image = _imageCache.GetSmallImage (imageEntiy.LocalIdentifier);
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
			var cell = (ImagePreviewCell)collectionView.CellForItem (indexPath);
			ImageEntity imageEntiy = _images [indexPath.Item];
			if (_selectedImages.ContainsKey (imageEntiy.LocalIdentifier))
			{
				_selectedImages.Remove (imageEntiy.LocalIdentifier);
				cell.BackgroundColor = UIColor.White;
				cell.Selected = false;
			} 
			else
			{
				_selectedImages [imageEntiy.LocalIdentifier] = imageEntiy;
				cell.BackgroundColor = UIColor.Black;
				cell.Selected = true;
			}
		}

		private void OnTagSelectorCancel(object sender, EventArgs ea)
		{
			SetReadMode ();
		}

		private void OnTagSelectorDone(object sender, EventArgs ea)
		{
			SetReadMode ();
		}
	}
}