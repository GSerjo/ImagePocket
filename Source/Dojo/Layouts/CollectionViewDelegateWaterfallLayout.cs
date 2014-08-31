using System;
using MonoTouch.UIKit;

namespace Dojo
{
	public class CollectionViewDelegateWaterfallLayout : UICollectionViewDelegate
	{
		public virtual float ColletionView (UICollectionView collectionView, UICollectionViewLayout collectionViewLayout,
			int minimumInteritemSpacingForSectionAtIndex)
		{
			return 0;
		}

		public virtual UIEdgeInsets ColletionView1(UICollectionView collectionView, UICollectionViewLayout collectionViewLayout,
			int insetForSectionAtIndex)
		{
			return UIEdgeInsets.Zero;
		}

	}
}

