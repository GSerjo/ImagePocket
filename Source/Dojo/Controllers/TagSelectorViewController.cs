
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Domain;
using System.Collections.Generic;

namespace Dojo
{
	public partial class TagSelectorViewController : UIViewController
	{
		public event EventHandler<EventArgs> Closed = delegate { };
		public event EventHandler<EventArgs> Done = delegate { };
		private static TagRepository _tagRepository = TagRepository.Instance;

		public TagSelectorViewController () : base ("TagSelectorViewController", null)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			allTags.Source = new TableSource ();
			btCancel.Clicked += OnCancel;
			btDone.Clicked += OnDone;
			
			// Perform any additional setup after loading the view, typically from a nib.
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

		public class TableSource : UITableViewSource
		{
			protected List<TagEntity> _tags;
			protected string cellIdentifier = "TableCell";

			public TableSource ()
			{
				_tags = _tagRepository.GetAll();
			}

			public override int RowsInSection (UITableView tableview, int section)
			{
				return _tags.Count;
			}

			public override UITableViewCell GetCell (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
			{
				UITableViewCell cell = tableView.DequeueReusableCell (cellIdentifier);
				if (cell == null)
				{
					cell = new UITableViewCell (UITableViewCellStyle.Default, cellIdentifier);
				}
				cell.TextLabel.Text = _tags[indexPath.Row].Name;
				return cell;
			}
		}
	}
}

