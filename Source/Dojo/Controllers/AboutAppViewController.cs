using System;
using Foundation;
using MonoTouch.Dialog;
using UIKit;

namespace Dojo
{
    public sealed class AboutAppViewController : DialogViewController
    {
        private const string AppUrl = "itms-apps://itunes.apple.com/app/id353372460";

        public AboutAppViewController(string caption) : base(UITableViewStyle.Grouped, null, true)
        {
            Root = new RootElement(caption)
            {
                CreateSection()
            };
        }

        private static Section CreateSection()
        {
            var result = new Section
            {
                new StringElement("Rate the App", OpenApp),
                new StringElement("App Version", "1.0.1")
            };
            return result;
        }

        private static void OpenApp()
        {
            try
            {
                UIApplication.SharedApplication.OpenUrl(new NSUrl(AppUrl));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
