using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace Dojo
{
	public class HomeViewController : UICollectionViewController
	{
		public HomeViewController () : base(new UICollectionViewFlowLayout())
		{
			Title = "ImagePacket";
		}
	}
}

