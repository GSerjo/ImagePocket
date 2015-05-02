using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Domain;
using MonoTouch.CoreFoundation;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using NSTokenView;

namespace Dojo
{
    public partial class TagSelectorViewController : UIViewController
    {
        private static readonly TagCache _tagRepository = TagCache.Instance;
        private readonly List<ImageEntity> _images = new List<ImageEntity>();
        private TagTableSource _tagTableSource;
        private TagTokenDelegate _tagTokenDelegate;
        private TagTokenSource _tagTokenSource;

        public TagSelectorViewController(ImageEntity image) : base("TagSelectorViewController", null)
        {
            _images.Add(image.CloneDeep());
        }

        public TagSelectorViewController(List<ImageEntity> images) : base("TagSelectorViewController", null)
        {
            _images.AddRange(images.Select(x => x.CloneDeep()));
        }

        public event EventHandler<EventArgs> Cancel = delegate { };
        public event EventHandler<EventArgsOf<List<ImageEntity>>> Done = delegate { };

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Initialise();

            tagTokenView.TokenDelegate = _tagTokenDelegate;
            tagTokenView.TokenDataSource = _tagTokenSource;
            tagTokenView.SetupInit();
            tagTokenView.Layer.CornerRadius = 5;
            tagTokenView.Layer.BorderColor = UIColor.LightTextColor.CGColor;
            tagTokenView.Layer.BorderWidth = 1;

            tagTokenView.PlaceholderText = "Selet or Enter Tag";
            tagTokenView.ColorScheme = new UIColor(61 / 255.0f, 149 / 255.0f, 206 / 255.0f, 1.0f);

            allTags.Source = _tagTableSource;
            btCancel.Clicked += OnCancel;
            btDone.Clicked += OnDone;
        }

        private void AddTagToImages(TagEntity tag)
        {
            _images.Iter(x => x.AddTag(tag));
        }

        private List<TagEntity> GetCommonTags()
        {
            List<TagEntity> result = _images.First().Tags;
            IEnumerable<List<TagEntity>> imageTags = _images.Select(x => x.Tags);
            var comparer = new FuncComparer<TagEntity>((x, y) => x.EntityId == y.EntityId);

            foreach (List<TagEntity> tags in imageTags)
            {
                result = result.Intersect(tags, comparer).ToList();
            }
            return result;
        }

        private void Initialise()
        {
            _tagTableSource = new TagTableSource(this);
            _tagTokenDelegate = new TagTokenDelegate(this);
            _tagTokenSource = new TagTokenSource(this);
        }

        private void OnCancel(object sender, EventArgs ea)
        {
            Cancel(null, EventArgs.Empty);
            DismissViewController(true, null);
        }

        private void OnDone(object sender, EventArgs ea)
        {
            var eventArgs = new EventArgsOf<List<ImageEntity>>
            {
                Data = _images
            };
            Done(null, eventArgs);
            DismissViewController(true, null);
        }

        private void ReloadData()
        {
            DispatchQueue.MainQueue.DispatchAsync(() => allTags.ReloadData());
        }

        private void RemoveTagFormImages(TagEntity tag)
        {
            _images.Iter(x => x.RemoveTag(tag));
        }


        private sealed class TagTableSource : UITableViewSource
        {
            private readonly TagSelectorViewController _controller;
            private readonly string cellIdentifier = "TableCell";
            private List<TagEntity> _tags;

            public TagTableSource(TagSelectorViewController controller)
            {
                _controller = controller;
                _tags = GetTags();
            }

            public void Filter(string text)
            {
                if (string.IsNullOrWhiteSpace(text))
                {
                    _tags = GetTags();
                }
                else
                {
                    _tags = GetTags()
                        .Where(x => x.Name.Contains(text))
                        .ToList();
                    if (_tags.IsEmpty())
                    {
                        TagEntity addTagRequest = TagEntity.AddTagRequest;
                        addTagRequest.Name = text;
                        _tags.Insert(_tags.Count, addTagRequest);
                    }
                }
                ReloadTags();
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                UITableViewCell cell = tableView.DequeueReusableCell(cellIdentifier);
                if (cell == null)
                {
                    cell = new UITableViewCell(UITableViewCellStyle.Default, cellIdentifier);
                }
                TagEntity tag = _tags[indexPath.Row];
                if (tag.IsAddTagRequest)
                {
                    cell.TextLabel.Text = string.Format("Add new tag \"{0}\"", tag.Name);
                    cell.TextLabel.TextColor = UIColor.DarkTextColor;
                }
                else
                {
                    cell.TextLabel.Text = tag.Name;
                }
                return cell;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                TagEntity addTag;
                TagEntity selectedTag = _tags[indexPath.Item];
                _tags.Remove(selectedTag);
                if (selectedTag.IsAddTagRequest)
                {
                    _tags = GetTags();
                    addTag = new TagEntity { Name = selectedTag.Name };
                }
                else
                {
                    addTag = selectedTag;
                }
                _controller.AddTagToImages(addTag);
                _controller._tagTokenSource.AddTag(addTag);
                ReloadTags();
            }

            public override int RowsInSection(UITableView tableview, int section)
            {
                return _tags.Count;
            }

            private List<TagEntity> GetTags()
            {
                var commparer = new FuncComparer<TagEntity>((x, y) => x.Equals(y));
                List<TagEntity> commonTags = _controller.GetCommonTags();
                return _tagRepository.GetUserTags()
                                     .Except(commonTags, commparer)
                                     .ToList();
            }

            private void ReloadTags()
            {
                _controller.ReloadData();
            }
        }


        private sealed class TagTokenDelegate : TokenViewDelegate
        {
            private readonly TagSelectorViewController _controller;

            public TagTokenDelegate(TagSelectorViewController controller)
            {
                _controller = controller;
            }

            public override void DidDeleteTokenAtIndex(TokenView tokenView, int index)
            {
                _controller._tagTokenSource.RemoveTagAtIndex(index);
                tokenView.ReloadData();
            }

            public override void FilterToken(TokenView tokenView, string text)
            {
                _controller._tagTableSource.Filter(text);
            }
        }


        private sealed class TagTokenSource : TokenViewSource
        {
            private readonly TagSelectorViewController _controller;
            private readonly List<TagEntity> _source;

            public TagTokenSource(TagSelectorViewController controller)
            {
                _controller = controller;
                _source = controller.GetCommonTags()
                                    .OrderBy(x => x.Name).ToList();
            }

            public void AddTag(TagEntity tag)
            {
                _source.Add(tag);
                _controller.tagTokenView.ReloadData();
            }

            public override string GetToken(TokenView tokenView, int index)
            {
                return _source[index].Name;
            }

            public override int NumberOfTokens(TokenView tokenView)
            {
                return _source.Count;
            }

            public void RemoveTagAtIndex(int index)
            {
                TagEntity tag = _source[index];
                _controller.RemoveTagFormImages(tag);
                _source.RemoveAt(index);
            }
        }
    }
}
