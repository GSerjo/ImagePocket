﻿using System;
using System.Drawing;
using System.Linq;
using Foundation;
using MonoTouch.SlideoutNavigation;
using UIKit;
using Xamarin;

namespace Dojo
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to
    // application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        // class-level declarations
        private UIWindow window;
        public SlideoutNavigationController Menu { get; private set; }

        //
        // This method is invoked when the application has loaded and is ready to run. In this
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
			try
			{
				// create a new window instance based on the screen size
				window = new UIWindow (UIScreen.MainScreen.Bounds);
            	
				Menu = new SimpleSlideoutNavigationController ();
				HomeViewController homeController = CreateHomeController ();
				Menu.MainViewController = new MainNavigationController (homeController, Menu);
				Menu.MenuViewController = new MenuNavigationController (new TagViewController (homeController), Menu) {
					NavigationBarHidden = true,
				};
            	
				// If you have defined a root view controller, set it here:
				window.RootViewController = Menu;
				UpdateTheme ();
            	
				// make the window visible
				window.MakeKeyAndVisible ();
            	
				return true;
			}
			catch (Exception ex)
			{
				Insights.Report (ex);
				throw;
			}
        }

        private HomeViewController CreateHomeController()
        {
            //			var layout = new CollectionViewWaterfallLayout
            //			{
            //				SectionInset = new UIEdgeInsets (10, 10, 10, 10),
            //				MinimumColumnSpacing = 10,
            //				MinimumInteritemSpacing = 10
            //			};

            var layout = new UICollectionViewFlowLayout
            {
                MinimumInteritemSpacing = 10.0f,
                MinimumLineSpacing = 10.0f,
                SectionInset = new UIEdgeInsets(10, 10, 10, 10),
                ItemSize = new SizeF(160, 160)
            };
            var controller = new HomeViewController(layout);
            return controller;
        }

        private void UpdateTheme()
        {
            var bars = new[] { typeof(MenuNavigationController), typeof(MainNavigationController) };
            foreach (UINavigationBar.UINavigationBarAppearance item in bars.Select(x => UINavigationBar.AppearanceWhenContainedIn(x)))
            {
                item.TintColor = Resources.NavigationBarTextColor;
                item.SetTitleTextAttributes(new UITextAttributes
                {
                    TextColor = Resources.NavigationBarTextColor,
                    Font = UIFont.SystemFontOfSize(18f)
                });
                item.BarTintColor = Resources.NavigationBarTintColor;
            }
            //			foreach (var item in bars.Select(x=>UIToolbar.AppearanceWhenContainedIn(x)))
            //			{
            //				item.BarTintColor = Resources.NavigationBarTintColor;
            //				item.TintColor = Resources.NavigationBarTextColor;
            //			}
        }
    }
}
