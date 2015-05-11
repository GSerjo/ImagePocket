﻿using System;
using System.Drawing;
using MonoTouch.Dialog;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Dojo
{
    public sealed class MenuElement : Element
    {
        private static readonly NSString _cellKey = new NSString("MenuElement");
        private readonly UIImage _image;
        private readonly Action _tapped;
        private readonly string _title;

        public MenuElement(string title, UIImage image, Action tapped)
            : base(string.Empty)
        {
            _title = title;
            _image = image;
            _tapped = tapped;
        }

        protected override NSString CellKey
        {
            get { return _cellKey; }
        }

        public override UITableViewCell GetCell(UITableView tv)
        {
            var cell = tv.DequeueReusableCell(_cellKey) as ElementCell;
            if (cell == null)
            {
                cell = new ElementCell(_title, _image);
            }
            else
            {
                cell.Update(_title, _image);
            }
            return cell;
        }

        public override void Selected(DialogViewController dvc, UITableView tableView, NSIndexPath path)
        {
            if (_tapped != null)
            {
                _tapped();
            }
            base.Selected(dvc, tableView, path);
        }


        private sealed class ElementCell : UITableViewCell
        {
//            private readonly UIImageView _imageView;
//            private readonly UILabel _label;
            private const float ImageSize = 24f;

            public ElementCell(string title, UIImage image) : base(UITableViewCellStyle.Default, _cellKey)
            {
//                _label = new UILabel
//                {
//                    TextAlignment = UITextAlignment.Left,
//                };
//                _imageView = new UIImageView();

                Update(title, image);
//                ContentView.Add(_imageView);
//                ContentView.Add(_label);
            }

            public override void LayoutSubviews()
            {
                base.LayoutSubviews();
                ImageView.Frame = new RectangleF(0, 0, ImageSize, ImageSize);

                var frame = TextLabel.Frame;
                frame.X = ImageSize * 2;
                frame.Width += (TextLabel.Frame.X - frame.X);
                TextLabel.Frame = frame;

//                _label.Frame = new RectangleF(45, 7, 200, 24);
            }

            public void Update(string title, UIImage image)
            {
                TextLabel.Text = title;
                ImageView.Image = image;
            }
        }
    }
}
