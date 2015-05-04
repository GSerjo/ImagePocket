using System;
using System.Collections.Generic;

namespace Domain
{
    internal sealed class TagRepository
    {
        private static readonly TagRepository _instance = new TagRepository();

        private TagRepository()
        {
        }

        public static TagRepository Instance
        {
            get { return _instance; }
        }

        public List<TagEntity> GetAll()
        {
            return Database.GetAll<TagEntity>();
        }

        public void Remove(List<TagEntity> tags)
        {
            Database.Remove(tags);
        }

        public void SaveOrUpdate(List<TagEntity> values)
        {
            Database.AddOrUpdate(values);
        }
    }
}
