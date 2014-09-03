using System;
using MonoTouch.UIKit;
using Core;
using MonoTouch.Foundation;
using System.Drawing;

namespace Dojo
{
	public class CollectionViewDelegateWaterfallLayout : UICollectionViewDelegate
	{
		public virtual Bag<float> CollectionView (UICollectionView collectionView, UICollectionViewLayout collectionViewLayout,
			int minimumInteritemSpacingForSectionAtIndex)
		{
			return Bag<float>.Empty;
		}

		public virtual Bag<UIEdgeInsets> CollectionView1(UICollectionView collectionView, UICollectionViewLayout collectionViewLayout,
			int insetForSectionAtIndex)
		{
			return Bag<UIEdgeInsets>.Empty;
		}

		public virtual Bag<float> CollectionView2 (UICollectionView collectionView, UICollectionViewLayout collectionViewLayout,
			int heightForHeaderInSection)
		{
			return Bag<float>.Empty;
		}

		public virtual Bag<SizeF> CollectionView (UICollectionView collectionView, UICollectionViewLayout collectionViewLayout,
			NSIndexPath sizeForItemAtIndexPath)
		{
			return Bag<SizeF>.Empty; 
		}

		public virtual Bag<float> ColletionView3 (UICollectionView collectionView, UICollectionViewLayout collectionViewLayout,
			int heightForFooterInSection)
		{
			return Bag<float>.Empty;
		}
	}
}