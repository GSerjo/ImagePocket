using System;
using System.Collections.Generic;

namespace Domain
{
	public class TagRepository
	{
		public List<TagEntity> GetAll()
		{
			return Database.GetAll<TagEntity> ();
		}
	}
}

