using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Domain;
using MonoTouch.CoreFoundation;
using MonoTouch.Foundation;
using MonoTouch.Photos;
using MonoTouch.UIKit;

namespace Dojo
{
    public sealed class HomeViewController : UICollectionViewController
    {
        private const string RootTitle = "Image Pocket";
        private const string SelectButtonName = "Select";
        private const string SelectImagesTitle = "Select images";
        private const string TagButonName = "Tag";
        private static readonly NSString _cellId = new NSString("ImageCell");
        private readonly ImageCache _imageCache = ImageCache.Instance;
        private UIBarButtonItem _btCancel, _btOpenMenu;
        private UIBarButtonItem _btSelect;
        private UIBarButtonItem _btTag;
        private UIBarButtonItem _btTrash;
        private TagEntity _currentTag = TagEntity.All;
        private List<ImageEntity> _filteredImages = new List<ImageEntity>();
        private Dictionary<string, ImageEntity> _selectedImages = new Dictionary<string, ImageEntity>();
        private ViewMode _viewMode = ViewMode.Read;

        public HomeViewController(UICollectionViewLayout layout) : base(layout)
        {
            Title = RootTitle;
        }


        private enum ViewMode
        {
            Read,
            Select
        }


        public void FilterImages(TagEntity entity)
        {
            _currentTag = entity;
            FilterImages();
            ReloadData();
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = (ImagePreviewCell)collectionView.DequeueReusableCell(_cellId, indexPath);
            ImageEntity entity = _filteredImages[indexPath.Item];
            cell.SetImage(entity.LocalIdentifier);

            if (_viewMode == ViewMode.Read)
            {
                cell.UnselectCell();
            }
            else if (_selectedImages.ContainsKey(entity.LocalIdentifier))
            {
                cell.SelectCell();
            }
            else
            {
                cell.UnselectCell();
            }
            return cell;
        }

        public override int GetItemsCount(UICollectionView collectionView, int section)
        {
            return _filteredImages.Count;
        }

        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            if (_viewMode == ViewMode.Read)
            {
                ImageEntity image = _filteredImages[indexPath.Item];
                var photoController = new PhotoViewController(image, _filteredImages);
                NavigationController.PushViewController(photoController, true);
                return;
            }
            var cell = (ImagePreviewCell)collectionView.CellForItem(indexPath);
            ImageEntity entity = _filteredImages[indexPath.Item];

            ImageEntity selectedImage;
            if (_selectedImages.TryGetValue(entity.LocalIdentifier, out selectedImage))
            {
                _selectedImages.Remove(entity.LocalIdentifier);
                cell.UnselectCell();
            }
            else
            {
                _selectedImages[entity.LocalIdentifier] = entity;
                cell.SelectCell();
            }

            NavigationItem.LeftBarButtonItem.Enabled = _selectedImages.IsNotEmpty();
            _btTrash.Enabled = _selectedImages.IsNotEmpty();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            ConfigureView();
            ConfigureToolbar();
            _imageCache.PhotoLibraryChanged += OnPhotoLibraryChanged;
        }

        public override void ViewWillAppear(bool animated)
        {
            NavigationController.SetToolbarHidden(false, true);
            FilterImages();
            ReloadData();
            base.ViewWillAppear(animated);
        }

        private void ClearSelectedCells()
        {
            _selectedImages = new Dictionary<string, ImageEntity>();
        }

        private void ConfigureToolbar()
        {
            _btSelect = new UIBarButtonItem(SelectButtonName, UIBarButtonItemStyle.Plain, OnBatchSelect);
            NavigationItem.RightBarButtonItem = _btSelect;

            _btCancel = new UIBarButtonItem(UIBarButtonSystemItem.Cancel, OnBatchSelectCancel);
            _btTag = new UIBarButtonItem(TagButonName, UIBarButtonItemStyle.Plain, OnTagClicked);

            _btTrash = new UIBarButtonItem(UIBarButtonSystemItem.Trash, OnTrashClicked);
            var deleteSpace = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
            ToolbarItems = new[] { deleteSpace, _btTrash };
            _btTrash.Enabled = false;
        }

        private void ConfigureView()
        {
            CollectionView.BackgroundColor = UIColor.White;
            CollectionView.RegisterClassForCell(typeof(ImagePreviewCell), _cellId);
        }

        private void FilterImages()
        {
            _filteredImages = _imageCache.GetImages(_currentTag);
            if (_filteredImages.IsNullOrEmpty())
            {
                _filteredImages = _imageCache.GetImages(TagEntity.All);
            }
        }

        private void OnBatchSelect(object sender, EventArgs ea)
        {
            SetSelectMode();
        }

        private void OnBatchSelectCancel(object sender, EventArgs ea)
        {
            SetReadMode();
            ReloadData();
        }

        private void OnDeleteAssetsCompleted(List<ImageEntity> removedImages, bool result, NSError error)
        {
            if (result)
            {
                _imageCache.Remove(removedImages);
                BeginInvokeOnMainThread(SetReadMode);
                //                FilterImages();
                //                DispatchQueue.MainQueue.DispatchAsync(() =>
                //                {
                //                    SetReadMode();
                //                    ReloadData();
                //                });
            }
            else
            {
                Console.WriteLine(error);
            }
        }

        private void OnPhotoLibraryChanged(object sender, EventArgs e)
        {
            FilterImages();
            BeginInvokeOnMainThread(ReloadData);
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
            FilterImages();
            ReloadData();
        }

        private void OnTagSelectorDone(object sender, EventArgsOf<List<ImageEntity>> ea)
        {
            SetReadMode();
            _imageCache.SaveOrUpdate(ea.Data);
            ReloadData();
        }

        private void OnTrashClicked(object sender, EventArgs e)
        {
            try
            {
                PHAsset[] assets = _selectedImages.Values.Select(x => x.LocalIdentifier)
                                                  .Select(x => _imageCache.GetAsset(x))
                                                  .ToArray();
                if (assets.IsNullOrEmpty())
                {
                    return;
                }
                PHPhotoLibrary.SharedPhotoLibrary.PerformChanges(
                    () => PHAssetChangeRequest.DeleteAssets(assets),
                    (result, error) => OnDeleteAssetsCompleted(_selectedImages.Values.ToList(), result, error));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void ReloadData()
        {
            DispatchQueue.MainQueue.DispatchAsync(CollectionView.ReloadData);
        }

        private void SetReadMode()
        {
            Title = RootTitle;
            NavigationItem.RightBarButtonItem = _btSelect;
            NavigationItem.LeftBarButtonItem = _btOpenMenu;
            _btTrash.Enabled = false;
            _viewMode = ViewMode.Read;
            ClearSelectedCells();
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
    }
}
