﻿using System;
using System.Collections.Generic;
using System.Linq;
using Core;

namespace Domain
{
    public sealed class TagCache
    {
        private static readonly TagCache _instance = new TagCache();
        private readonly TagRepository _tagRepository = TagRepository.Instance;
        private readonly Dictionary<int, TagEntity> _tags = new Dictionary<int, TagEntity>();

        private TagCache()
        {
            _tags = Database.GetAll<TagEntity>().ToDictionary(x => x.EntityId);
        }

        public static TagCache Instance
        {
            get { return _instance; }
        }

        public bool Contains(string name)
        {
            TagEntity tag = _tags.Values.SingleOrDefault(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
            return tag != null;
        }

        public List<TagEntity> Filter(string name)
        {
            return _tags.Values.Where(x => x.Name.Contains(name)).ToList();
        }

        public List<TagEntity> GetAll()
        {
            List<TagEntity> result = _tags.Values.OrderBy(x => x.Name).ToList();
            result.Insert(0, TagEntity.All);
            result.Insert(1, TagEntity.Untagged);
            return result;
        }

        public TagEntity GetById(int id)
        {
            return _tags[id];
        }

        public List<TagEntity> GetById(IEnumerable<int> ids)
        {
            var result = new List<TagEntity>();
            foreach (int id in ids)
            {
                result.Add(_tags[id]);
            }
            return result;
        }

        public List<TagEntity> GetUserTags()
        {
            return _tags.Values
                        .Where(x => !x.IsAll && !x.IsUntagged)
                        .ToList();
        }

        public void Remove(List<TagEntity> tags)
        {
            if (tags.IsNullOrEmpty())
            {
                return;
            }
            tags.Iter(x => _tags.Remove(x.EntityId));
            _tagRepository.Remove(tags);
        }

        public void SaveOrUpdate(List<TagEntity> values)
        {
            _tagRepository.SaveOrUpdate(values);
            values.Iter(x => _tags[x.EntityId] = x);
        }
    }
}
