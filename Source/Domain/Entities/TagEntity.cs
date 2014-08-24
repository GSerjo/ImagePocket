using System;
using SQLite;

namespace Domain
{
	public sealed class TagEntity : Entity
	{
		private static readonly TagEntity _all = new TagEntity { Name = "All", EntityId = -1};
		private static readonly TagEntity _untagged = new TagEntity { Name = "Untagged" , EntityId = -2};
		private static readonly TagEntity _addTagRequest = new TagEntity { EntityId = -3};

		[Indexed]
		public string Name { get; set; }

		[Ignore]
		public bool IsAll
		{
			get { return _all.EntityId == EntityId; }
		}

		[Ignore]
		public bool IsUntagged
		{
			get { return _untagged.EntityId == EntityId; }
		}

		[Ignore]
		public bool IsAddTagRequest
		{
			get { return _addTagRequest.EntityId == EntityId; }
		}

		[Ignore]
		public static TagEntity All
		{
			get { return _all; }
		}

		[Ignore]
		public static TagEntity AddTagRequest
		{
			get { return _addTagRequest; }
		}

		[Ignore]
		public static TagEntity Untagged
		{
			get { return _untagged; }
		}
	}
}