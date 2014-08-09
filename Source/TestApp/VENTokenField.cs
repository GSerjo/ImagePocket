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
		private string _placeholderText;

		public override bool BecomeFirstResponder ()
		{
			ReloadData ();

			InputTextFieldBecomeFirstResponder ();
			return true;
		}

		public override bool ResignFirstResponder ()
		{
			return _inputTextField.ResignFirstResponder ();
		}

		private void SetupInit()
		{
			_maxHeight = DefaultMaxHeight;
			_verticalInset = DefaultVerticalInset;
			_horizontalInset = DefaultHorizontalInset;
			_tokenPadding = DefaultTokenPadding;
			_minInputWidth = DefaultMinImputWidth;
			_colorScheme = UIColor.Blue;
			_toLabelTextColor = new UIColor (112 / 255.0f, 124 / 255.0f, 124 / 255.0f, 1.0f);
			_inputTextFieldTextColor = new UIColor (38 / 255.0f, 39 / 255.0f, 41 / 255.0f, 1.0f);
			_orifinalHeight = Frame.Height;

			LayoutInvisibleTextField ();
			LayoutScrollView ();

			ReloadData ();
		}

		private void Collapse()
		{
			_collapsedLabel.RemoveFromSuperview();
			_scrollView.Hidden = true;
			Frame = new RectangleF (Frame.X, Frame.Y, Frame.Width, _orifinalHeight);
			float currentX = 0;
			LayoutToLabelInView (this, new PointF (_horizontalInset, _verticalInset), ref currentX);
			LayoutCollapsedLabelWithCurrentX (ref currentX);
			_tapGestureRecognizer = new UITapGestureRecognizer (HandleSingleTap);
			AddGestureRecognizer (_tapGestureRecognizer);
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
			LayoutToLabelInView (_scrollView, new PointF (), ref currentX);
			LayoutTokensWithCurrentX (ref currentX, ref currentY);
			LayoutInputTextFieldWithCurrentX (ref currentX, ref currentY);

			AdjustHeightForCurrentY (currentY);
			_scrollView.ContentSize = new SizeF (_scrollView.ContentSize.Width, currentY + HeightForToken ());
			UpdateInputTextField ();

			if (inputFieldShouldBecomeFirstResponder)
			{
				InputTextFieldBecomeFirstResponder ();
			}
			else
			{
				FocusInputTextField ();
			}
		}

		private void SetPlaceholderText(string placeholderText)
		{
			_placeholderText = placeholderText;
			_inputTextField.Placeholder = _placeholderText;
		}

		private void SetInputTextFieldTextColor(UIColor inputTextFieldTextColor)
		{
			_inputTextFieldTextColor = inputTextFieldTextColor;
			_inputTextField.TextColor = _inputTextFieldTextColor;
		}

		private void SetToLabelTextColor(UIColor toLabelTextColor)
		{
			_toLabelTextColor = toLabelTextColor;
			_toLabel.TextColor = _toLabelTextColor;
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

		private void LayoutInputTextFieldWithCurrentX(ref float currentX, ref float currentY)
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

		private void LayoutCollapsedLabelWithCurrentX(ref float currentX)
		{
			var label = new UILabel(new RectangleF(currentX, _toLabel.Frame.Y, Frame.Width - currentX - _horizontalInset, _toLabel.Frame.Height));
//			label.Font = new UIFont()
			label.Text = CollapsedText ();
			label.TextColor = _colorScheme;
			label.MinimumScaleFactor = 5 / label.Font.PointSize;
			label.AdjustsFontSizeToFitWidth = true;
			AddSubview (label);
			_collapsedLabel = label;
		}

		private void LayoutToLabelInView(UIView view, PointF originX, ref float currentX)
		{
			_toLabel.RemoveFromSuperview ();
			_toLabel = ToLabel ();
			//Origin
			view.AddSubview (_toLabel);
			currentX += _toLabel.Hidden ? _toLabel.Frame.X : _toLabel.Frame.X + DefaultToLabelPadding;
		}

		private void LayoutTokensWithCurrentX(ref float currentX, ref float currentY)
		{
			for (int i = 0; i < NumberOfTokens(); i++)
			{
				var title = TitleForTokenAtIndex (i);
				var token = new VENToken ();
				token.ColorScheme = _colorScheme;
				token.SetTitleText (title);
				_tokens.Add (token);
				if (currentX + token.Frame.Width <= _scrollView.Frame.Width)
				{
					token.Frame = new RectangleF (currentX, currentY, token.Frame.Width, token.Frame.Height);
				}
				else
				{
					currentY += token.Frame.Height;
					currentX = 0;
					float tokenWidth = token.Frame.Width;
					if (tokenWidth > _scrollView.ContentSize.Width)
					{
						tokenWidth = _scrollView.ContentSize.Width;
					}
					token.Frame = new RectangleF (currentX, currentY, tokenWidth, token.Frame.Height);
				}currentX += token.Frame.Width + _tokenPadding;
				_scrollView.AddSubview (token);

			}
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

		private void InputTextFieldBecomeFirstResponder()
		{
			if (_inputTextField.IsFirstResponder)
			{
				return;
			}
			_inputTextField.BecomeFirstResponder();
//			if (RespondsToSelector (new MonoTouch.ObjCRuntime.Selector ())) 
//			{
//			}
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

		private void AdjustHeightForCurrentY(float currentY)
		{
			float height;
			if (currentY + HeightForToken () > Frame.Height)
			{
				if (currentY + HeightForToken () <= _maxHeight) 
				{
					height = currentY + HeightForToken () + _verticalInset * 2;
				}
				else
				{
					height = _maxHeight;
				}
			}
			else
			{
				if (currentY + HeightForToken () > _orifinalHeight) 
				{
					height = currentY + HeightForToken () + _verticalInset * 2;
				} 
				else 
				{
					height = _orifinalHeight;
				}
			}
			Frame = new RectangleF (Frame.X, Frame.Y, Frame.Width, height);
		}

		private VENBackspaceTextField InputTextField()
		{
			if (_inputTextField == null) 
			{
				_inputTextField = new VENBackspaceTextField ();
				_inputTextField.KeyboardType = _inputTextFieldKeyboardType;
				_inputTextField.TextColor = _inputTextFieldTextColor;
				//Font
				_inputTextField.AccessibilityLabel = "To";
				_inputTextField.AutocorrectionType = UITextAutocorrectionType.No;
				_inputTextField.TintColor = _colorScheme;
				_inputTextField.Placeholder = _placeholderText;
				//addTarget
			}
			return _inputTextField;
		}

		private void SetInputTextFieldKeyboardType(UIKeyboardType inputTextFieldKeyboardType)
		{
			_inputTextFieldKeyboardType = inputTextFieldKeyboardType;
			_inputTextField.KeyboardType = _inputTextFieldKeyboardType;
		}

		private void InputTextFieldDidChange(UITextField textField)
		{

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

		private void UnhighlightAllTokens ()
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

		private void UpdateInputTextField()
		{
			_inputTextField.Placeholder = _tokens.Count == 0 ? string.Empty : _placeholderText;
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

		private string TitleForTokenAtIndex(int index)
		{
			return "Data";
		}

		private int NumberOfTokens()
		{
			return 0;
		}

		private string CollapsedText()
		{
			return string.Empty;
		}

		private bool TextFieldShouldReturn(UITextField textField)
		{
			return false;
		}

		private void TextFieldDidBeginEditing(UITextField textField)
		{
			if (textField == _inputTextField)
			{
				UnhighlightAllTokens ();
			}
		}

		private bool TextField(UITextField textField, NSRange range, string replacementString)
		{
			UnhighlightAllTokens ();
			return true;
		}
	}
}