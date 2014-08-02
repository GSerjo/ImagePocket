using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

		public Task<int> SaveOrUpdate(List<ImageEntity> images)
		{
			return Database.Add (images);
		}
	}
}