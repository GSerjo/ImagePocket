using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using System.Drawing;

namespace Dojo
{
	public sealed class TagSelectorViewController : UIViewController
	{
		public event EventHandler<EventArgs> Closed = delegate { };
		public event EventHandler<EventArgs> Done = delegate { };

		public TagSelectorViewController ()
		{
			View.BackgroundColor = UIColor.White;
			NavigationItem.LeftBarButtonItem = new UIBarButtonItem (UIBarButtonSystemItem.Cancel, OnCancel);
			NavigationItem.RightBarButtonItem = new UIBarButtonItem (UIBarButtonSystemItem.Done, OnDone);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
		}

		private void OnCancel(object sender, EventArgs ea)
		{
			Closed (null, EventArgs.Empty);
			DismissViewController (true, null);
		}

		private void OnDone(object sender, EventArgs ea)
		{
			Done (null, EventArgs.Empty);
			DismissViewController (true, null);
		}
	}
}