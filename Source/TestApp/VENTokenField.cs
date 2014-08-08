using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace TestApp
{
	public class VENTokenField : UIView
	{
		private UIScrollView _scrollView;
		private List<VENToken> _tokens = new List<VENToken> ();
		private float _orifinalHeight;
		private UITapGestureRecognizer _tapGestureRecognizer;
		private const float DefaultVerticalInset = 7.0f;
		private const float DefaultHorizontalInset = 15.0f;
		private const float DefaultToLabelPadding = 5.0f;
		private const float DefaultTokenPadding = 2.0f;
		private const float DefaultMinImputWidth = 80.0f;
		private const float DefaultMaxHeight = 150.0f;
		private VENBackspaceTextField _invisibleTextField;
		private VENBackspaceTextField _inputTextField;
		private UIColor _colorScheme;
		private UILabel _collapsedLabel;
		private float _maxHeight;
		private float _verticalInset;
		private float _horizontalInset;
		private float _tokenPadding;
		private float _minInputWidth;
		private UIKeyboardType _inputTextFieldKeyboardType;
		private UIColor _toLabelTextColor;
		private UIColor _inputTextFieldTextColor;
		private UILabel _toLabel;
		private string _placeholder;

		private void SetupInit()
		{
			_maxHeight = DefaultMaxHeight;
			_verticalInset = DefaultVerticalInset;
			_horizontalInset = DefaultHorizontalInset;
			_tokenPadding = DefaultTokenPadding;
			_minInputWidth = DefaultMinImputWidth;
			_colorScheme = UIColor.Blue;
			_toLabelTextColor = new UIColor (112 / 255.0f, 124 / 255.0f, 124 / 255.0f, 1.0f);
			_orifinalHeight = Frame.Height;
			LayoutInvisibleTextField ();
			LayoutScrollView ();
			ReloadData ();
		}

		private void Collapse()
		{
			var removeFromSuperView = _collapsedLabel;
			_scrollView.Hidden = true;
//			setHeight;
			float currentX = 0;
		}

		private void ReloadData()
		{
			bool inputFieldShouldBecomeFirstResponder = _inputTextField.IsFirstResponder;
			_collapsedLabel.RemoveFromSuperview ();
//			_scrollView.Subviews.make
			_scrollView.Hidden = false;
			_tokens = new List<VENToken> ();
			float currentX = 0;
			float currentY = 0;
			LayoutToLabelInView (_scrollView, new PointF (), currentX);
			LayoutTokenWithCurrentX ();
			LayoutInputTextFieldWithCurrentX (currentX, currentY);
		}

		public override bool BecomeFirstResponder ()
		{
			ReloadData ();

			return base.BecomeFirstResponder ();
		}

		public override bool ResignFirstResponder ()
		{
			return _inputTextField.ResignFirstResponder ();
		}

		private void InputTextFieldBecomeFirstResponder()
		{
			if (_inputTextField.IsFirstResponder)
			{
				return;
			}
			_inputTextField.BecomeFirstResponder();
			if (RespondsToSelector (new MonoTouch.ObjCRuntime.Selector ())) 
			{
			}
		}

		private void HandleSingleTap(UITapGestureRecognizer gestureRecognizer)
		{
			BecomeFirstResponder ();
		}

		private void DidTapToken(VENToken token)
		{
			foreach (var item in _tokens)
			{
				if (item == token)
				{
					item.Highlighted = !item.Highlighted;
				} 
				else
				{
					item.Highlighted = false;
				}
			}
			SetCursorVisibility ();
		}

		private void unhighlightAllTokens ()
		{
			foreach (var token in _tokens)
			{
				token.Highlighted = false;
			}
			SetCursorVisibility ();
		}

		private void SetCursorVisibility ()
		{
			var highlightedTokens = _tokens.Where (x => x.Highlighted).ToList ();
			if (highlightedTokens.Count == 0)
			{
				InputTextFieldBecomeFirstResponder ();
			}
			else
			{
				_invisibleTextField.BecomeFirstResponder ();
			}
		}

		private void TextFieldDidBeginEditing(UITextField textField)
		{
			if (textField == _inputTextField)
			{
				unhighlightAllTokens ();
			}
		}

		private void SetColorScheme(UIColor color)
		{
			_colorScheme = color;
			_collapsedLabel.TextColor = color;
			_inputTextField.TintColor = color;
			foreach (var token in _tokens)
			{
				token.SetColorSheme (color);
			}
		}

		private string InputText()
		{
			return _inputTextField.Text;
		}

		private void LayoutScrollView()
		{
			_scrollView = new UIScrollView (new RectangleF (0, 0, Frame.Width, Frame.Height));
			_scrollView.ContentSize = new SizeF (Frame.Width - _horizontalInset * 2, Frame.Height - _verticalInset * 2);
			_scrollView.ContentInset = new UIEdgeInsets(_verticalInset, _horizontalInset, _verticalInset, _horizontalInset);
			_scrollView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;
			AddSubview (_scrollView);
		}

		private void LayoutInputTextFieldWithCurrentX(float currentX, float currentY)
		{
			float inputTextFieldWidth = _scrollView.ContentSize.Width - currentX;
			if (inputTextFieldWidth < _minInputWidth)
			{
				inputTextFieldWidth = _scrollView.ContentSize.Width;
				currentY += HeightForToken ();
				currentX = 0;
			}
			var inputTextField = _inputTextField;
			inputTextField.Text = "";
			inputTextField.Frame = new RectangleF (currentX, currentY + 1, inputTextFieldWidth, HeightForToken () - 1);
			inputTextField.TintColor = _colorScheme;
			_scrollView.AddSubview (inputTextField);
		}

		private void UpdateInputTextField()
		{
			_inputTextField.Placeholder = _tokens.Count == 0 ? string.Empty : _placeholder;
		}

		private float HeightForToken()
		{
			return 30.0f;
		}

		private void LayoutInvisibleTextField()
		{
			_invisibleTextField = new VENBackspaceTextField ();
			//Add Delegate
			AddSubview (_invisibleTextField);
		}

		private void FocusInputTextField()
		{
			PointF contentOffset = _scrollView.ContentOffset;
			float targetY = _inputTextField.Frame.Y + HeightForToken () - _maxHeight;
			if (targetY > contentOffset.Y)
			{
				_scrollView.SetContentOffset (new PointF (), false);
			}
		}

		private void LayoutToLabelInView(UIView view, PointF originX, float currentX)
		{
			_toLabel.RemoveFromSuperview ();
			_toLabel = ToLabel ();
		}

		private UILabel ToLabel()
		{
			if (_toLabel == null)
			{
				_toLabel = new UILabel ();
				_toLabel.TextColor = _toLabelTextColor;
				_toLabel.Text = "To:";
				_toLabel.SizeToFit ();
			}
			return _toLabel;
		}

	}
}