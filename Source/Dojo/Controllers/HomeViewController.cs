using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Domain;
using MonoTouch.CoreFoundation;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Dojo
{
    public sealed class HomeViewController : UICollectionViewController
    {
        private const string RootTitle = "Image Pocket";
        private const string SelectImagesTitle = "Select images";
        private static readonly NSString _cellId = new NSString("ImageCell");
        private readonly ImageCache _imageCache = ImageCache.Instance;
        private UIBarButtonItem _btCancel, _btOpenMenu;
        private UIBarButtonItem _btSelect;
        private UIBarButtonItem _btTag;
        private TagEntity _currentTag = TagEntity.All;
        private List<ImageEntity> _images = new List<ImageEntity>();
        private Dictionary<string, ImageEntity> _selectedImages = new Dictionary<string, ImageEntity>();
        private ViewMode _viewMode = ViewMode.Read;

        public HomeViewController(UICollectionViewLayout layout) : base(layout)
        {
            Title = RootTitle;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = (ImagePreviewCell)collectionView.DequeueReusableCell(_cellId, indexPath);
            ImageEntity entity = _images[indexPath.Item];
            cell.SetImage(entity.LocalIdentifier);

            if (_viewMode == ViewMode.Read)
            {
                cell.Unselect();
            }
            else if (_selectedImages.ContainsKey(entity.LocalIdentifier))
            {
                cell.SelectCell();
            }
            else
            {
                cell.Unselect();
            }
            return cell;
        }

        public override int GetItemsCount(UICollectionView collectionView, int section)
        {
            return _images.Count;
        }

        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            if (_viewMode == ViewMode.Read)
            {
                ImageEntity image = _images[indexPath.Item];
                var photoController = new PhotoViewController(image);
                NavigationController.PushViewController(photoController, true);
                return;
            }
            var cell = (ImagePreviewCell)collectionView.CellForItem(indexPath);
            ImageEntity entity = _images[indexPath.Item];

            ImageEntity selectedImage;
            if (_selectedImages.TryGetValue(entity.LocalIdentifier, out selectedImage))
            {
                _selectedImages.Remove(entity.LocalIdentifier);
                cell.Unselect();
            }
            else
            {
                _selectedImages[entity.LocalIdentifier] = entity;
                cell.SelectCell();
            }

            NavigationItem.LeftBarButtonItem.Enabled = _selectedImages.IsNotEmpty();
        }

        public void SetTag(TagEntity entity)
        {
            _currentTag = entity;
            FilterImages();
            ReloadData();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            ConfigureView();
            ConfigureToolbar();
        }

        public override void ViewWillAppear(bool animated)
        {
            FilterImages();
            base.ViewWillAppear(animated);
            ReloadData();
        }

        private void ClearSelectedCells()
        {
            _selectedImages = new Dictionary<string, ImageEntity>();
        }

        private void ConfigureToolbar()
        {
            _btSelect = new UIBarButtonItem("Select", UIBarButtonItemStyle.Plain, OnBatchSelect);
            NavigationItem.RightBarButtonItem = _btSelect;

            _btCancel = new UIBarButtonItem(UIBarButtonSystemItem.Cancel);
            _btCancel.Clicked += OnBatchSelectCancel;

            _btTag = new UIBarButtonItem("Tag", UIBarButtonItemStyle.Plain, OnTagClicked);
        }

        private void ConfigureView()
        {
            CollectionView.BackgroundColor = UIColor.White;
            CollectionView.RegisterClassForCell(typeof(ImagePreviewCell), _cellId);
        }

        private void FilterImages()
        {
            _images = _imageCache.GetImages(_currentTag);
            if (_images.IsNullOrEmpty())
            {
                _images = _imageCache.GetImages(TagEntity.All);
            }
        }

        private void OnBatchSelect(object sender, EventArgs ea)
        {
            SetSelectMode();
        }

        private void OnBatchSelectCancel(object sender, EventArgs ea)
        {
            SetReadMode();
        }

        private void OnTagClicked(object sender, EventArgs ea)
        {
            var controller = new TagSelectorViewController(_selectedImages.Values.ToList())
            {
                ModalPresentationStyle = UIModalPresentationStyle.FormSheet
            };
            controller.Cancel += OnTagSelectorCancel;
            controller.Done += OnTagSelectorDone;
            NavigationController.PresentViewController(controller, true, null);
        }

        private void OnTagSelectorCancel(object sender, EventArgs ea)
        {
            SetReadMode();
        }

        private void OnTagSelectorDone(object sender, EventArgsOf<List<ImageEntity>> ea)
        {
            SetReadMode();
            _imageCache.SaveOrUpdate(ea.Data);
        }

        private void ReloadData()
        {
            DispatchQueue.MainQueue.DispatchAsync(() => CollectionView.ReloadData());
        }

        private void SetReadMode()
        {
            Title = RootTitle;
            NavigationItem.RightBarButtonItem = _btSelect;
            NavigationItem.LeftBarButtonItem = _btOpenMenu;
            _viewMode = ViewMode.Read;
            ClearSelectedCells();
            ReloadData();
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

        //		private sealed class CollectionViewDelegate : CollectionViewDelegateWaterfallLayout
        //		{
        //			private Dictionary<int, Bag<SizeF>> _sizes = new Dictionary<int, Bag<SizeF>> ();
        //			private HomeViewController _controller;
        //
        //			public CollectionViewDelegate (HomeViewController controller)
        //			{
        //				_controller = controller;
        //			}
        //
        //			public override Bag<SizeF> CollectionView (UICollectionView collectionView, UICollectionViewLayout collectionViewLayout, NSIndexPath sizeForItemAtIndexPath)
        //			{
        //				int itemKey = sizeForItemAtIndexPath.Item;
        //				if (_sizes.ContainsKey (itemKey))
        //				{
        //					return _sizes [itemKey];
        //				}
        //				ImageEntity entity = _controller._images [sizeForItemAtIndexPath.Item];
        //				var size = _controller._imageCache.GetImageSize (entity.LocalIdentifier);
        //				_sizes[itemKey] = size.ToBag ();
        //				return _sizes [itemKey];
        //			}
        //		}
    }
}
