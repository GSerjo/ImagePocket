using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using SQLite;

namespace Domain
{
    public sealed class ImageEntity : Entity
    {
        private const string Separator = ",";
        private readonly TagCache _tagCache = TagCache.Instance;
        private List<TagEntity> _tags = new List<TagEntity>();
        private string _tagsInternal;

        public ImageEntity()
        {
            CreateTime = DateTime.MinValue;
        }

        public DateTime CreateTime { get; set; }

        [Indexed]
        public string LocalIdentifier { get; set; }

        [Ignore]
        public List<TagEntity> Tags
        {
            get
            {
                if (_tags.IsNotEmpty())
                {
                    return _tags;
                }
                if (string.IsNullOrEmpty(_tagsInternal))
                {
                    return new List<TagEntity>();
                }
                _tags = _tagsInternal.Split(Separator[0])
                                     .Select(x => int.Parse(x))
                                     .Where(_tagCache.Contains)
                                     .Select(x => _tagCache.GetById(x))
                                     .ToList();
                return _tags;
            }
        }

        public string TagsInternal
        {
            get
            {
                _tagsInternal = TagsToString();
                return _tagsInternal;
            }
            set { _tagsInternal = value; }
        }

        public void AddTag(TagEntity tag)
        {
            if (TagExists(tag))
            {
                return;
            }
            _tags.Add(tag);
            _tagsInternal = TagsToString();
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

        public bool ContainsTag(TagEntity tag)
        {
            return TagExists(tag);
        }

        public bool Equals(ImageEntity value)
        {
            if (ReferenceEquals(null, value))
            {
                return false;
            }
            if (ReferenceEquals(this, value))
            {
                return true;
            }
            return value.EntityId == EntityId
                   && string.Equals(value.LocalIdentifier, LocalIdentifier, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return ((LocalIdentifier != null ? LocalIdentifier.GetHashCode() : 0) * 397) ^ EntityId;
        }

        public List<TagEntity> GetRemovedTags(ImageEntity entity)
        {
            IEnumerable<TagEntity> result = Tags.Except(entity.Tags, new FuncComparer<TagEntity>((x, y) => x.Equals(y)));
            return result.ToList();
        }

        public void RemoveTag(TagEntity tag)
        {
            _tags.RemoveAll(x => x.EntityId == tag.EntityId);
            _tagsInternal = TagsToString();
        }

        public void Update(ImageEntity value)
        {
            _tags.Clear();
            _tags.AddRange(value.Tags);
            EntityId = value.EntityId;
            _tagsInternal = TagsToString();
        }

        private bool TagExists(TagEntity tag)
        {
            return tag.New == false && Tags.Exists(x => x.EntityId == tag.EntityId);
        }

        private string TagsToString()
        {
            if (_tags.IsNullOrEmpty())
            {
                return string.Empty;
            }
            return string.Join(Separator, _tags.Select(x => x.EntityId));
        }
    }
}
