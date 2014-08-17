﻿using System;
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

			tokenField.TokenDelegate = new TagTokenDelegate(_tableSource);
			tokenField.TokenDataSource = new TagTokenDataSource (_tableSource);
			tokenField.SetupInit ();

			tokenField.PlaceholderText = "Enter Tag";
			tokenField.ColorScheme = new UIColor (61 / 255.0f, 149 / 255.0f, 206 / 255.0f, 1.0f);
			tokenField.BecomeFirstResponder ();

			allTags.Source = _tableSource;
			btCancel.Clicked += OnCancel;
			btDone.Clicked += OnDone;
			UpdateTagText ();
		}

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
			private List<TagEntity> _initialTags;
			private TagSelectorViewController _controller;

			public int TagCount { get { return _tags.Count; } }

			public TableSource (TagSelectorViewController controller)
			{
				_controller = controller;
				var commonTags = _controller.GetCommonTags();
				_initialTags = _tagRepository.GetAll()
					.Where(x=>!x.IsAll && !x.IsUntagged)
					.Except(commonTags, new FuncComparer<TagEntity>((x,y)=>x.EntityId == y.EntityId))
					.ToList();
				_tags = _initialTags;
			}

			public TagEntity GetTag(int index)
			{
				return _tags [index];
			}

			public void Filter (string text)
			{
				if (string.IsNullOrWhiteSpace (text)) 
				{
					_tags = _initialTags;
				}
				else
				{
					_tags = _initialTags.Where (x => x.Name.Contains (text)).ToList ();
				}
				ReloadTags ();
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
				_controller._images.Iter (x => x.AddTag (tag));
				_tags.Remove (tag);
				ReloadTags ();
			}

			private void ReloadTags()
			{
				_controller.ReloadData ();
				_controller.UpdateTagText ();
			}
		}

		private sealed class TagTokenDelegate : TokenDelegate
		{
			private readonly TableSource _source;

			public TagTokenDelegate (TableSource source)
			{
				_source = source;
			}

			public override void DidDeleteTokenAtIndex (VENTokenField tokenField, int index)
			{
				Console.WriteLine ("DidDeleteTokenAtIndex");
			}

			public override void FilterToken (VENTokenField tokenField, string text)
			{
				_source.Filter (text);
				Console.WriteLine ("FilterToken: {0}", text);
			}

			public override void AddToken (VENTokenField tokenField, string text)
			{
				Console.WriteLine ("AddToken");
			}
		}

		private sealed class TagTokenDataSource : TokenDataSource
		{
			private TableSource _source;

			public TagTokenDataSource (TableSource source)
			{
				_source = source;
			}

			public override string GetToken (VENTokenField tokenField, int index)
			{
				return _source.GetTag (index).Name;
			}

			public override int NumberOfTokens (VENTokenField tokenField)
			{
				return _source.TagCount;
			}
		}
	}
}