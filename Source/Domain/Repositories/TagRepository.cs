using System;
using System.Collections.Generic;

namespace Domain
{
	public sealed class TagRepository
	{

		private TagRepository()
		{
		}

		public static TagRepository Instance
		{
			get { return new TagRepository (); }
		}

		public List<TagEntity> GetAll()
		{
			return Database.GetAll<TagEntity> ();
		}
	}
}