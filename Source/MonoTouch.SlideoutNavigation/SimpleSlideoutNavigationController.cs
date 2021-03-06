using System;
using System.Drawing;
using CoreGraphics;
using UIKit;

namespace MonoTouch.SlideoutNavigation
{
    /// <summary>
    /// A "Simple" slideout controller is a controller in which the top view simply slides
    /// to the right when the user opens the menu. Nothing fancy.
    /// </summary>
    public class SimpleSlideoutNavigationController : SlideoutNavigationController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MonoTouch.SlideoutNavigation.SimpleSlideoutNavigationController"/> class.
        /// </summary>
        public SimpleSlideoutNavigationController()
        {
            MenuWidth = 290f;
            ShadowEnabled = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether shadowing is enabled
        /// </summary>
        public bool ShadowEnabled { get; set; }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            //Create some shadowing
            if (ShadowEnabled)
            {
                ContainerView.Layer.ShadowOffset = new SizeF(-5, 0);
                ContainerView.Layer.ShadowPath = UIBezierPath.FromRect(ContainerView.Bounds).CGPath;
                ContainerView.Layer.ShadowRadius = 3.0f;
                ContainerView.Layer.ShadowColor = UIColor.Black.CGColor;
            }
        }

        /// <summary>
        /// Animate the specified menuView and mainView based on a percentage.
        /// </summary>
        /// <param name="menuView">The menu view.</param>
        /// <param name="mainView">The main view.</param>
        /// <param name="percentage">The floating point number (0-1) of how far to animate.</param>
        protected override void Animate(UIView menuView, UIView mainView, float percentage)
        {
            if (percentage > 1)
                percentage = 1;

            // Determine if shadow should be shown
            if (ShadowEnabled)
            {
                if (percentage <= 0)
                    mainView.Layer.ShadowOpacity = 0;
                else
                    ContainerView.Layer.ShadowOpacity = 0.3f;
            }

            var x = View.Bounds.X + (MenuWidth * percentage);
            mainView.Frame = new CGRect(new CGPoint(x, mainView.Frame.Y), mainView.Frame.Size);
        }
    }
}

