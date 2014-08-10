using System;
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

		public VENToken (IntPtr handle) : base(handle)
		{
			SetupInit ();
		}

		public void SetupInit()
		{
			Layer.CornerRadius = 5;
			ColorScheme = UIColor.Blue;
//			titleLable.TextColor = ColorScheme;
			UITapGestureRecognizer gesture = new UITapGestureRecognizer (DidTapToken);
			this.AddGestureRecognizer (gesture);
		}

		public void SetTitleText(string title)
		{
//			titleLable.Text = title;
//			titleLable.TextColor = ColorScheme;
//			Frame = new RectangleF (Frame.X, Frame.Y, titleLable.Frame.X + 3, Frame.Height);
//			titleLable.SizeToFit ();
		}

		private void SetHighlighted(bool highlighted)
		{
			Highlighted = highlighted;
			var textColor = Highlighted ? UIColor.White : ColorScheme;
			var backgroundColor = Highlighted ? ColorScheme : UIColor.Clear;
//			titleLable.TextColor = textColor;
//			backgroundView.BackgroundColor = backgroundColor;
		}

		public void SetColorSheme(UIColor colorSheme)
		{
			ColorScheme = colorSheme;
//			titleLable.TextColor = ColorScheme;
			SetHighlighted (Highlighted);
		}

		private void DidTapToken(UITapGestureRecognizer gesture)
		{
			Console.WriteLine ("DidTapToken");
		}
	}
}