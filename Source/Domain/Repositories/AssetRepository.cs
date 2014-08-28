using System;
using MonoTouch.Photos;
using System.Drawing;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Collections.Generic;
using System.Linq;
using Core;

namespace Domain
{
	public sealed class AssetRepository
	{
		private readonly PHImageManager _imageManager = new PHImageManager();
		private readonly SizeF _smallImage = new SizeF(150, 150);
		private Dictionary<string, PHAsset> _assets = new Dictionary<string, PHAsset>();
		private static AssetRepository _instance = new AssetRepository();

		private AssetRepository ()
		{
			var fetchResult = PHAsset.FetchAssets (PHAssetMediaType.Image, null);
			_assets = fetchResult.ToDictionary (x => ((PHAsset)x).LocalIdentifier, y => ((PHAsset)y));
		}

		public static AssetRepository Instance
		{
			get { return _instance; }
		}

		public PHAsset GetAsset(string localId)
		{
			return _assets [localId];
		}

		public List<PHAsset> GetAll()
		{
			return _assets.Values.ToList();
		}

		public UIImage GetSmallImage(string localId)
		{
			PHAsset asset = GetAsset (localId);
			return CreateSmallImage (asset);
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