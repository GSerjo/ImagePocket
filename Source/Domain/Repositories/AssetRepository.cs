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
		private readonly PHCachingImageManager _cachingImageManager = new PHCachingImageManager ();
		private readonly SizeF _smallImage = new SizeF(320, 320);
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

		public SizeF GetSize(string localId)
		{
			PHAsset asset = GetAsset (localId);
			return new SizeF (asset.PixelWidth, asset.PixelHeight);
		}

		public UIImage GetCachedImage(string localId, SizeF targetSize)
		{
			PHAsset asset = GetAsset (localId);
			UIImage result = null;
			_cachingImageManager.RequestImageForAsset (asset, targetSize, PHImageContentMode.AspectFit,
				null, (image, info) =>
			{
				result = image;
			});
			Console.WriteLine ("Asset1: W {0}, H: {1}", asset.PixelWidth, asset.PixelHeight);
			Console.WriteLine ("Image1: W {0}, H: {1}, S: {2}", result.Size.Width, result.Size.Height, result.CurrentScale);
			return result;
		}

		public UIImage GetSmallImage(string localId)
		{
//			var t = UIImage.FromFile ("Image.jpg");
			PHAsset asset = GetAsset (localId);
			var result = CreateImage (asset, _smallImage);
//			var t = CreateImage (asset, new SizeF (asset.PixelWidth, asset.PixelHeight));
			return result;
		}

		private UIImage CreateImage(PHAsset asset, SizeF imageSize)
		{
			UIImage result = null;
			var options = new PHImageRequestOptions
			{
			};
//			_imageManager.RequestImageForAsset (asset, imageSize, PHImageContentMode.AspectFill,
//				null, (image, info) =>
//				{
//					result = image;
//				});
			PHImageManager.DefaultManager.RequestImageForAsset (asset, imageSize, PHImageContentMode.AspectFill,
				null, (image, info) =>
			{
				result = image;
			});
			Console.WriteLine ("Image: W {0}, H: {1}, S: {2}", result.Size.Width, result.Size.Height, result.CurrentScale);
			return result;
		}
	}
}