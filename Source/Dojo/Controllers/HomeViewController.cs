using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.Foundation;
using Domain;
using Core;
using System.Collections.Generic;
using MonoTouch.CoreFoundation;
using System.Linq;
using MonoTouch.Photos;

namespace Dojo
{
	public sealed class HomeViewController : UICollectionViewController
	{
		private static NSString _cellId = new NSString ("ImageCell");
		private UIBarButtonItem _btSelect, _btCancel, _btOpenMenu, _btTag;
		private const string RootTitle = "Bodo Title";
		private const string SelectImagesTitle = "Select images";
		private TagEntity _currentTag = TagEntity.All;
		private List<ImageEntity> _images = new List<ImageEntity> ();
		private readonly ImageCache _imageCache = ImageCache.Instance;
		private ViewMode _viewMode = ViewMode.Read;
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

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
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
			var controller = new TagSelectorViewController (_selectedImages.Values.ToList ())
			{
				ModalPresentationStyle = UIModalPresentationStyle.FormSheet
			};
			controller.Cancel += OnTagSelectorCancel;
			controller.Done += OnTagSelectorDone;
			NavigationController.PresentViewController (controller, true, null);
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
			NavigationItem.LeftBarButtonItem.Enabled = false;
			_viewMode = ViewMode.Select;
		}

		private void SetReadMode()
		{
			Title = RootTitle;
			NavigationItem.RightBarButtonItem = _btSelect;
			NavigationItem.LeftBarButtonItem = _btOpenMenu;
			_viewMode = ViewMode.Read;
			ClearSelectedCells ();
			ReloadData ();
		}

		public override int GetItemsCount (UICollectionView collectionView, int section)
		{
			return _images.Count;
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = (ImagePreviewCell)collectionView.DequeueReusableCell (_cellId, indexPath);
			ImageEntity entity = _images [indexPath.Item];
			UIImage image = _imageCache.GetSmallImage (entity.LocalIdentifier);
			cell.Image = image;
			UpdateSelectCellStatus (cell, entity);
			return cell;
		}

		public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
		{
			if (_viewMode == ViewMode.Read)
			{
				ImageEntity image = _images [indexPath.Item];
				var photoController = new PhotoViewController (image);
				NavigationController.PushViewController (photoController, true);
				return;
			}
			var cell = (ImagePreviewCell)collectionView.CellForItem (indexPath);
			ImageEntity entity = _images [indexPath.Item];
			UpdateSelectCellStatus (cell, entity);
			NavigationItem.LeftBarButtonItem.Enabled = _selectedImages.IsNotEmpty ();
		}

		private void UpdateSelectCellStatus(ImagePreviewCell cell, ImageEntity entity)
		{
			if (_viewMode == ViewMode.Read)
			{
				cell.Unselect ();
				return;
			}
			if (IsCellSelected(entity))
			{
				_selectedImages.Remove(entity.LocalIdentifier);
				cell.Unselect ();
			} 
			else
			{
				_selectedImages [entity.LocalIdentifier] = entity;
				cell.Select ();
			}
		}

		private void OnTagSelectorCancel(object sender, EventArgs ea)
		{
			SetReadMode ();
		}

		private void OnTagSelectorDone(object sender, EventArgsOf<List<ImageEntity>> ea)
		{
			SetReadMode ();
			_imageCache.SaveOrUpdate (ea.Data);
		}

		private void ClearSelectedCells()
		{
			_selectedImages = new Dictionary<string, ImageEntity> ();
//			CollectionView.VisibleCells.Cast<ImagePreviewCell> ()
//				.Where(x=>x.Selected)
//				.Iter(x => x.Unselect ());
		}

		private bool IsCellSelected(ImageEntity entity)
		{
			return _selectedImages.ContainsKey (entity.LocalIdentifier);
		}
	}
}