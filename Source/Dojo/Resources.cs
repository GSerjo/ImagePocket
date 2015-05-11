using System;
using MonoTouch.UIKit;

namespace Dojo
{
    public static class Resources
    {
        public static readonly UIImage AboutImage = UIImage.FromBundle("AboutSet");
        public static readonly UIImage FlagImage = UIImage.FromBundle("FlagSet");
        public static readonly UIColor NavigationBarTintColor = UIColor.FromRGB(82, 93, 107);
        public static readonly UIImage SelectImage = UIImage.FromBundle("Selected");
    }
}
