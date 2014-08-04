using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain
{
	public sealed class ImageRepository
	{

		private static ImageRepository _instance = new ImageRepository();

		private ImageRepository()
		{
		}

		public List<ImageEntity> GetAll()
		{
			return Database.GetAll<ImageEntity> ();
		}

		public static ImageRepository Instance
		{
			get { return _instance; }
		}

		public Task SaveOrUpdate(List<ImageEntity> images)
		{
			return Database.AddOrUpdateAll (images);
		}
	}
}