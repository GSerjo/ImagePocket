using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;
using Core;

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
				if (_tags.IsNotEmpty())
				{
					return _tags;
				}
				if (string.IsNullOrEmpty (_tagInternal)) 
				{
					return new List<int> ();
				}
				_tags = _tagInternal.Split(Separator[0])
					.Select (x => int.Parse (x))
					.ToList ();
				return _tags;
			}
		}

		public ImageEntity CloneDeep()
		{
			var result = new ImageEntity
			{
				EntityId = EntityId,
				LocalIdentifier = LocalIdentifier,
				TagsInternal = _tagInternal
			};
			result._tags.AddRange (Tags);
			return result;
		}

		public void AddTag(TagEntity tag)
		{
			if (_tags.Contains (tag.EntityId))
			{
				return;
			}
			_tags.Add (tag.EntityId);
		}

		public void RemoveTag (TagEntity tag)
		{
			if (!_tags.Contains (tag.EntityId))
			{
				return;
			}
			_tags.Remove (tag.EntityId);
		}

		public override int GetHashCode ()
		{
			return ((LocalIdentifier != null ? LocalIdentifier.GetHashCode () : 0) * 397) ^ EntityId;
		}
	}
}