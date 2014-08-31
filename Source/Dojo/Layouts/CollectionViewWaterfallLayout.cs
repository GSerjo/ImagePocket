using System;
using MonoTouch.UIKit;
using System.Collections.Generic;

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
	}
}

