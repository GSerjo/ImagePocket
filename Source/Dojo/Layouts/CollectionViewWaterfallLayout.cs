using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Core;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Dojo
{
    public sealed class CollectionViewWaterfallLayout : UICollectionViewLayout
    {
        private const string CHTCollectionElementKindSectionFooter = "CHTCollectionElementKindSectionFooter";
        private const string CHTCollectionElementKindSectionHeader = "CHTCollectionElementKindSectionHeader";
        private readonly List<UICollectionViewLayoutAttributes> _allItemAttributes = new List<UICollectionViewLayoutAttributes>();
        private readonly List<float> _columnHeights = new List<float>();
        private readonly Dictionary<int, UICollectionViewLayoutAttributes> _footersAtributes = new Dictionary<int, UICollectionViewLayoutAttributes>();
        private readonly Dictionary<int, UICollectionViewLayoutAttributes> _headerAtributes = new Dictionary<int, UICollectionViewLayoutAttributes>();
        private readonly List<List<UICollectionViewLayoutAttributes>> _sectionItemAttributes = new List<List<UICollectionViewLayoutAttributes>>();
        private readonly List<RectangleF> _unionRects = new List<RectangleF>();
        private readonly int _unionSize = 20;
        private int _columnCount;
        private float _footerHeight;
        private float _headerHeight;
        private CollectionViewWaterfallLayoutItemRenderDirection _itemRenderDirection;
        private float _minimumColumnSpacing;
        private float _minimumInteritemSpacing;
        private UIEdgeInsets _sectionInset;

        public CollectionViewWaterfallLayout()
        {
            HeaderHeight = 0;
            FooterHeight = 0;
            ColumnCount = 2;
            MinimumInteritemSpacing = 10;
            MinimumColumnSpacing = 10;
            SectionInset = UIEdgeInsets.Zero;
        }

        public override SizeF CollectionViewContentSize
        {
            get
            {
                int numberOfSections = CollectionView.NumberOfSections();
                if (numberOfSections == 0)
                {
                    return SizeF.Empty;
                }
                SizeF contentSize = CollectionView.Bounds.Size;
                float height = _columnHeights.First();
                contentSize.Height = height;
                return contentSize;
            }
        }

        public int ColumnCount
        {
            get { return _columnCount; }
            set
            {
                _columnCount = value;
                InvalidateLayout();
            }
        }

        public float FooterHeight
        {
            get { return _footerHeight; }
            set
            {
                _footerHeight = value;
                InvalidateLayout();
            }
        }

        public float HeaderHeight
        {
            get { return _headerHeight; }
            set
            {
                _headerHeight = value;
                InvalidateLayout();
            }
        }

        public CollectionViewWaterfallLayoutItemRenderDirection ItemRenderDirection
        {
            get { return _itemRenderDirection; }
            set
            {
                _itemRenderDirection = value;
                InvalidateLayout();
            }
        }

        public float MinimumColumnSpacing
        {
            get { return _minimumColumnSpacing; }
            set
            {
                _minimumColumnSpacing = value;
                InvalidateLayout();
            }
        }

        public float MinimumInteritemSpacing
        {
            get { return _minimumInteritemSpacing; }
            set
            {
                _minimumInteritemSpacing = value;
                InvalidateLayout();
            }
        }

        public UIEdgeInsets SectionInset
        {
            get { return _sectionInset; }
            set
            {
                _sectionInset = value;
                InvalidateLayout();
            }
        }

        private CollectionViewDelegateWaterfallLayout Delegate
        {
            get { return CollectionView.Delegate as CollectionViewDelegateWaterfallLayout; }
        }

        public override UICollectionViewLayoutAttributes[] LayoutAttributesForElementsInRect(RectangleF rect)
        {
            int begin = 0, end = _unionRects.Count;
            var attrs = new List<UICollectionViewLayoutAttributes>();

            for (int i = 0; i < end; i++)
            {
                if (rect.IntersectsWith(_unionRects[i]))
                {
                    begin = i * _unionSize;
                    break;
                }
            }
            for (int i = _unionRects.Count - 1; i >= 0; i--)
            {
                if (rect.IntersectsWith(_unionRects[i]))
                {
                    end = Math.Min((i + 1) * _unionSize, _allItemAttributes.Count);
                }
            }
            for (int i = begin; i < end; i++)
            {
                UICollectionViewLayoutAttributes attr = _allItemAttributes[i];
                if (rect.IntersectsWith(attr.Frame))
                {
                    attrs.Add(attr);
                }
            }
            return attrs.ToArray();
        }

        public override UICollectionViewLayoutAttributes LayoutAttributesForItem(NSIndexPath indexPath)
        {
            if (indexPath.Section >= _sectionItemAttributes.Count)
            {
                return null;
            }
            if (indexPath.Item >= _sectionItemAttributes[indexPath.Section].Count)
            {
                return null;
            }
            List<UICollectionViewLayoutAttributes> list = _sectionItemAttributes[indexPath.Section];
            return list[indexPath.Item];
        }

        public override UICollectionViewLayoutAttributes LayoutAttributesForSupplementaryView(NSString kind, NSIndexPath indexPath)
        {
            var attribute = new UICollectionViewLayoutAttributes();
            if (kind == CHTCollectionElementKindSectionHeader)
            {
                attribute = _headerAtributes[indexPath.Section];
            }
            else if (kind == CHTCollectionElementKindSectionFooter)
            {
                attribute = _footersAtributes[indexPath.Section];
            }
            return base.LayoutAttributesForSupplementaryView(kind, indexPath);
        }

        public override void PrepareLayout()
        {
            base.PrepareLayout();

            int numberOfSections = CollectionView.NumberOfSections();
            if (numberOfSections == 0)
            {
                return;
            }

            _headerAtributes.Clear();
            _footersAtributes.Clear();
            _unionRects.Clear();
            _columnHeights.Clear();
            _allItemAttributes.Clear();
            _sectionItemAttributes.Clear();

            int idx = 0;

            while (idx < ColumnCount)
            {
                _columnHeights.Add(0);
                idx++;
            }

            float top = 0;

            var attributes = new UICollectionViewLayoutAttributes();
            for (int section = 0; section < numberOfSections; ++section)
            {
                // 1. Get section - specific metrics (minimumInteritemSpacing, sectionInset)
                float minimumInteritemSpacing;
                Option<float> minimumSpacing = Delegate.CollectionView(CollectionView, this, section);
                if (minimumSpacing.HasValue)
                {
                    minimumInteritemSpacing = minimumSpacing.Value;
                }
                else
                {
                    minimumInteritemSpacing = MinimumColumnSpacing;
                }
                UIEdgeInsets sectionInsets;
                Option<UIEdgeInsets> insets = Delegate.CollectionView1(CollectionView, this, section);
                if (insets.HasValue)
                {
                    sectionInsets = insets.Value;
                }
                else
                {
                    sectionInsets = SectionInset;
                }
                float width = CollectionView.Frame.Size.Width - _sectionInset.Left - _sectionInset.Right;
                float spaceColumnCount = ((float)ColumnCount - 1);
                var itemWidth = ((float)Math.Floor((width - (spaceColumnCount * MinimumColumnSpacing)) / ColumnCount));

                //2. Section header
                top = CalculateHeaderHeight(section);

                //3. Section items
                int itemCount = CollectionView.NumberOfItemsInSection(section);
                var itemAttributes = new List<UICollectionViewLayoutAttributes>(itemCount);

                for (int i = 0; i < itemCount; i++)
                {
                    NSIndexPath indexPath = NSIndexPath.FromItemSection(i, section);
                    int columnIndex = NextColumnIndexForItem(i);
                    float xOffset = SectionInset.Left + (itemWidth + MinimumColumnSpacing) * columnIndex;
                    float yOffset = _columnHeights[columnIndex];
                    SizeF itemSize = Delegate.CollectionView(CollectionView, this, indexPath).Value;
                    float itemHeight = 10;
                    if (itemSize.Height > 0 && itemSize.Width > 0)
                    {
                        itemHeight = (float)Math.Floor(itemSize.Height * itemWidth / itemSize.Width);
                    }
                    attributes = UICollectionViewLayoutAttributes.CreateForCell(indexPath);
                    attributes.Frame = new RectangleF(xOffset, yOffset, itemWidth, itemHeight);
                    itemAttributes.Add(attributes);
                    _allItemAttributes.Add(attributes);
                    _columnHeights[columnIndex] = attributes.Frame.Bottom + MinimumInteritemSpacing;
                }
                _sectionItemAttributes.Add(itemAttributes);

                //4. Section footer
                float footerHeight = 0;
                int longestColumnIndex = LongestColumnIndex();
                top = _columnHeights[longestColumnIndex] - minimumInteritemSpacing + SectionInset.Bottom;
                Option<float> heightFooterCustom = Delegate.ColletionView3(CollectionView, this, section);
                if (heightFooterCustom.HasValue)
                {
                    footerHeight = heightFooterCustom.Value;
                }
                else
                {
                    footerHeight = FooterHeight;
                }
                if (footerHeight > 0)
                {
                    NSIndexPath indexPath = NSIndexPath.FromItemSection(0, section);
                    attributes = UICollectionViewLayoutAttributes.CreateForCell(indexPath);
                    attributes.Frame = new RectangleF(0, top, CollectionView.Frame.Size.Width, footerHeight);
                    _footersAtributes[section] = attributes;
                    _allItemAttributes.Add(attributes);
                    top = attributes.Frame.Y;
                }

                for (int i = 0; i < ColumnCount; i++)
                {
                    _columnHeights[i] = top;
                }

                idx = 0;
                int itemCounts = _allItemAttributes.Count;
                while (idx < itemCounts)
                {
                    RectangleF rect1 = _allItemAttributes[idx].Frame;
                    idx = Math.Min(idx + _unionSize, itemCounts) - 1;
                    RectangleF rect2 = _allItemAttributes[idx].Frame;
                    _unionRects.Add(RectangleF.Union(rect1, rect2));
                    idx++;
                }
            }
        }

        public override bool ShouldInvalidateLayoutForBoundsChange(RectangleF newBounds)
        {
            RectangleF oldBounds = CollectionView.Bounds;
            if (newBounds.Width != oldBounds.Width)
            {
                return false;
            }
            return true;
        }

        private float CalculateHeaderHeight(int section)
        {
            float top = 0;
            float heightHeader;
            Option<float> heightHeaderCustom = Delegate.CollectionView2(CollectionView, this, section);
            if (heightHeaderCustom.HasValue)
            {
                heightHeader = heightHeaderCustom.Value;
            }
            else
            {
                heightHeader = HeaderHeight;
            }
            if (heightHeader > 0)
            {
                UICollectionViewLayoutAttributes attributes = UICollectionViewLayoutAttributes.CreateForSupplementaryView(UICollectionElementKindSection.Header, NSIndexPath.FromRowSection(0, section));
                attributes.Frame = new RectangleF(0, top, CollectionView.Frame.Size.Width, heightHeader);
                _headerAtributes[section] = attributes;
                _allItemAttributes.Add(attributes);
                top = attributes.Frame.X;
            }
            top += _sectionInset.Top;
            for (int i = 0; i < ColumnCount; i++)
            {
                _columnHeights[i] = top;
            }
            return top;
        }

        private float ItemwidthInSectionAtIndex(int section)
        {
            UIEdgeInsets insets;
            Option<UIEdgeInsets> sectionInsets = Delegate.CollectionView1(CollectionView, this, section);
            if (sectionInsets.HasValue)
            {
                insets = sectionInsets.Value;
            }
            else
            {
                insets = SectionInset;
            }
            float width = CollectionView.Frame.Size.Width - _sectionInset.Right;
            var spaceColumnCount = (double)(ColumnCount - 1);
            return (float)(Math.Floor(width - (spaceColumnCount * MinimumColumnSpacing) / ColumnCount));
        }

        private int LongestColumnIndex()
        {
            int index = 0;
            float longestHeight = 0;
            float height = 0;
            for (int i = 0; i < _columnHeights.Count; i++)
            {
                height = _columnHeights[i];
                if (height > longestHeight)
                {
                    longestHeight = height;
                    index = i;
                }
            }
            return index;
        }

        private int NextColumnIndexForItem(int item)
        {
            int index = 0;
            switch (ItemRenderDirection)
            {
                case CollectionViewWaterfallLayoutItemRenderDirection.ShortestFirst:
                    index = ShortestColumnIndex();
                    break;
                case CollectionViewWaterfallLayoutItemRenderDirection.LeftToRight:
                    index = item % ColumnCount;
                    break;
                case CollectionViewWaterfallLayoutItemRenderDirection.RightToLeft:
                    index = (ColumnCount - 1) - (item % ColumnCount);
                    break;
                default:
                    index = ShortestColumnIndex();
                    break;
            }
            return index;
        }

        private int ShortestColumnIndex()
        {
            int index = 0;
            float shortestHeight = float.MaxValue;
            float height = 0;
            for (int i = 0; i < _columnHeights.Count; i++)
            {
                height = _columnHeights[i];
                if (height < shortestHeight)
                {
                    shortestHeight = height;
                    index = i;
                }
            }
            return index;
        }
    }
}
