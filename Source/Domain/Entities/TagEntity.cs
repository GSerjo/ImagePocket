using System;
using SQLite;

namespace Domain
{
	public sealed class TagEntity : Entity
	{
		private static readonly TagEntity _all = new TagEntity{ Name = TagNames.All};
		private static readonly TagEntity _untagged = new TagEntity{ Name = TagNames.Untagged};

		[Indexed]
		public string Name { get; set; }

		[Ignore]
		public bool IsAll
		{
			get { return string.Equals (Name, _all.Name, StringComparison.OrdinalIgnoreCase); }
		}

		[Ignore]
		public bool IsUntagged
		{
			get { return string.Equals (Name, _untagged.Name, StringComparison.OrdinalIgnoreCase); }
		}


		[Ignore]
		public static TagEntity All
		{
			get { return _all; }
		}

		[Ignore]
		public static TagEntity Untagged
		{
			get { return _untagged; }
		}
	}
}