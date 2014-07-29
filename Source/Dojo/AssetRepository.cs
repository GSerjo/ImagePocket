using System;
using MonoTouch.Photos;
using System.Drawing;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Collections.Generic;
using System.Linq;
using Core;

namespace Dojo
{
	public sealed class AssetRepository
	{
		private readonly PHImageManager _imageManager = new PHImageManager();
		private readonly SizeF _smallImage = new SizeF(150, 150);
		private Dictionary<string, PHAsset> _assets = new Dictionary<string, PHAsset>();

		public AssetRepository ()
		{
			var fetchResult = PHAsset.FetchAssets (PHAssetMediaType.Image, null);
			_assets = fetchResult.ToDictionary (x => ((PHAsset)x).LocalIdentifier, y => ((PHAsset)y));
		}

		public Bag<PHAsset> GetAsset(string localId)
		{
			if (!_assets.ContainsKey (localId))
			{
				return Bag<PHAsset>.Empty;
			}
			return _assets [localId].ToBag();
		}

		public List<PHAsset> GetAll()
		{
			return _assets.Values.ToList();
		}

		public UIImage GetSmallImage(string localId)
		{
			var asset = GetAsset (localId);
			return CreateSmallImage (asset.Value);
		}

		private UIImage CreateSmallImage(PHAsset asset)
		{
			UIImage result = null;
			var options = new PHImageRequestOptions ();
			_imageManager.RequestImageForAsset (asset, _smallImage, PHImageContentMode.Default,
				options, (image, info) =>
				{
					result = image;
				});
			return result;
		}
	}
}