// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace Dojo
{
	[Register ("TagSelectorViewController")]
	partial class TagSelectorViewController
	{
		[Outlet]
		MonoTouch.UIKit.UITableView allTags { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem btCancel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem btDone { get; set; }

		[Outlet]
		NSTokenView.TokenView tagTokenView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (allTags != null) {
				allTags.Dispose ();
				allTags = null;
			}

			if (btCancel != null) {
				btCancel.Dispose ();
				btCancel = null;
			}

			if (btDone != null) {
				btDone.Dispose ();
				btDone = null;
			}

			if (tagTokenView != null) {
				tagTokenView.Dispose ();
				tagTokenView = null;
			}
		}
	}
}
