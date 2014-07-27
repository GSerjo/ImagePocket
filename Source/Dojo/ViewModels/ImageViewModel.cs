using System;
using Domain;

namespace Dojo
{
	public sealed class ImageViewModel
	{
		private ImageEntity _entity;

		public ImageViewModel (ImageEntity entity)
		{
			_entity = entity;
		}

		public bool Selected { get; set; }
	}
}