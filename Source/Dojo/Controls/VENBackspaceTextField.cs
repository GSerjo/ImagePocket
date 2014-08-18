using System;
using MonoTouch.UIKit;
using MonoTouch.ObjCRuntime;
using System.Drawing;
using MonoTouch.Foundation;

namespace Dojo
{
	[Register ("VENBackspaceTextField")]
	public class VENBackspaceTextField : UITextField
	{
		[Export("initWithFrame:")]
		public VENBackspaceTextField(RectangleF frame) : base(frame)
		{

		}

		public override void DeleteBackward ()
		{
			Console.WriteLine ("DeleteBackward");
			if (Text.Length == 0)
			{ 
			}
			base.DeleteBackward ();
		}
	}
}