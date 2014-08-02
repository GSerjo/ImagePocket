using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Domain;
using System.Collections.Generic;
using MonoTouch.CoreFoundation;
using System.Linq;
using Core;

namespace Dojo
{
	public partial class TagSelectorViewController : UIViewController
	{
		public event EventHandler<EventArgs> Closed = delegate { };
		public event EventHandler<EventArgsOf<List<ImageEntity>>> Done = delegate { };
		private static TagRepository _tagRepository = TagRepository.Instance;
		private List<ImageEntity> _images = new List<ImageEntity>();

		public TagSelectorViewController (ImageEntity image) : base ("TagSelectorViewController", null)
		{
			_images.Add (image.CloneDeep());
		}

		public TagSelectorViewController (List<ImageEntity> images) : base ("TagSelectorViewController", null)
		{
			_images.AddRange (images.Select (x => x.CloneDeep ()));
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			allTags.Source = new TableSource (this);
			btCancel.Clicked += OnCancel;
			btDone.Clicked += OnDone;
			UpdateTagText ();
		}

		private void UpdateTagText()
		{
			List<int> tagIds = _images.SelectMany (x => x.Tags).ToList();
			List<TagEntity> tags = _tagRepository.GetById (tagIds);
			currentTags.Text = string.Join(" ", tags.Select (x => x.Name));
		}

		private void OnCancel(object sender, EventArgs ea)
		{
			Closed (null, EventArgs.Empty);
			DismissViewController (true, null);
		}

		private void OnDone(object sender, EventArgs ea)
		{
			var eventArgs = new EventArgsOf<List<ImageEntity>>
			{
				Data = _images
			};
			Done (null, eventArgs);
			DismissViewController (true, null);
		}

		private void ReloadData()
		{
			DispatchQueue.MainQueue.DispatchAsync (() => allTags.ReloadData());
		}

		private sealed class TableSource : UITableViewSource
		{
			private string cellIdentifier = "TableCell";
			private List<TagEntity> _tags;
			private TagSelectorViewController _controller;

			public TableSource (TagSelectorViewController controller)
			{
				_controller = controller;
				_tags = _tagRepository.GetAll().Where(x=>!x.IsAll && !x.IsUntagged).ToList();
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

			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
			{
				TagEntity tag = _tags[indexPath.Item];
				_tags.Remove (tag);
				_controller.ReloadData ();
				_controller._images.Iter (x => x.AddTag (tag));
				_controller.UpdateTagText ();
			}
		}
	}
}