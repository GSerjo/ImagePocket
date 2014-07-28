using System;
using SQLite;

namespace Domain
{
	public abstract class Entity
	{
		[PrimaryKey, AutoIncrement]
		public int EntityId { get; set; }

		[Ignore]
		public bool New
		{
			get { return EntityId == 0; }
		}
	}
}

