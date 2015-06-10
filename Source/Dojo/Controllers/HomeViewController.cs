using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core;
using CoreGraphics;
using Domain;
using Foundation;
using Photos;
using UIKit;

namespace Dojo
{
    public sealed class HomeViewController : UICollectionViewController
    {
        private const string RootTitle = "Image Pocket";
        private const string SelectButtonName = "Select";
        private const string SelectImagesTitle = "Select images";
        private const string TagButonName = "Tag";
        private static readonly NSString _cellId = new NSString("ImageCell");
        private UIBarButtonItem _btCancel;
        private UIBarButtonItem _btOpenMenu;
        private UIBarButtonItem _btSelect;
        private UIBarButtonItem _btShare;
        private UIBarButtonItem _btTag;
        private UIBarButtonItem _btTrash;
        private TagEntity _currentTag = TagEntity.All;
        private List<ImageEntity> _filteredImages = new List<ImageEntity>();
        private ImageCache _imageCache;
        private Dictionary<string, ImageEntity> _selectedImages = new Dictionary<string, ImageEntity>();
        private UIPopoverController _shareController;
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
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = (ImagePreviewCell)collectionView.DequeueReusableCell(_cellId, indexPath);
            ImageEntity entity = _filteredImages[(int)indexPath.Item];
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

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return _filteredImages.Count;
        }

        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            if (_viewMode == ViewMode.Read)
            {
                ImageEntity image = _filteredImages[(int)indexPath.Item];

//                var photoController = new PhotoViewController(image, _filteredImages);
//                NavigationController.PushViewController(photoController, true);

//                                var pageViewController = new PageViewController(image, _filteredImages);
                //                NavigationController.PushViewController(pageViewController, true);

//                                var viewController = new PhotoViewController1(image, _filteredImages);
//                                NavigationController.PushViewController(viewController, false);

//                var viewController = new PhotoViewController2(image, _filteredImages);
//                NavigationController.PushViewController(viewController, false);

                var viewController = new PhotoViewController4(image, _filteredImages);
                NavigationController.PushViewController(viewController, false);
                return;
            }
            var cell = (ImagePreviewCell)collectionView.CellForItem(indexPath);
            ImageEntity entity = _filteredImages[(int)indexPath.Item];

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
            _btShare.Enabled = _selectedImages.IsNotEmpty();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            if (PHPhotoLibrary.AuthorizationStatus == PHAuthorizationStatus.Authorized)
            {
                AfterViewDidLoad();
            }
            else
            {
                PHPhotoLibrary.RequestAuthorization(x =>
                {
                    PHAuthorizationStatus result = x;
                    if (result == PHAuthorizationStatus.Authorized)
                    {
                        BeginInvokeOnMainThread(() =>
                        {
                            AfterViewDidLoad();
                            FilterImages();
                            ReloadData();
                        });
                    }
                });
            }
        }

        public override void ViewWillAppear(bool animated)
        {
            NavigationController.SetToolbarHidden(false, false);
            FilterImages();
            ReloadData();
            base.ViewWillAppear(animated);
        }

        private void AfterViewDidLoad()
        {
            _imageCache = ImageCache.Instance;
            ConfigureView();
            ConfigureToolbar();
            _imageCache.PhotoLibraryChanged += OnPhotoLibraryChanged;
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

            _btShare = new UIBarButtonItem(UIBarButtonSystemItem.Action, OnShareClicked);

            _btTrash = new UIBarButtonItem(UIBarButtonSystemItem.Trash, OnTrashClicked);
            var deleteSpace = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
            ToolbarItems = new[] { _btShare, deleteSpace, _btTrash };
            _btTrash.Enabled = false;
            _btShare.Enabled = false;
        }

        private void ConfigureView()
        {
            CollectionView.BackgroundColor = UIColor.White;
            CollectionView.RegisterClassForCell(typeof(ImagePreviewCell), _cellId);
        }

        private void FilterImages()
        {
            if (_imageCache == null)
            {
                return;
            }
            _filteredImages = _imageCache.GetImages(_currentTag);
            if (_filteredImages.IsNullOrEmpty())
            {
                _filteredImages = _imageCache.GetImages(TagEntity.All);
            }
        }

        private Task<List<UIImage>> GetSharedImages()
        {
            List<PHAsset> assets = _selectedImages.Values.Select(x => _imageCache.GetAsset(x.LocalIdentifier)).ToList();
            var images = new List<UIImage>();
            var options = new PHImageRequestOptions
            {
                Synchronous = true,
                DeliveryMode = PHImageRequestOptionsDeliveryMode.HighQualityFormat
            };
            return Task.Run(() =>
            {
                foreach (PHAsset asset in assets)
                {
                    var size = new CGSize(asset.PixelWidth, asset.PixelHeight);
                    PHImageManager.DefaultManager.RequestImageForAsset(asset,
                        size,
                        PHImageContentMode.AspectFit,
                        options,
                        (image, info) => images.Add(image));
                }
                return images;
            });
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

        private async void OnShareClicked(object sender, EventArgs ea)
        {
            if (_shareController == null)
            {
                List<UIImage> images = await GetSharedImages();
                if (images.IsNullOrEmpty())
                {
                    return;
                }
                var activityController = new UIActivityViewController(images.ToArray(), null);
                activityController.SetCompletionHandler(
                    (activityType, completed, returnedItems, error) =>
                    {
                        if (completed)
                        {
                            OnTagSelectorCancel();
                        }
                        _shareController = null;
                    });
                _shareController = new UIPopoverController(activityController);
                _shareController.DidDismiss += (s, e) => { _shareController = null; };
                _shareController.PresentFromBarButtonItem(_btShare, UIPopoverArrowDirection.Up, true);
            }
            else
            {
                _shareController.Dismiss(true);
                _shareController = null;
            }
        }

        private void OnTagClicked(object sender, EventArgs ea)
        {
            var controller = new TagSelectorViewController(_selectedImages.Values.ToList())
            {
                ModalPresentationStyle = UIModalPresentationStyle.FormSheet
            };
            controller.Cancel += (s, e) => OnTagSelectorCancel();
            controller.Done += OnTagSelectorDone;
            NavigationController.PresentViewController(controller, true, null);
        }

        private void OnTagSelectorCancel()
        {
            SetReadMode();
            ReloadData();
        }

        private void OnTagSelectorDone(object sender, EventArgsOf<List<ImageEntity>> ea)
        {
            SetReadMode();
            _imageCache.SaveOrUpdate(ea.Data);
            FilterImages();
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
            InvokeOnMainThread(CollectionView.ReloadData);
        }

        private void SetReadMode()
        {
            Title = RootTitle;
            NavigationItem.RightBarButtonItem = _btSelect;
            NavigationItem.LeftBarButtonItem = _btOpenMenu;
            _btTrash.Enabled = false;
            _btShare.Enabled = false;
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
