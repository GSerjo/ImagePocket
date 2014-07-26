using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;

namespace Dojo
{
	public class TagViewController : DialogViewController
	{
		public TagViewController () : base(UITableViewStyle.Plain, new RootElement(string.Empty))
		{
		}
	}
}

