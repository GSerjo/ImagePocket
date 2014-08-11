﻿using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Drawing;

namespace Dojo
{
	[Register("VENToken")]
	public partial class VENToken : UIView
	{
		public UIColor ColorScheme { get; set; }
		public bool Highlighted { get ; set; }
		public Action<VENToken> OnDidTapToken { get; set; }
		public Guid Id { get; private set; }

		public VENToken (IntPtr handle) : base(handle)
		{
			Id = Guid.NewGuid ();
			//SetupInit ();
		}

		public void SetupInit()
		{
			Layer.CornerRadius = 5;
			ColorScheme = UIColor.Blue;
			titleLabel.TextColor = ColorScheme;
			UITapGestureRecognizer gesture = new UITapGestureRecognizer (DidTapToken);
			this.AddGestureRecognizer (gesture);
		}

		public void SetTitleText(string title)
		{
			titleLabel.Text = title;
			titleLabel.TextColor = ColorScheme;
			Frame = new RectangleF (Frame.X, Frame.Y, titleLabel.Frame.Right + 3, Frame.Height);
			titleLabel.SizeToFit ();
		}

		private void SetHighlighted(bool highlighted)
		{
			Highlighted = highlighted;
			var textColor = Highlighted ? UIColor.White : ColorScheme;
			var backgroundColor = Highlighted ? ColorScheme : UIColor.Clear;
			titleLabel.TextColor = textColor;
			backgroundView.BackgroundColor = backgroundColor;
		}

		public void SetColorSheme(UIColor colorSheme)
		{
			ColorScheme = colorSheme;
			titleLabel.TextColor = ColorScheme;
			SetHighlighted (Highlighted);
		}

		private void DidTapToken(UITapGestureRecognizer gesture)
		{
			if (OnDidTapToken != null)
			{
				OnDidTapToken (this);
			}
			Console.WriteLine ("DidTapToken");
		}

		public override int GetHashCode ()
		{
			return  (int)((Frame.X + Frame.Y + Frame.Width + Frame.Height)) ^ 397;
		}

	}
}