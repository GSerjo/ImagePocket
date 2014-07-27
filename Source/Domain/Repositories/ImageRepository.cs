using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain
{
	public sealed class ImageRepository
	{

		private Dictionary<int, List<ImageEntity>> _images= new Dictionary<int, List<ImageEntity>>();

		public List<ImageEntity> GetAll()
		{
			return Database.GetAll<ImageEntity> ();
		}

		public void LoadAll()
		{
			_images = GetAll ()
				.GroupBy (x => x.TagId)
				.ToDictionary (x => x.Key, y=> y.ToList());
		}

		public List<ImageEntity> GetByTag(TagEntity tag)
		{
			if (tag.IsAll)
			{
				return _images.SelectMany (x => x.Value).ToList ();
			}
			if (_images.ContainsKey (tag.EntityId))
			{
				return _images [tag.EntityId];
			}
			return new List<ImageEntity> ();
		}

	}
}