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
		private const string Separator = ",";
		private TagCache _tagRepository = TagCache.Instance;

		public ImageEntity ()
		{
			CreateTime = DateTime.MinValue;
			TagsInternal = string.Empty;
		}

		[Indexed]
		public string LocalIdentifier { get; set; }

		public string TagsInternal { get; set; }

		public DateTime CreateTime { get; set; }

		[Ignore]
		public List<TagEntity> Tags
		{
			get
			{
				if (_tags.IsNotEmpty())
				{
					return _tags;
				}
				if (string.IsNullOrEmpty (TagsInternal)) 
				{
					return new List<TagEntity> ();
				}
				_tags = TagsInternal.Split(Separator[0])
					.Select (x => int.Parse (x))
					.Select(x => _tagRepository.GetById(x))
					.ToList ();
				return _tags;
			}
		}

		public List<TagEntity> GetRemovedTags(ImageEntity entity)
		{
			var result = Tags.Except (entity.Tags, new FuncComparer<TagEntity> ((x, y) => x.Equals(y)));
			return result.ToList();
		}

		public ImageEntity CloneDeep()
		{
			var result = new ImageEntity
			{
				EntityId = EntityId,
				LocalIdentifier = LocalIdentifier,
				TagsInternal = TagsInternal,
				CreateTime = CreateTime
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
			TagsInternal = TagsToString ();
		}

		public void RemoveTag (TagEntity tag)
		{
			_tags.RemoveAll(x => x.EntityId == tag.EntityId);
			TagsInternal = TagsToString ();
		}

		public bool ContainsTag(TagEntity tag)
		{
			return TagExists (tag);
		}

		public override int GetHashCode ()
		{
			return ((LocalIdentifier != null ? LocalIdentifier.GetHashCode () : 0) * 397) ^ EntityId;
		}

		public bool Equals(ImageEntity value)
		{
			if (ReferenceEquals (null, value))
			{
				return false;
			}
			if(ReferenceEquals(this, value))
			{
				return true;
			}
			return value.EntityId == EntityId;		
		}

		private string TagsToString()
		{
			if (_tags.IsNullOrEmpty ())
			{
				return string.Empty;
			}
			return string.Join (Separator, _tags.Select(x => x.EntityId));
		}

		private bool TagExists(TagEntity tag)
		{
			return Tags.Exists (x => x.EntityId == tag.EntityId);
		}
	}
}