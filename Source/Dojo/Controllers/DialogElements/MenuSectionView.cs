using System;
using System.Drawing;
using MonoTouch.UIKit;

namespace Dojo
{
    public sealed class MenuSectionView : UIView
    {
        public MenuSectionView(string caption)
            : base(new RectangleF(0, 0, 320, 27))
        {
            //            BackgroundColor = Themes.Theme.Current.PrimaryNavigationBarColor;
            var label = new UILabel
            {
                BackgroundColor = UIColor.Clear,
                Opaque = false,
                TextColor = UIColor.FromRGB(171, 171, 171),
                Font = UIFont.BoldSystemFontOfSize(12.5f),
                Frame = new RectangleF(8, 1, 200, 23),
                Text = caption
            };
            AddSubview(label);
        }
    }
}
