using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace Domain
{
	public sealed class ImageEntity : Entity
	{
		private List<int> _tags = new List<int>();
		private string _tagInternal = string.Empty;
		private const string Separator = ",";

		[Indexed]
		public string LocalIdentifier { get; set; }

		public string TagsInternal
		{
			get
			{
				_tagInternal = string.Join (Separator, _tags);
				return _tagInternal;
			}
			set
			{
				_tagInternal = value;
			}
		}

		[Ignore]
		public List<int> Tags
		{
			get
			{
				if (_tags.Count == 0)
				{
					_tags = _tagInternal.Split(Separator[0])
						.Select (x => int.Parse (x))
						.ToList ();
				}
				return _tags;
			}
		}

		public override int GetHashCode ()
		{
			return ((LocalIdentifier != null ? LocalIdentifier.GetHashCode () : 0) * 397) ^ EntityId;
		}
	}
}