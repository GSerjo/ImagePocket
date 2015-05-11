using System;
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
        private readonly Action _tappedAction;
		private readonly Func<UIViewController> _tappedFunc;
        private readonly string _title;

        public MenuElement(string title, UIImage image, Action tapped)
            : base(string.Empty)
        {
            _title = title;
            _image = image;
			_tappedAction = tapped;
        }

		public MenuElement(string title, UIImage image, Func<UIViewController> tapped)
			: base(string.Empty)
		{
			_title = title;
			_image = image;
			_tappedFunc = tapped;
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
            if (_tappedAction != null)
            {
                _tappedAction();
            }
			if (_tappedFunc != null)
			{
				var controller = _tappedFunc ();
				dvc.ActivateController (controller);
			}
        }


        private sealed class ElementCell : UITableViewCell
        {
            private const float ImageSize = 24f;

            public ElementCell(string title, UIImage image) : base(UITableViewCellStyle.Default, _cellKey)
            {
                Update(title, image);
            }

            public override void LayoutSubviews()
            {
                ImageView.Frame = new RectangleF(12, 12, ImageSize, ImageSize);
				TextLabel.Frame = new RectangleF(48, 12, 200, 24);
            }

            public void Update(string title, UIImage image)
            {
                TextLabel.Text = title;
                ImageView.Image = image;
            }
        }
    }
}
