﻿using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.SlideoutNavigation;
using System.Drawing;

namespace Dojo
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		UIWindow window;
		public SlideoutNavigationController Menu { get; private set; }

		//
		// This method is invoked when the application has loaded and is ready to run. In this
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			// create a new window instance based on the screen size
			window = new UIWindow (UIScreen.MainScreen.Bounds);

			Menu = new SimpleSlideoutNavigationController ();
			Menu.MainViewController = new MainNavigationController (CreateHomeController(), Menu);
			Menu.MenuViewController = new MenuNavigationController (new TagViewController (), Menu)
			{
				NavigationBarHidden = true
			};
			// If you have defined a root view controller, set it here:
			window.RootViewController = Menu;
			
			// make the window visible
			window.MakeKeyAndVisible();
			
			return true;
		}

		private UIViewController CreateHomeController()
		{
			var layout = new UICollectionViewFlowLayout
			{
				MinimumInteritemSpacing = 10.0f,
				SectionInset = new UIEdgeInsets (10, 15, 10, 15),
				ItemSize = new SizeF(120, 120)
			};
			var controller = new HomeViewController (layout);
			return controller;
		}
	}
}

