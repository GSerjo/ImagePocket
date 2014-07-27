using System;
using System.Collections.Generic;

namespace Domain
{
	public sealed class ImageRepository
	{
		public List<ImageEntity> GetAll()
		{
			return Database.GetAll<ImageEntity> ();
		}
	}
}