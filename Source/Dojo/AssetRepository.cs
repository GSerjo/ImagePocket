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
		private readonly PHFetchResult _fetchResult;
		private readonly SizeF _smallImage = new SizeF(200, 200);
		private Dictionary<string, PHAsset> _assets = new Dictionary<string, PHAsset>();

		public AssetRepository ()
		{
			_fetchResult = PHAsset.FetchAssets (PHAssetMediaType.Image, null);
			_assets = _fetchResult.ToDictionary (x => ((PHAsset)x).LocalIdentifier, y => ((PHAsset)y));
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

		public Bag<UIImage> GetSmallImage(string localId)
		{
			var asset = GetAsset (localId);
			if (asset.HasNoValue)
			{
				return Bag<UIImage>.Empty;
			}
			return CreateSmallImage (asset.Value).ToBag();
		}

		public UIImage GetSmallImage(int index)
		{
			UIImage result = null;
			var asset = (PHAsset)_fetchResult [(uint)index];
			var options = new PHImageRequestOptions ();
			_imageManager.RequestImageForAsset (asset, _smallImage, PHImageContentMode.Default,
				options, (image, info) =>
				{
					result = image;
				});
			return result;
		}

		public PHAsset GetAsset(int index)
		{
			return (PHAsset)_fetchResult [(uint)index];
		}

		public int ImageCount
		{
			get { return (int)_fetchResult.Count; }
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