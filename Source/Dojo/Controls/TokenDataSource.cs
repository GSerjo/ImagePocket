using System;

namespace Dojo
{
	public class TokenDataSource
	{
		public virtual int NumberOfTokens(VENTokenField tokenField)
		{
			throw new NotImplementedException ();
		}

		public virtual string GetToken(VENTokenField tokenField, int index)
		{
			throw new NotImplementedException ();
		}
	}
}