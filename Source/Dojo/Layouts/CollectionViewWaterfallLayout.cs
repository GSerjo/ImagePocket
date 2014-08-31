using System;
using MonoTouch.UIKit;
using System.Collections.Generic;
using System.Drawing;

namespace Dojo
{
	public sealed class CollectionViewWaterfallLayout : UICollectionViewLayout
	{
		private const string CHTCollectionElementKindSectionHeader = "CHTCollectionElementKindSectionHeader";
		private const string CHTCollectionElementKindSectionFooter = "CHTCollectionElementKindSectionFooter";
		private int _columnCount;
		private float _minimumColumnSpacing;
		private float _minimumInteritemSpacing;
		private float _headerHeight;
		private float _footerHeight;
		private UIEdgeInsets _sectionInset;
		private List<float> _columnHeights = new List<float>();
		private Dictionary<int, UICollectionViewLayoutAttributes> _headerAtributes = new Dictionary<int, UICollectionViewLayoutAttributes> ();
		private Dictionary<int, UICollectionViewLayoutAttributes> _footersAtributes = new Dictionary<int, UICollectionViewLayoutAttributes> ();
		private List<RectangleF> _unionRects = new List<RectangleF>();
		private List<RectangleF> _allItemAttributes = new List<RectangleF>();
		private List<int> _sectionItemAttributes = new List<int>();

		public CollectionViewWaterfallLayout ()
		{
			HeaderHeight = 0;
			FooterHeight = 0;
			ColumnCount = 2;
			MinimumInteritemSpacing = 10;
			MinimumColumnSpacing = 10;
			SectionInset = UIEdgeInsets.Zero;
		}

		public int ColumnCount
		{
			get { return _columnCount; }
			set
			{
				_columnCount = value;
				InvalidateLayout ();
			}
		}

		public float MinimumColumnSpacing
		{
			get { return _minimumColumnSpacing; }
			set
			{
				_minimumColumnSpacing = value;
				InvalidateLayout ();
			}
		}

		public float MinimumInteritemSpacing
		{
			get { return _minimumInteritemSpacing; }
			set
			{
				_minimumInteritemSpacing = value;
				InvalidateLayout ();
			}
		}

		public float HeaderHeight
		{
			get { return _headerHeight; }
			set
			{
				_headerHeight = value;
				InvalidateLayout ();
			}
		}

		private CollectionViewDelegateWaterfallLayout Delegate
		{
			get { return CollectionView.Delegate as CollectionViewDelegateWaterfallLayout; }
		}

		public float FooterHeight
		{
			get { return _footerHeight; }
			set
			{
				_footerHeight = value;
				InvalidateLayout ();
			}
		}

		public UIEdgeInsets SectionInset
		{
			get { return _sectionInset; }
			set
			{
				_sectionInset = value;
				InvalidateLayout ();
			}
		}

		private float ItemwidthInSectionAtIndex(int section)
		{
			UIEdgeInsets insets;
			UIEdgeInsets sectionInset = Delegate.ColletionView1 (CollectionView,this, section);
			if (!sectionInset.Equals(UIEdgeInsets.Zero))
			{
				insets = sectionInset;
			}
			else
			{
				insets = SectionInset;
			}
			var width = CollectionView.Frame.Size.Width - sectionInset.Right;
			var spaceColumnCount = (double)(ColumnCount - 1);
			return (float)(Math.Floor (width - (spaceColumnCount * MinimumColumnSpacing) / ((double)ColumnCount)));
		}

		public override void PrepareLayout ()
		{
			base.PrepareLayout ();

			var numberOfSections = CollectionView.NumberOfSections();
			if (numberOfSections == 0)
			{
				return;
			}

			_headerAtributes.Clear ();
			_footersAtributes.Clear ();
			_unionRects.Clear ();
			_columnHeights.Clear ();
			_allItemAttributes.Clear ();
			_sectionItemAttributes.Clear ();

			var idx = 0;

			while (idx < ColumnCount)
			{
				_columnHeights.Add (0);
				idx++;
			}

			float top = 0;

			var attributes = new UICollectionViewLayoutAttributes ();
			for (int section = 0; section < numberOfSections; ++section)
			{
				// 1. Get section - specific metrics (minimumInteritemSpacing, sectionInset)
				float minimumInteritemSpacing;

			}

		}


	}
}

