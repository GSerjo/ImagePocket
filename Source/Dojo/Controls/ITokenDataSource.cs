using System;

namespace Dojo
{
	public interface ITokenDataSource
	{
		int NumberOfTokensInTokenField(VENTokenField tokenField); 
		string TokenField(VENTokenField tokenField, int index);
		string TokenFieldCollapsedText(VENTokenField tokenField);
	}
}