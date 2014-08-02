using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using System.Drawing;
using Domain;
using System.Collections.Generic;

namespace Dojo
{
	public sealed class TagSelectorViewController : UIViewController
	{
		public event EventHandler<EventArgs> Closed = delegate { };
		public event EventHandler<EventArgs> Done = delegate { };
		private static TagRepository _tagRepository = TagRepository.Instance;

		public TagSelectorViewController ()
		{
			View.BackgroundColor = UIColor.White;
			NavigationItem.LeftBarButtonItem = new UIBarButtonItem (UIBarButtonSystemItem.Cancel, OnCancel);
			NavigationItem.RightBarButtonItem = new UIBarButtonItem (UIBarButtonSystemItem.Done, OnDone);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			var tableBounds = new RectangleF (0, 44, View.Bounds.Width, View.Bounds.Height - 44);
			var table = new UITableView (tableBounds);
			var tagBounds = new RectangleF (0, 44, View.Bounds.Width, 20);
			var tagTextField = new UITextField (tagBounds);
			Add (tagTextField);
//			Add (table);
			table.Source = new TableSource ();

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