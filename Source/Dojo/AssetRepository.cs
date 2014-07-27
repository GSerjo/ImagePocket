using System;
using MonoTouch.Photos;
using System.Drawing;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace Dojo
{
	public sealed class AssetRepository
	{
		private readonly PHImageManager _imageManager = new PHImageManager();
		private readonly PHFetchResult _fetchResult;
		private readonly SizeF _smallImage = new SizeF(200, 200);

		public AssetRepository ()
		{
			_fetchResult = PHAsset.FetchAssets (PHAssetMediaType.Image, null);
		}

		public UIImage GetImage(int index)
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
	}
}