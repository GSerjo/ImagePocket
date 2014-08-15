using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Drawing;

namespace Dojo
{
	[Register("VENToken")]
	public partial class VENToken : UIView
	{
		private bool _highlighted;
		private UIColor _colorScheme;

		public VENToken (IntPtr handle) : base(handle)
		{
			Id = Guid.NewGuid ();
		}

		public UIColor ColorScheme
		{ 
			get
			{
				return _colorScheme;
			}
			set 
			{
				_colorScheme = value;
				titleLabel.TextColor = _colorScheme;
				Highlighted = false;
			}
		}

		public bool Highlighted
		{
			get
			{
				return _highlighted;
			}
			set
			{
				_highlighted = value;
				var textColor = _highlighted ? UIColor.White : ColorScheme;
				var backgroundColor = _highlighted ? ColorScheme : UIColor.Clear;
				titleLabel.TextColor = textColor;
				backgroundView.BackgroundColor = backgroundColor;
			}
		}
		public Action<VENToken> OnDidTapToken { get; set; }
		public Guid Id { get; private set; }

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

		private void DidTapToken(UITapGestureRecognizer gesture)
		{
			if (OnDidTapToken != null)
			{
				OnDidTapToken (this);
			}
			Console.WriteLine ("DidTapToken");
		}
	}
}