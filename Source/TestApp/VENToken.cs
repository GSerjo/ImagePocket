using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace TestApp
{
	public partial class VENToken : UIView
	{

		private UIColor _colorScheme = UIColor.Blue;
		private bool _highlighted;
		private const float DefaultVerticalInset = 7.0;
		private const float DefaultHorizontalInset = 15.0;
		private const float DefaultToLabelPadding = 5.0;
		private const float DefaultTokenPadding = 2.0;
		private const float DefaultMinImputWidth = 80.0;
		private const float DefaultMaxHeight = 150.0;
		private VENBackspaceTextField _invisibleTextField;
		private VENBackspaceTextField _inputTextField;
		private UILabel collapsedLabel;

		private UIScrollView _scrollView;
		private NSMutableArray _tokens;
		private float _orifinalHeight;
		private UITapGestureRecognizer _tapGestureRecognizer;

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
			_highlighted = highlighted;
			var textColor = _highlighted ? UIColor.White : _colorScheme;
			var backgroundColor = _highlighted ? _colorScheme : UIColor.Clear;
			titleLable.TextColor = textColor;
			backgroundView.BackgroundColor = backgroundColor;
		}

		private void SetColorSheme(UIColor colorSheme)
		{
			_colorScheme = colorSheme;
			titleLable.TextColor = _colorScheme;
			SetHighlighted (_highlighted);
		}

		private void DidTapToken(UITapGestureRecognizer gesture)
		{

		}
	}
}