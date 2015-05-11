using System;
using MonoTouch.Dialog;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

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
                new StringElement("Rate the App", () => UIApplication.SharedApplication.OpenUrl(new NSUrl(AppUrl))),
                new StringElement("App Version", "1.0.1")
            };
            return result;
        }
    }
}
