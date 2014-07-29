﻿using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.Foundation;

namespace Dojo
{
	public class ImpagePreviewCell : UICollectionViewCell
	{
		private UIImageView _imageView;

		[Export("initWithFrame:")]
		public ImpagePreviewCell(RectangleF frame) : base(frame)
		{
			_imageView = new UIImageView(frame);
//			_imageView = new UIImageView (Bounds);
			_imageView.Center = ContentView.Center;
			_imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
//			_imageView.ContentMode = UIViewContentMode.ScaleAspectFill;
//			_imageView.ClipsToBounds = true;
			ContentView.AddSubview(_imageView);
		}

		public UIImage Image
		{
			set { _imageView.Image = value; }
		}

		public bool Selected { get; set; }
	}
}

