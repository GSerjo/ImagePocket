using System;

namespace Dojo
{
	public interface ITokenDelegate
	{
		void DidEnterText (VENTokenField tokenField, string text);

//		void TokenFieldDidBeginEditing(VENTokenField tokenField);
//		void InputTextFIeldDidChange(VENTokenField tokenField, string didChangeText);
//		void TokenFieldShouldReturn(VENTokenField tokenField, string didEnterText);
		void DidDeleteTokenAtIndex(VENTokenField tokenField, int index);
	}
}