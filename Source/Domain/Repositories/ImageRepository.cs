using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core;
using System.Linq;

namespace Domain
{
	internal sealed class ImageRepository
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

		public void SaveOrUpdate(List<ImageEntity> images)
		{
			if (images.IsNullOrEmpty ())
			{
				return;
			}
			foreach (ImageEntity image in images)
			{
				if (image.Tags.IsEmpty () && image.New)
				{
					continue;
				}
				TagCache.Instance.SaveOrUpdate (image.Tags);
			}

			var removeImages = images.Where (x => x.Tags.IsEmpty () && x.New == false).ToList ();
			var addOrUpdate = images.Where (x => x.Tags.IsNotEmpty ()).ToList ();

			Database.Remove (removeImages);
			Database.AddOrUpdateAll (addOrUpdate);
		}
	}
}