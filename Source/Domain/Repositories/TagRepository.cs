using System;
using System.Collections.Generic;
using System.Linq;
using Core;

namespace Domain
{
	public sealed class TagRepository
	{
		private static TagRepository _instance = new TagRepository();
		private Dictionary<int, TagEntity> _tags = new Dictionary<int, TagEntity> ();

		private TagRepository()
		{
			_tags = Database.GetAll<TagEntity> ().ToDictionary (x => x.EntityId);
		}

		public static TagRepository Instance
		{
			get { return _instance; }
		}

		public List<TagEntity> GetAll()
		{
			var result = _tags.Values.OrderBy(x => x.Name).ToList();
			result.Insert (0, TagEntity.All);
			result.Insert (1, TagEntity.Untagged);
			return result;
		}

		public List<TagEntity> GetUserTags()
		{
			return _tags.Values
				.Where (x => !x.IsAll && !x.IsUntagged)
				.ToList ();
		}

		public TagEntity GetById(int id)
		{
			return _tags [id];
		}

		public List<TagEntity> GetById(IEnumerable<int> ids)
		{
			var result = new List<TagEntity> ();
			foreach (int id in ids)
			{
				result.Add (_tags [id]);
			}
			return result;
		}

		public bool Contains(string name)
		{
			var tag = _tags.Values.SingleOrDefault(x=>string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
			return tag != null;
		}

		public List<TagEntity> Filter(string name)
		{
			return _tags.Values.Where (x => x.Name.Contains (name)).ToList ();
		}

		public void SaveOrUpdate(List<TagEntity> values)
		{
			Database.AddOrUpdateAll (values);
			values.Iter (x => _tags [x.EntityId] = x);
		}
	}
}