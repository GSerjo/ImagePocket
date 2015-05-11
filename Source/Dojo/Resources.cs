using System;
using MonoTouch.UIKit;

namespace Dojo
{
    public static class Resources
    {
        public static readonly UIImage AboutImage = UIImage.FromBundle("AboutSet");
        public static readonly UIImage FlagImage = UIImage.FromBundle("FlagSet");
        //		public static readonly UIColor NavigationBarTintColor = UIColor.FromRGB(65, 82, 59);
        //		public static readonly UIColor NavigationBarTintColor = UIColor.FromRGB(26, 86, 125);
        //		public static readonly UIColor NavigationBarTintColor = UIColor.FromRGB(82, 93, 107);
        public static readonly UIColor NavigationBarTextColor = UIColor.FromRGB(255, 255, 255);
        public static readonly UIColor NavigationBarTintColor = UIColor.FromRGB(66, 78, 83);
        public static readonly UIImage SelectImage = UIImage.FromBundle("Selected");
    }
}
