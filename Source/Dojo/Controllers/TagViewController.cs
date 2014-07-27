using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using Domain;
using System.Collections.Generic;
using System.Linq;

namespace Dojo
{
	public sealed class TagViewController : DialogViewController
	{
		private readonly TagRepository _repository = new TagRepository ();
		private readonly HomeViewController _homeViewController;

		public TagViewController (HomeViewController homeViewController) : base(UITableViewStyle.Plain, null)
		{
			_homeViewController = homeViewController;
			List<TagEntity> tags = _repository.GetAll();
			Root = new RootElement (string.Empty)
			{
				CreateTagSection(tags)
			};
		}

		private void FilterImage (TagEntity entity)
		{
			_homeViewController.SetTag(entity);
			NavigationController.PushViewController (_homeViewController, true);
		}

		private Section CreateTagSection(List<TagEntity> tags)
		{
			var result = new Section();
			var elements =tags.Select (x => new StyledStringElement (x.Name, () => FilterImage(x)));
			result.AddAll (elements.Cast<Element> ());
			return result;
		}
	}
}