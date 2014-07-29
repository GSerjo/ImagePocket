using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using System.Drawing;

namespace Dojo
{
	public sealed class TagSelectorViewController : UIViewController
	{
		private UIToolbar _toolbar;
		public event EventHandler<EventArgs> Closed = delegate { };

		public TagSelectorViewController ()
		{
			ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
			View.BackgroundColor = UIColor.White;

			_toolbar = new UIToolbar(new RectangleF(0, 0, View.Bounds.Width, 44));
			_toolbar.SetItems (new []
				{
					new UIBarButtonItem(UIBarButtonSystemItem.Cancel, OnCancel),
					new UIBarButtonItem(UIBarButtonSystemItem.FixedSpace, null),
					new UIBarButtonItem(UIBarButtonSystemItem.Done, OnDone),
				}, false);

			Add(_toolbar);
		}

		private void OnCancel(object sender, EventArgs ea)
		{
			Closed (null, EventArgs.Empty);
			DismissViewController (true, null);
		}

		private void OnDone(object sender, EventArgs ea)
		{
			DismissViewController (true, null);
		}
	}
}

