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
		private const float DefaultVerticalInset = 7.0;
		private const float DefaultHorizontalInset = 15.0;
		private const float DefaultToLabelPadding = 5.0;
		private const float DefaultTokenPadding = 2.0;
		private const float DefaultMinImputWidth = 80.0;
		private const float DefaultMaxHeight = 150.0;
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
				var thheDelegate = Delegate;
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

		private void UpdateInputTextField()
		{
			_inputTextField.Placeholder = _tokens.Count == 0 ? string.Empty : _placeholder;
		}

		private float heightForToken()
		{
			return 30.0;
		}

		private void FocusInputTextField()
		{
			PointF contentOffset = _scrollView.ContentOffset;
			float targetY = _inputTextField.Frame.Y + heightForToken () - _maxHeight;
			if (targetY > contentOffset.Y)
			{
				_scrollView.SetContentOffset (new PointF (), false);
			}
		}

		private void LayoutToLabelInView(UIView view, PointF originX, PointF currentX)
		{
			_toLabel.RemoveFromSuperview ();
			_toLabel = ToLabel ();
		}
	}
}