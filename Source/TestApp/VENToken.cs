using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace TestApp
{
	public partial class VENToken : UIView
	{

		private UIColor _colorScheme = UIColor.Blue;
		public bool Highlighted { get ; set; }


		[Export("initWithFrame:")]
		public VENToken(RectangleF frame) : base(frame)
		{
		}

		public override bool BecomeFirstResponder ()
		{
		}

		private void SetupInit()
		{
			Layer.CornerRadius = 5;
			UITapGestureRecognizer gesture = new UITapGestureRecognizer (DidTapToken);
			this.AddGestureRecognizer (gesture);
		}

		private void SetTitleText(string title)
		{
			titleLable.Text = title;
			titleLable.TextColor = _colorScheme;
			titleLable.SizeToFit ();
		}

		private void SetHighlighted(bool highlighted)
		{
			Highlighted = highlighted;
			var textColor = Highlighted ? UIColor.White : _colorScheme;
			var backgroundColor = Highlighted ? _colorScheme : UIColor.Clear;
			titleLable.TextColor = textColor;
			backgroundView.BackgroundColor = backgroundColor;
		}

		private void SetColorSheme(UIColor colorSheme)
		{
			_colorScheme = colorSheme;
			titleLable.TextColor = _colorScheme;
			SetHighlighted (Highlighted);
		}

		private void DidTapToken(UITapGestureRecognizer gesture)
		{

		}
	}
}