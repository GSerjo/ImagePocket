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
		private readonly SizeF _smallImage = new SizeF(240, 240);
		private Dictionary<string, PHAsset> _assets = new Dictionary<string, PHAsset>();
		private static AssetRepository _instance = new AssetRepository();

		private AssetRepository ()
		{
			PHFetchResult fetchResult = PHAsset.FetchAssets (PHAssetMediaType.Image, null);
			_assets = fetchResult.Cast<PHAsset>()
								.Where(x => x.PixelWidth > 0 && x.PixelHeight > 0)
								.ToDictionary (x => x.LocalIdentifier);
		}

		public static AssetRepository Instance
		{
			get { return _instance; }
		}

		public PHAsset GetAsset(string localId)
		{
			return _assets [localId];
		}

		public UIImage GetOrigianlImage(string localId)
		{
			PHAsset asset = GetAsset (localId);
			return CreateImage (asset, new SizeF (asset.PixelWidth, asset.PixelHeight));
		}

		public List<PHAsset> GetAll()
		{
			return _assets.Values.ToList();
		}

		public UIImage GetSmallImage(string localId)
		{
			PHAsset asset = GetAsset (localId);
			return CreateImage (asset, _smallImage);
		}

		private UIImage CreateImage(PHAsset asset, SizeF imageSize)
		{
			UIImage result = null;
			var options = new PHImageRequestOptions ();
			_imageManager.RequestImageForAsset (asset, imageSize, PHImageContentMode.Default,
				options, (image, info) =>
				{
					result = image;
				});
			return result;
		}
	}
}