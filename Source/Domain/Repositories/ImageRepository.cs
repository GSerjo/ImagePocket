using System;
using System.Collections.Generic;

namespace Domain
{
	public sealed class ImageRepository
	{

		private ImageRepository()
		{
		}

		public List<ImageEntity> GetAll()
		{
			return Database.GetAll<ImageEntity> ();
		}

		public static ImageRepository Instance
		{
			get { return new ImageRepository (); }
		}

	}
}