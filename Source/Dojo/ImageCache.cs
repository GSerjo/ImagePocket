using System;
using Domain;
using System.Collections.Generic;
using System.Linq;

namespace Dojo
{
	public sealed class ImageCache
	{
		private AssetRepository _assetRepository = new AssetRepository();
		private ImageRepository _imageRepository = new ImageRepository();
		private Dictionary<string, ImageEntity> _taggedImages = new Dictionary<string, ImageEntity> ();

		public ImageCache()
		{
			_taggedImages = _imageRepository.GetAll ().ToDictionary (x => x.LocalIdentifier);
		}

		public List<ImageEntity> GetAll()
		{
			var result = new List<ImageEntity> ();
			result.AddRange (_taggedImages.Values.ToList ());
			var actualImages =  _assetRepository.GetAll ()
				.Select (x => new ImageEntity { LocalIdentifier = x.LocalIdentifier });
			//actualImages.Except(result, 
			return result;
		}

	}
}

