using System;
using Domain;
using Core;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.UIKit;

namespace Dojo
{
	public sealed class ImageCache
	{
		private AssetRepository _assetRepository = AssetRepository.Instance;
		private ImageRepository _imageRepository = ImageRepository.Instance;
		private Dictionary<string, ImageEntity> _taggedImages = new Dictionary<string, ImageEntity> ();
		private readonly Dictionary<string, ImageEntity> _actualImages = new Dictionary<string, ImageEntity> ();
		private static ImageCache _instance = new ImageCache();

		private ImageCache()
		{
			_taggedImages = _imageRepository.GetAll ().ToDictionary (x => x.LocalIdentifier);
			_actualImages =  _assetRepository.GetAll ()
				.Select (x => new ImageEntity { LocalIdentifier = x.LocalIdentifier })
				.ToDictionary (x => x.LocalIdentifier);
		}

		public static ImageCache Instance
		{
			get { return _instance; }
		}

		public List<ImageEntity> GetImages(TagEntity tag)
		{
			if (tag.IsAll)
			{
				return GetAll ();
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

		public UIImage GetSmallImage(string localId)
		{
			return _assetRepository.GetSmallImage (localId);
		}

		private void UpdateTaggedImages(List<ImageEntity> images)
		{
			foreach (ImageEntity image in images)
			{
				_taggedImages [image.LocalIdentifier] = image;
			}
		}

		private List<ImageEntity> GetAll()
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
	}
}