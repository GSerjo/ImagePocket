using System;
using System.Collections.Generic;

namespace Dojo
{
	public class ImageWindowViewModel
	{
		private Dictionary<string, List<ImageViewModel>> _images;

		public ImageWindowViewModel ()
		{
			_images = new Dictionary<string, List<ImageViewModel>> ();
		}
	}
}

