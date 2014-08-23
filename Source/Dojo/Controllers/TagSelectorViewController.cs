﻿using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Domain;
using System.Collections.Generic;
using MonoTouch.CoreFoundation;
using System.Linq;
using Core;
using NSTokenView;

namespace Dojo
{
	public partial class TagSelectorViewController : UIViewController
	{
		public event EventHandler<EventArgs> Cancel = delegate { };
		public event EventHandler<EventArgsOf<List<ImageEntity>>> Done = delegate { };
		private static TagRepository _tagRepository = TagRepository.Instance;
		private List<ImageEntity> _images = new List<ImageEntity>();
		private TagTableSource _tagTableSource;
		private TagTokenDelegate _tagTokenDelegate;
		private TagTokenSource _tagTokenSource;

		public TagSelectorViewController (ImageEntity image) : base ("TagSelectorViewController", null)
		{
			_images.Add (image.CloneDeep());
		}

		public TagSelectorViewController (List<ImageEntity> images) : base ("TagSelectorViewController", null)
		{
			_images.AddRange (images.Select (x => x.CloneDeep ()));
		}

		private void Initialise()
		{
			_tagTableSource = new TagTableSource (this);
			_tagTokenDelegate = new TagTokenDelegate (this);
			_tagTokenSource = new TagTokenSource (this);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			tagTokenView.TokenDelegate = _tagTokenDelegate;
			tagTokenView.TokenDataSource = _tagTokenSource;
			tagTokenView.SetupInit ();
			tagTokenView.Layer.CornerRadius = 5;
			tagTokenView.Layer.BorderColor = UIColor.LightTextColor.CGColor;
			tagTokenView.Layer.BorderWidth = 1;

			tagTokenView.PlaceholderText = "Selet or Enter Tag";
			tagTokenView.ColorScheme = new UIColor (61 / 255.0f, 149 / 255.0f, 206 / 255.0f, 1.0f);

			allTags.Source = _tagTableSource;
			btCancel.Clicked += OnCancel;
			btDone.Clicked += OnDone;
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

		private void AddTagToImages(TagEntity tag)
		{
			_images.Iter (x => x.AddTag (tag));
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

		private sealed class TagTableSource : UITableViewSource
		{
			private string cellIdentifier = "TableCell";
			private List<TagEntity> _tags;
			private TagSelectorViewController _controller;

			public int TagCount { get { return _tags.Count; } }

			public TagTableSource (TagSelectorViewController controller)
			{
				_controller = controller;
				_tags = GetTags();
			}

			public TagEntity GetTag(int index)
			{
				return _tags [index];
			}

			public void Filter (string text)
			{
				if (string.IsNullOrWhiteSpace (text)) 
				{
					_tags = GetTags();
				}
				else
				{
					_tags = GetTags().Where (x => x.Name.Contains (text)).ToList ();
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
				AddTagToImages (tag);
				_controller._tagTokenSource.AddTag (tag);
				ReloadTags ();
			}

			private List<TagEntity> GetTags()
			{
				var commparer = new FuncComparer<TagEntity> ((x, y) => x.EntityId == y.EntityId);
				var commonTags = _controller.GetCommonTags();
				return _tagRepository.GetUserTags()
					.Except(commonTags, commparer)
					.ToList();
			}

			private void AddTagToImages(TagEntity tag)
			{
				_controller.AddTagToImages(tag);
				_tags.Remove (tag);
			}

			private void ReloadTags()
			{
				_controller.ReloadData ();
			}
		}

		private sealed class TagTokenDelegate : TokenViewDelegate
		{
			private readonly TagSelectorViewController _controller;

			public TagTokenDelegate (TagSelectorViewController controller)
			{
				_controller = controller;
			}

			public override void DidDeleteTokenAtIndex (TokenView tokenView, int index)
			{
				Console.WriteLine ("DidDeleteTokenAtIndex");
			}

			public override void FilterToken (TokenView tokenView, string text)
			{
				_controller._tagTableSource.Filter (text);
				Console.WriteLine ("FilterToken: {0}", text);
			}
		}

		private sealed class TagTokenSource : TokenViewSource
		{
			private readonly TokenView _tokenView;
			private List<TagEntity> _source;

			public TagTokenSource (TagSelectorViewController controller)
			{
				_source = controller.GetCommonTags()
										.OrderBy(x=>x.Name).ToList();
				_tokenView = controller.tagTokenView;
			}

			public override string GetToken (TokenView tokenView, int index)
			{
				return _source[index].Name;
			}

			public override int NumberOfTokens (TokenView tokenView)
			{
				return _source.Count;
			}

			public void AddTag(TagEntity tag)
			{
				_source.Add (tag);
				_tokenView.ReloadData ();
			}
		}
	}
}