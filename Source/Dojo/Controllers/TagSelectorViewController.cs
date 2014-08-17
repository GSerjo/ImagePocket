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
		private TableSource _tableSource;

		public TagSelectorViewController (ImageEntity image) : base ("TagSelectorViewController", null)
		{
			_images.Add (image.CloneDeep());
			_tableSource = new TableSource (this);
		}

		public TagSelectorViewController (List<ImageEntity> images) : base ("TagSelectorViewController", null)
		{
			_images.AddRange (images.Select (x => x.CloneDeep ()));
			_tableSource = new TableSource (this);
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			tokenField.TokenDelegate = this;
			tokenField.TokenDataSource = this;
			tokenField.SetupInit ();

			tokenField.PlaceholderText = "Enter Tag";
			tokenField.ColorScheme = new UIColor (61 / 255.0f, 149 / 255.0f, 206 / 255.0f, 1.0f);
			tokenField.BecomeFirstResponder ();

			allTags.Source = _tableSource;
			btCancel.Clicked += OnCancel;
			btDone.Clicked += OnDone;
			UpdateTagText ();
		}

		#region ITokenDataSource implementation

		public int NumberOfTokensInTokenField (VENTokenField tokenField)
		{
			return _tableSource.TagCount;
		}
		public string TokenField (VENTokenField tokenField, int index)
		{
			return _tableSource.GetTag (index).Name;
		}
		public string TokenFieldCollapsedText (VENTokenField tokenField)
		{
			return string.Format ("Tags count: {0}", _tableSource.TagCount);
		}

		#endregion

		#region ITokenDelegate implementation

		public void DidDeleteTokenAtIndex (VENTokenField tokenField, int index)
		{
			_tableSource.RemoveTag (index);
			tokenField.ReloadData ();
		}

		public void DidEnterText (VENTokenField tokenField, string text)
		{
			Console.WriteLine (text);
			tokenField.ReloadData ();
		}

		#endregion

		private void UpdateTagText()
		{
			List<TagEntity> entities = GetCommonTags ();
			currentTags.Text = string.Join(" ", entities.Select (x => x.Name));
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

			public int TagCount { get { return _tags.Count; } }

			public TagEntity GetTag(int index)
			{
				return _tags [index];
			}

			public void RemoveTag(int index)
			{
				TagEntity tag = _tags[index];
				ReloadTags (tag);
			}

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
				ReloadTags (tag);
			}

			private void ReloadTags(TagEntity tag)
			{
				_tags.Remove (tag);
				_controller.ReloadData ();
				_controller._images.Iter (x => x.AddTag (tag));
				_controller.UpdateTagText ();
			}
		}
	}
}