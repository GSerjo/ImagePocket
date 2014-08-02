using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using Domain;
using System.Linq;

namespace Dojo
{
	public sealed class TagSelectorViewController1 : DialogViewController
	{
		public event EventHandler<EventArgs> Closed = delegate { };
		public event EventHandler<EventArgs> Done = delegate { };
		private static TagRepository _tagRepository = TagRepository.Instance;

		public TagSelectorViewController1 () : base(UITableViewStyle.Grouped, null)
		{
			Root = new RootElement ("Add Tags")
			{
				CreateImageTagSection(),
				CreateAllTagSection()
			};
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			ConfigureToolbar ();
		}

		private Section CreateImageTagSection()
		{
			var result = new Section ();
			result.Add(new EntryElement(string.Empty, "Enter tags", string.Empty));
			return result;
		}

		private Section CreateAllTagSection()
		{
			var result = new Section ();
			var tags = _tagRepository.GetAll ();
			var elements =tags.Select (x => new StyledStringElement (x.Name, () => TagSelected(x)));
			result.AddAll (elements.Cast<Element> ());
			return result;
		}

		private void TagSelected(TagEntity tag)
		{
		}

		private void ConfigureToolbar ()
		{
			NavigationItem.LeftBarButtonItem = new UIBarButtonItem (UIBarButtonSystemItem.Cancel, OnCancel);
			NavigationItem.RightBarButtonItem = new UIBarButtonItem (UIBarButtonSystemItem.Done, OnDone);
		}

		private void OnCancel(object sender, EventArgs ea)
		{
			Closed (null, EventArgs.Empty);
			DismissViewController (true, null);
		}

		private void OnDone(object sender, EventArgs ea)
		{
			Done (null, EventArgs.Empty);
			DismissViewController (true, null);
		}
	}
}

