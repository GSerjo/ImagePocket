using System;

namespace Core
{
	public sealed class EventArgsOf<T> : EventArgs
	{
		public T Data { get; set; }
	}
}

