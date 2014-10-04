using System;
using Domain;
using Core;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.Photos;

namespace Domain
{
	public sealed class ImageCache
	{
		private ImageRepository _imageRepository = ImageRepository.Instance;
		private Dictionary<string, ImageEntity> _taggedImages = new Dictionary<string, ImageEntity> ();
		private Dictionary<string, ImageEntity> _actualImages = new Dictionary<string, ImageEntity> ();

		private Dictionary<string, PHAsset> _assets = new Dictionary<string, PHAsset>();
		private readonly PHCachingImageManager _imageManager = new PHCachingImageManager ();

		private static ImageCache _instance = new ImageCache();

		private ImageCache()
		{
			InitialiseAssets ();
			InitialiseImages ();
		}

		public static ImageCache Instance
		{
			get { return _instance; }
		}

		public PHCachingImageManager ImageManager
		{
			get { return _imageManager; }
		}

		public List<ImageEntity> GetImages(TagEntity tag)
		{
			if (tag.IsAll)
			{
				return GetImages ();
			} 
			else if (tag.IsUntagged)
			{
				return GetUntagged ();
			}
			return _taggedImages.Values.Where (x => x.ContainsTag (tag)).ToList ();
		}

		public void SaveOrUpdate(List<ImageEntity> images)
		{
			_imageRepository.SaveOrUpdate (images);
			UpdateTaggedImages (images);
		}

		public SizeF GetImageSize(string localId)
		{
			return GetSize (localId);
		}

		public PHAsset GetAsset(string localId)
		{
			return _assets [localId];
		}

		private void InitialiseAssets()
		{
			PHFetchResult fetchResult = PHAsset.FetchAssets (PHAssetMediaType.Image, null);
			_assets = fetchResult.Cast<PHAsset>()
				.Where(x => x.PixelWidth > 0 && x.PixelHeight > 0)
				.ToDictionary (x => x.LocalIdentifier);
		}

		private void InitialiseImages()
		{
			_taggedImages = _imageRepository.GetAll ().ToDictionary (x => x.LocalIdentifier);
			_actualImages =  _assets.Values
				.Select (x => new ImageEntity { LocalIdentifier = x.LocalIdentifier })
				.ToDictionary (x => x.LocalIdentifier);
		}

		private void UpdateTaggedImages(List<ImageEntity> images)
		{
			foreach (ImageEntity image in images)
			{
				_taggedImages [image.LocalIdentifier] = image;
			}
		}

		private List<ImageEntity> GetImages()
		{
			foreach (ImageEntity image in _taggedImages.Values)
			{
				if (!_actualImages.ContainsKey (image.LocalIdentifier))
				{
					continue;
				}
				_actualImages [image.LocalIdentifier] = image;
			}
			return _actualImages.Values.ToList();
		}

		private List<ImageEntity> GetUntagged()
		{
			var result = new List<ImageEntity> ();
			var comparer = new FuncComparer<ImageEntity> ((x, y) => string.Equals (x.LocalIdentifier, y.LocalIdentifier, StringComparison.OrdinalIgnoreCase));
			var untaggedImages = _actualImages.Values.Except (_taggedImages.Values.ToList(), comparer).ToList();
			result.AddRange (untaggedImages);
			return result;
		}

		private SizeF GetSize(string localId)
		{
			PHAsset asset = GetAsset (localId);
			return new SizeF (asset.PixelWidth, asset.PixelHeight);
		}
	}
}