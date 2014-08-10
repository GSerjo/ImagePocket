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
	public partial class TagSelectorViewController : UIViewController, ITokenDelegate, ITokenDataSource
	{
		public event EventHandler<EventArgs> Cancel = delegate { };
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

			tokenField.Delegate = this;
			tokenField.DataSource = this;
			tokenField.SetupInit ();
			tokenField.PlaceholderText = "Enter Tag";
			tokenField.SetColorScheme (new UIColor (61 / 255.0f, 149 / 255.0f, 206 / 255.0f, 1.0f));
			tokenField.BecomeFirstResponder ();

			allTags.Source = new TableSource (this);
			btCancel.Clicked += OnCancel;
			btDone.Clicked += OnDone;
			UpdateTagText ();
		}

		#region ITokenDataSource implementation

		public int NumberOfTokensInTokenField (VENTokenField tokenField)
		{
			throw new System.NotImplementedException ();
		}
		public string TokenField (VENTokenField tokenField, int index)
		{
			throw new System.NotImplementedException ();
		}
		public string TokenFieldCollapsedText (VENTokenField tokenField)
		{
			throw new System.NotImplementedException ();
		}

		#endregion

		#region ITokenDelegate implementation

		public void TokenFieldDidBeginEditing (VENTokenField tokenField)
		{
			throw new System.NotImplementedException ();
		}

		public void InputTextFIeldDidChange (VENTokenField tokenField, string didChangeText)
		{
			throw new System.NotImplementedException ();
		}

		public void TokenFieldShouldReturn (VENTokenField tokenField, string didEnterText)
		{
			throw new System.NotImplementedException ();
		}

		#endregion

		private void UpdateTagText()
		{
//			List<TagEntity> entities = GetCommonTags ();
//			currentTags.Text = string.Join(" ", entities.Select (x => x.Name));
			var t = new UILabel (new RectangleF (0, 0, 40, 40));
			t.Text = "Test";
			t.BackgroundColor = UIColor.Red;
			currentTags.AddSubviews (t);
		}

		private List<TagEntity> GetCommonTags()
		{
			var tagIds = _images.First ().Tags;
			IEnumerable<List<int>> imageTags = _images.Select (x => x.Tags);
			foreach (var tags in imageTags)
			{
				tagIds = tagIds.Intersect (tags).ToList();
			}
			return _tagRepository.GetById (tagIds);
		}

		private void OnCancel(object sender, EventArgs ea)
		{
			Cancel (null, EventArgs.Empty);
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
				var commonTags = _controller.GetCommonTags();
				_tags = _tagRepository.GetAll()
					.Where(x=>!x.IsAll && !x.IsUntagged)
					.Except(commonTags, new FuncComparer<TagEntity>((x,y)=>x.EntityId == y.EntityId))
					.ToList();
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