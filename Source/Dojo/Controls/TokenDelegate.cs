using System;

namespace Dojo
{
	public class TokenDelegate
	{
		public virtual void FilterToken (VENTokenField tokenField, string text)
		{
		}

		public virtual void DidDeleteTokenAtIndex(VENTokenField tokenField, int index)
		{
		}
	}
}