using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;

namespace Dojo
{
	public sealed class TagSelectorViewController1 : DialogViewController
	{
		public event EventHandler<EventArgs> Closed = delegate { };
		public event EventHandler<EventArgs> Done = delegate { };

		public TagSelectorViewController1 () : base(UITableViewStyle.Grouped, null)
		{
			NavigationItem.LeftBarButtonItem = new UIBarButtonItem (UIBarButtonSystemItem.Cancel, OnCancel);
			NavigationItem.RightBarButtonItem = new UIBarButtonItem (UIBarButtonSystemItem.Done, OnDone);
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

