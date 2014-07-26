using System;
using SQLite;

namespace Domain
{
	public sealed class ImageEntity : Entity
	{
		public string LocalIdentifier { get; set; }

		[Indexed]
		public int TagId { get; set; }
	}
}