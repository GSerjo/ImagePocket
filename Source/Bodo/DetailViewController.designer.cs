// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;

namespace MasterDetail
{
	[Register ("DetailViewController")]
	partial class DetailViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UICollectionView CollectionView { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (CollectionView != null) {
				CollectionView.Dispose ();
				CollectionView = null;
			}
		}
	}
}
