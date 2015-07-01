using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using CoreGraphics;
using Photos;
using UIKit;

namespace Domain
{
    public sealed class ImageCache
    {
        private static readonly ImageCache _instance = new ImageCache();
        private readonly PHCachingImageManager _cachingImageManager = new PHCachingImageManager();
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

        public PHAsset GetAsset(string localId)
        {
            lock (_locker)
            {
                return _assets[localId];
            }
        }

        public void GetCachingImage(PHAsset asset, CGSize size, PHImageRequestOptions options, Action<UIImage> action)
        {
            try
            {
                _cachingImageManager.RequestImageForAsset(asset, size, PHImageContentMode.AspectFit, options,
                    (image, info) =>
                    {
                        try
                        {
                            action(image);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void GetImage(PHAsset asset, CGSize size, PHImageRequestOptions options, Action<UIImage> action)
        {
            try
            {
                PHImageManager.DefaultManager.RequestImageForAsset(asset, size, PHImageContentMode.AspectFit, options,
                    (image, info) =>
                    {
                        try
                        {
                            action(image);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public List<ImageEntity> GetImages(TagEntity tag)
        {
            List<ImageEntity> result;
            if (tag.IsAll)
            {
                result = GetImages();
            }
            else if (tag.IsUntagged)
            {
                result = GetUntagged();
            }
            else
            {
                result = _taggedImages.Values.Where(x => x.ContainsTag(tag)).ToList();
            }
            return result.OrderByDescending(x => x.CreateTime).ToList();
        }

        public void Remove(List<ImageEntity> images)
        {
            if (images.IsNullOrEmpty())
            {
                return;
            }
            _imageRepository.Remove(images);
            RemoveCachedImages(images);
            List<TagEntity> tags = GetDistinctTags(images);
            RemoveEmpyTags(tags);
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

        public void StartCachingImagePreview()
        {
            try
            {
                var options = new PHImageRequestOptions
                {
                    Synchronous = false,
                    NetworkAccessAllowed = true
                };
                var imageSize = new CGSize(16, 160);
                _cachingImageManager.StartCaching(_assets.Values.ToArray(), imageSize, PHImageContentMode.AspectFit, options);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private ImageEntity CreateImage(PHAsset asset)
        {
            try
            {
                var result = new ImageEntity
                {
                    LocalIdentifier = asset.LocalIdentifier,
                    CreateTime = asset.CreationDate.ToDateTime()
                };
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new ImageEntity
                {
                    LocalIdentifier = asset.LocalIdentifier,
                    CreateTime = DateTime.MinValue
                };
            }
        }

        private List<TagEntity> GetDistinctTags(List<ImageEntity> images)
        {
            var comparer = new FuncComparer<TagEntity>((x, y) => x.Equals(y));
            return images.SelectMany(x => x.Tags).Distinct(comparer).ToList();
        }

        private List<ImageEntity> GetImages()
        {
            lock (_locker)
            {
                return _actualImages.Values.ToList();
            }
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

            var remove = new List<ImageEntity>();
            foreach (ImageEntity imageEntity in _taggedImages.Values)
            {
                if (_actualImages.ContainsKey(imageEntity.LocalIdentifier))
                {
                    _actualImages[imageEntity.LocalIdentifier] = imageEntity;
                }
                else
                {
                    remove.Add(imageEntity);
                }
            }
            Remove(remove);
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

        private void RemoveEmpyTags(List<TagEntity> tags)
        {
            if (tags.IsNullOrEmpty())
            {
                return;
            }
            var comparer = new FuncComparer<TagEntity>((x, y) => x.Equals(y));
            tags = tags.Distinct(comparer).ToList();
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

        private void UpdateTaggedImages(List<ImageEntity> images)
        {
            var removedImages = new List<ImageEntity>();
            var removeTags = new List<TagEntity>();
            foreach (ImageEntity image in images)
            {
                ImageEntity previousImage;
                if (_actualImages.TryGetValue(image.LocalIdentifier, out previousImage) == false)
                {
                    removedImages.Add(image);
                    continue;
                }

                removeTags.AddRange(previousImage.GetRemovedTags(image));
                previousImage.Update(image);
                if (image.Tags.IsEmpty())
                {
                    _taggedImages.Remove(image.LocalIdentifier);
                }
                else
                {
                    _taggedImages[image.LocalIdentifier] = image;
                }
            }
            RemoveEmpyTags(removeTags);
            Remove(removedImages);
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
