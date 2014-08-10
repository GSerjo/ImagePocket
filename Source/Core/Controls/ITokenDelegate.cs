using System;

namespace Core
{
	public interface ITokenDelegate
	{
		void TokenFieldDidBeginEditing(VENTokenField tokenField);
		void InputTextFIeldDidChange(VENTokenField tokenField, string didChangeText);
		void TokenFieldShouldReturn(VENTokenField tokenField, string didEnterText);
	}
}