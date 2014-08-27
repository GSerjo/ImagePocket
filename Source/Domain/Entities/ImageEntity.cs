using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;
using Core;

namespace Domain
{
	public sealed class ImageEntity : Entity
	{
		private List<TagEntity> _tags = new List<TagEntity>();
		private string _tagInternal = string.Empty;
		private const string Separator = ",";
		private TagRepository _tagRepository = TagRepository.Instance;

		[Indexed]
		public string LocalIdentifier { get; set; }

		public string TagsInternal
		{
			get
			{
				_tagInternal = string.Join (Separator, _tags.Select(x => x.EntityId));
				return _tagInternal;
			}
			set
			{
				_tagInternal = value;
			}
		}

		[Ignore]
		public List<TagEntity> Tags
		{
			get
			{
				if (_tags.IsNotEmpty())
				{
					return _tags;
				}
				if (string.IsNullOrEmpty (_tagInternal)) 
				{
					return new List<TagEntity> ();
				}
				_tags = _tagInternal.Split(Separator[0])
					.Select (x => int.Parse (x))
					.Select(x => _tagRepository.GetById(x))
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
			return result;
		}

		public void AddTag(TagEntity tag)
		{
			if (TagExists(tag))
			{
				return;
			}
			_tags.Add (tag);
		}

		public void RemoveTag (TagEntity tag)
		{
			if (!TagExists(tag))
			{
				return;
			}
			_tags.RemoveAll (x => x.EntityId == tag.EntityId);
		}

		public bool ContainsTag(TagEntity tag)
		{
			return TagExists (tag);
		}

		public override int GetHashCode ()
		{
			return ((LocalIdentifier != null ? LocalIdentifier.GetHashCode () : 0) * 397) ^ EntityId;
		}

		private bool TagExists(TagEntity tag)
		{
			return Tags.Exists (x => x.EntityId == tag.EntityId);
		}
	}
}