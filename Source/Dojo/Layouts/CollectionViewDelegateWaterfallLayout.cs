using System;
using System.Drawing;
using Core;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Dojo
{
    public class CollectionViewDelegateWaterfallLayout : UICollectionViewDelegate
    {
        public virtual Option<float> CollectionView(UICollectionView collectionView, UICollectionViewLayout collectionViewLayout,
            int minimumInteritemSpacingForSectionAtIndex)
        {
            return Option<float>.Empty;
        }

        public virtual Option<SizeF> CollectionView(UICollectionView collectionView, UICollectionViewLayout collectionViewLayout,
            NSIndexPath sizeForItemAtIndexPath)
        {
            return Option<SizeF>.Empty;
        }

        public virtual Option<UIEdgeInsets> CollectionView1(UICollectionView collectionView, UICollectionViewLayout collectionViewLayout,
            int insetForSectionAtIndex)
        {
            return Option<UIEdgeInsets>.Empty;
        }

        public virtual Option<float> CollectionView2(UICollectionView collectionView, UICollectionViewLayout collectionViewLayout,
            int heightForHeaderInSection)
        {
            return Option<float>.Empty;
        }

        public virtual Option<float> ColletionView3(UICollectionView collectionView, UICollectionViewLayout collectionViewLayout,
            int heightForFooterInSection)
        {
            return Option<float>.Empty;
        }
    }
}
