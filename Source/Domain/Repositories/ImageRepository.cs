using System;
using System.Collections.Generic;
using System.Linq;
using Core;

namespace Domain
{
    internal sealed class ImageRepository
    {
        private static readonly ImageRepository _instance = new ImageRepository();

        private ImageRepository()
        {
        }

        public static ImageRepository Instance
        {
            get { return _instance; }
        }

        public List<ImageEntity> GetAll()
        {
            return Database.GetAll<ImageEntity>();
        }

        public void Remove(List<ImageEntity> images)
        {
            Database.Remove(images);
        }

        public void SaveOrUpdate(List<ImageEntity> images)
        {
            if (images.IsNullOrEmpty())
            {
                return;
            }
            foreach (ImageEntity image in images)
            {
                if (image.Tags.IsEmpty() && image.New)
                {
                    continue;
                }
                TagCache.Instance.SaveOrUpdate(image.Tags);
            }

            List<ImageEntity> forRemove = images.Where(x => x.Tags.IsEmpty() && x.New == false).ToList();
            List<ImageEntity> forAddOrUpdate = images.Where(x => x.Tags.IsNotEmpty()).ToList();

            Database.Remove(forRemove);
            Database.AddOrUpdate(forAddOrUpdate);
        }
    }
}
