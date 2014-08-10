// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace Core
{
	[Register ("TokenField")]
	partial class VENToken
	{
		[Outlet]
		MonoTouch.UIKit.UIView backgroundView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel titleLable { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (titleLable != null) {
				titleLable.Dispose ();
				titleLable = null;
			}

			if (backgroundView != null) {
				backgroundView.Dispose ();
				backgroundView = null;
			}
		}
	}
}
