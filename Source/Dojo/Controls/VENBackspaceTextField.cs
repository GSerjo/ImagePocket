using System;
using MonoTouch.UIKit;
using MonoTouch.ObjCRuntime;

namespace Dojo
{
	public class VENBackspaceTextField : UITextField
	{
		//public ITokenDelegate Delgate {	get; set; }

		public override void DeleteBackward ()
		{
			if (Text.Length == 0)
			{ 
				if (RespondsToSelector (new Selector ("textFieldDidEnterBackspace:"))) 
				{
					Console.WriteLine ("Test");
				}
			}
			base.DeleteBackward ();
		}
	}
}