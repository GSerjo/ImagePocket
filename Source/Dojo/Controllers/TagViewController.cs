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
		private readonly TagCache _repository = TagCache.Instance;
		private readonly HomeViewController _homeViewController;
		private Section _dataSection;

		public TagViewController (HomeViewController homeViewController) : base(UITableViewStyle.Plain, null)
		{
			_homeViewController = homeViewController;
			List<TagEntity> tags = _repository.GetAll();
			_dataSection = CreateTagSection (tags);
			Root = new RootElement (string.Empty)
			{
				_dataSection
			};
		}

		public override void ViewWillAppear (bool animated)
		{
			_dataSection.Clear ();
			_dataSection.AddAll (CreateElements (_repository.GetAll ()));
			base.ViewWillAppear (animated);
		}

		private void FilterImage (TagEntity entity)
		{
			_homeViewController.SetTag(entity);
			NavigationController.PushViewController (_homeViewController, true);
		}

		private Section CreateTagSection(List<TagEntity> tags)
		{
			var result = new Section();
			result.AddAll (CreateElements (tags));
			return result;
		}

		private List<Element> CreateElements(List<TagEntity> tags)
		{
			var result = tags.Select (x => new StyledStringElement (x.Name, () => FilterImage (x)))
							   .Cast<Element> ()
							   .ToList ();
			return result;
		}
	}
}