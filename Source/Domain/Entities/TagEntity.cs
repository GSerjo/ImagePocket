using System;
using SQLite;

namespace Domain
{
	public sealed class TagEntity : Entity
	{
		[Indexed]
		public string Name { get; set; }
	}
}