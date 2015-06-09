using System;
using Foundation;
using MonoTouch.Dialog;
using UIKit;

namespace Dojo
{
    public sealed class AboutAppViewController : DialogViewController
    {
        private const string AppUrl = "itms-apps://itunes.apple.com/app/id353372460";
		private const string TwitterUrl = "https://twitter.com/ImagePocketApp";

        public AboutAppViewController(string caption) : base(UITableViewStyle.Grouped, null, true)
        {
            Root = new RootElement(caption)
            {
                CreateSection()
            };
        }

        private static Section CreateSection()
        {
			var result = new Section {
				new StringElement ("Follow on Twitter", ()=> OpenApp(TwitterUrl)),
				new StringElement ("Rate the App", () => OpenApp (AppUrl)),
				new StringElement ("App Version", "1.0.1")
			};
            return result;
        }

        private static void OpenApp(string url)
        {
            try
            {
				UIApplication.SharedApplication.OpenUrl(new NSUrl(url));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
