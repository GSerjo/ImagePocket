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
		private readonly List<ImageEntity> _actualImages;

		public ImageCache()
		{
			_taggedImages = _imageRepository.GetAll ().ToDictionary (x => x.LocalIdentifier);
			_actualImages =  _assetRepository.GetAll ()
				.Select (x => new ImageEntity { LocalIdentifier = x.LocalIdentifier })
				.ToList();
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
			return _taggedImages.Values.Where (x => x.Tags.Contains (tag.EntityId)).ToList ();
		}

		public void SaveOrUpdate(List<ImageEntity> images)
		{
			_imageRepository.SaveOrUpdate (images)
				.ContinueWith(x => UpdateTaggedImages (images))
				.Wait();
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
			var result = new List<ImageEntity> ();
			result.AddRange (_taggedImages.Values.ToList ());
			result.AddRange (GetUntagged ());
			return result;
		}

		private List<ImageEntity> GetUntagged()
		{
			var result = new List<ImageEntity> ();
			var comparer = new FuncComparer<ImageEntity> ((x, y) => string.Equals (x.LocalIdentifier, y.LocalIdentifier, StringComparison.OrdinalIgnoreCase));
			var untaggedImages = _actualImages.Except (_taggedImages.Values.ToList(), comparer).ToList();
			result.AddRange (untaggedImages);
			return result;
		}
	}
}