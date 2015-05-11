using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;

namespace Dojo
{
	public sealed class AboutAppViewController : DialogViewController
	{
		public AboutAppViewController (string caption) : base(UITableViewStyle.Grouped,null, true)
		{
			Root = new RootElement (caption)
			{
				CreateSection()
			};
		}

		private static Section CreateSection()
		{
			var result = new Section
			{
				new StringElement("App Version", "1.0.1")
			};
			return result;
		}
	}
}

