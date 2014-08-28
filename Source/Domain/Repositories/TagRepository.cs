using System;
using System.Collections.Generic;
using System.Linq;
using Core;

namespace Domain
{
	internal sealed class TagRepository
	{
		private static TagRepository _instance = new TagRepository();

		private TagRepository()
		{
		}

		public static TagRepository Instance
		{
			get { return _instance; }
		}

		public List<TagEntity> GetAll()
		{
			return Database.GetAll<TagEntity> ();
		}

		public void SaveOrUpdate(List<TagEntity> values)
		{
			Database.AddOrUpdateAll (values);
		}
	}
}