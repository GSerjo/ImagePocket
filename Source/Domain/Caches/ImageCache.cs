﻿using System;
using System.Collections.Generic;
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

        private ImageEntity CreateImage(PHAsset asset)
        {
            var result = new ImageEntity
            {
                LocalIdentifier = asset.LocalIdentifier,
                CreateTime = asset.CreationDate
            };
            return result;
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

            foreach (ImageEntity taggedImage in _taggedImages.Values)
            {
                if (_actualImages.ContainsKey(taggedImage.LocalIdentifier))
                {
                    _actualImages[taggedImage.LocalIdentifier] = taggedImage;
                }
            }
            //TODO Remove absent images from TaggedStore
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
