using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core;
using System.Linq;

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
			foreach (ImageEntity image in images)
			{
				if (image.Tags.IsEmpty ())
				{
					continue;
				}
				Database.AddOrUpdateAll (image.Tags).Wait ();
			}
			var saveImages = images.Where (x => x.Tags.IsNotEmpty ()).ToList ();
			return Database.AddOrUpdateAll (saveImages);
		}
	}
}