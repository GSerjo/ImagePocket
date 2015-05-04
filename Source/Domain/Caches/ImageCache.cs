using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Core;
using MonoTouch.Photos;

namespace Domain
{
    public sealed class ImageCache
    {
        private static readonly ImageCache _instance = new ImageCache();
        private readonly PHCachingImageManager _imageManager = new PHCachingImageManager();
        private readonly ImageRepository _imageRepository = ImageRepository.Instance;
        private readonly object _locker = new object();
        private readonly PhotoLibraryObserver _photoLibraryObserver;
        private readonly Dictionary<string, ImageEntity> _taggedImages = new Dictionary<string, ImageEntity>();
        private Dictionary<string, ImageEntity> _actualImages = new Dictionary<string, ImageEntity>();
        private Dictionary<string, PHAsset> _assets = new Dictionary<string, PHAsset>();
        private PHFetchResult _fetchResult;

        private ImageCache()
        {
            _taggedImages = _imageRepository.GetAll().ToDictionary(x => x.LocalIdentifier);
            PHFetchResult fetchResult = PHAsset.FetchAssets(PHAssetMediaType.Image, null);
            InitialiseAssets(fetchResult);
            InitialiseImages(_assets);
            _photoLibraryObserver = new PhotoLibraryObserver(this);
            RegisterObservers();
        }

        public event EventHandler<EventArgs> PhotoLibraryChanged = delegate { };

        public static ImageCache Instance
        {
            get { return _instance; }
        }

        public PHCachingImageManager ImageManager
        {
            get { return _imageManager; }
        }

        public PHAsset GetAsset(string localId)
        {
            lock (_locker)
            {
                return _assets[localId];
            }
        }

        public SizeF GetImageSize(string localId)
        {
            return GetSize(localId);
        }

        public List<ImageEntity> GetImages(TagEntity tag)
        {
            if (tag.IsAll)
            {
                return GetImages();
            }
            if (tag.IsUntagged)
            {
                return GetUntagged();
            }
            return _taggedImages.Values.Where(x => x.ContainsTag(tag)).ToList();
        }

        public void Remove(List<ImageEntity> images)
        {
            _imageRepository.Remove(images);
            UpdateTaggedImages(images);
            RemoveCachedImages(images);
        }

        public void Remove(ImageEntity image)
        {
            Remove(new List<ImageEntity> { image });
        }

        public void SaveOrUpdate(List<ImageEntity> images)
        {
            _imageRepository.SaveOrUpdate(images);
            UpdateTaggedImages(images);
        }

        private void CheckRemovedTags(List<TagEntity> tags)
        {
            if (tags.IsNullOrEmpty())
            {
                return;
            }
            var emptyTags = new List<TagEntity>();
            foreach (TagEntity tag in tags)
            {
                if (_taggedImages.Values.Any(x => x.ContainsTag(tag)) == false)
                {
                    emptyTags.Add(tag);
                }
            }
            TagCache.Instance.Remove(emptyTags);
        }

        private ImageEntity CreateImage(PHAsset asset)
        {
            var result = new ImageEntity
            {
                LocalIdentifier = asset.LocalIdentifier,
                CreateTime = asset.CreationDate
            };
            return result;
        }

        private List<ImageEntity> GetImages()
        {
            lock (_locker)
            {
                foreach (ImageEntity taggedImage in _taggedImages.Values)
                {
                    if (!_actualImages.ContainsKey(taggedImage.LocalIdentifier))
                    {
                        continue;
                    }
                    if (taggedImage.Equals(_actualImages[taggedImage.LocalIdentifier]))
                    {
                        continue;
                    }
                    _actualImages[taggedImage.LocalIdentifier] = taggedImage;
                }
                return _actualImages.Values.OrderByDescending(x => x.CreateTime).ToList();
            }
        }

        private SizeF GetSize(string localId)
        {
            PHAsset asset = GetAsset(localId);
            return new SizeF(asset.PixelWidth, asset.PixelHeight);
        }

        private List<ImageEntity> GetUntagged()
        {
            var result = new List<ImageEntity>();
            var comparer = new FuncComparer<ImageEntity>((x, y) => x.Equals(y));
            lock (_locker)
            {
                List<ImageEntity> untaggedImages = _actualImages.Values.Except(_taggedImages.Values.ToList(), comparer).ToList();
                result.AddRange(untaggedImages);
                return result;
            }
        }

        private void InitialiseAssets(PHFetchResult fetchResult)
        {
            _fetchResult = fetchResult;
            _assets = fetchResult.Cast<PHAsset>()
                                 .Where(x => x.PixelWidth > 0 && x.PixelHeight > 0)
                                 .ToDictionary(x => x.LocalIdentifier);
        }

        private void InitialiseImages(Dictionary<string, PHAsset> assets)
        {
            _actualImages = assets.Values
                                  .Select(x => CreateImage(x))
                                  .ToDictionary(x => x.LocalIdentifier);
        }

        private void OnPhotoLibraryDidChange(PHFetchResult fetchResult)
        {
            lock (_locker)
            {
                InitialiseAssets(fetchResult);
                InitialiseImages(_assets);
            }
            this.RaiseEvent(PhotoLibraryChanged, EventArgs.Empty);
        }

        private void RegisterObservers()
        {
            PHPhotoLibrary.SharedPhotoLibrary.RegisterChangeObserver(_photoLibraryObserver);
        }

        private void RemoveCachedImages(List<ImageEntity> images)
        {
            lock (_locker)
            {
                foreach (ImageEntity entity in images)
                {
                    if (_assets.ContainsKey(entity.LocalIdentifier))
                    {
                        _assets.Remove(entity.LocalIdentifier);
                    }
                    if (_actualImages.ContainsKey(entity.LocalIdentifier))
                    {
                        _actualImages.Remove(entity.LocalIdentifier);
                    }
                    if (_taggedImages.ContainsKey(entity.LocalIdentifier))
                    {
                        _taggedImages.Remove(entity.LocalIdentifier);
                    }
                }
            }
        }

        private void UpdateTaggedImages(List<ImageEntity> images)
        {
            foreach (ImageEntity image in images)
            {
                ImageEntity previousImage;
                if (_taggedImages.TryGetValue(image.LocalIdentifier, out previousImage))
                {
                    List<TagEntity> removedTags = previousImage.GetRemovedTags(image);

                    if (image.Tags.IsEmpty())
                    {
                        _taggedImages.Remove(image.LocalIdentifier);
                    }
                    else
                    {
                        _taggedImages[image.LocalIdentifier] = image;
                    }
                    CheckRemovedTags(removedTags);
                }
                else
                {
                    _taggedImages[image.LocalIdentifier] = image;
                }
            }
        }


        private class PhotoLibraryObserver : PHPhotoLibraryChangeObserver
        {
            private readonly ImageCache _imageCache;

            public PhotoLibraryObserver(ImageCache imageCache)
            {
                _imageCache = imageCache;
            }

            public override void PhotoLibraryDidChange(PHChange changeInstance)
            {
                PHFetchResultChangeDetails changes = changeInstance.GetFetchResultChangeDetails(_imageCache._fetchResult);
                if (changes == null)
                {
                    return;
                }
                _imageCache.OnPhotoLibraryDidChange(changes.FetchResultAfterChanges);
            }
        }
    }
}
