﻿using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using MonoTouch.ObjCRuntime;

namespace Dojo
{
	[Register ("VENTokenField")]
	public sealed class VENTokenField : UIView
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
//		private UILabel _toLabel;
		public string PlaceholderText { get; set; }

		public ITokenDataSource DataSource { get; set; }
		public ITokenDelegate Delegate { get; set; }

		public VENTokenField (IntPtr handle) : base(handle)
		{
			SetupInit ();
		}

		[Export("initWithFrame:")]
		public VENTokenField(RectangleF frame) : base(frame)
		{
			SetupInit ();
		}

		public override bool BecomeFirstResponder ()
		{
			//ReloadData ();

			InputTextFieldBecomeFirstResponder ();
			return true;
		}

		public override bool ResignFirstResponder ()
		{
			return _inputTextField.ResignFirstResponder ();
		}

		public void SetupInit()
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
			_inputTextField = InputTextField ();
			LayoutInvisibleTextField ();
			LayoutScrollView ();

			ReloadData ();
		}

//		private void Collapse()
//		{
//			_collapsedLabel.RemoveFromSuperview();
//			_scrollView.Hidden = true;
//			Frame = new RectangleF (Frame.X, Frame.Y, Frame.Width, _orifinalHeight);
//			float currentX = 0;
//			LayoutCollapsedLabelWithCurrentX (ref currentX);
//			_tapGestureRecognizer = new UITapGestureRecognizer (HandleSingleTap);
//			AddGestureRecognizer (_tapGestureRecognizer);
//		}

		public void ReloadData()
		{

			bool inputFieldShouldBecomeFirstResponder = _inputTextField.IsFirstResponder;
			//_collapsedLabel.RemoveFromSuperview ();

			var removeSubviews = _scrollView.Subviews.ToList();
			foreach (var view in removeSubviews)
			{
				var t = view as VENToken;
				if (t != null) {
					Console.WriteLine (t.Id);
					t.RemoveFromSuperview ();
				}
			}

			_scrollView.Hidden = false;
				
			if (_tapGestureRecognizer != null)
			{
				RemoveGestureRecognizer (_tapGestureRecognizer);
			}

			_tokens.Clear ();
				
			float currentX = 0;
			float currentY = 0;

//			LayoutToLabelInView (_scrollView, new PointF (), ref currentX);
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
			InputTextFieldBecomeFirstResponder ();
		}

		private void SetPlaceholderText(string placeholderText)
		{
			PlaceholderText = placeholderText;
			_inputTextField.Placeholder = PlaceholderText;
		}

		private void SetInputTextFieldTextColor(UIColor inputTextFieldTextColor)
		{
			_inputTextFieldTextColor = inputTextFieldTextColor;
			_inputTextField.TextColor = _inputTextFieldTextColor;
		}

		private void SetToLabelTextColor(UIColor toLabelTextColor)
		{
			_toLabelTextColor = toLabelTextColor;
//			_toLabel.TextColor = _toLabelTextColor;
		}

		public void SetColorScheme(UIColor color)
		{
			_colorScheme = color;
			//_collapsedLabel.TextColor = color;
			_inputTextField.TintColor = color;
			foreach (VENToken token in _tokens)
			{
				token.ColorScheme = color;
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
			_inputTextField.Text = "";
			_inputTextField.Frame = new RectangleF (currentX, currentY + 1, inputTextFieldWidth, HeightForToken () - 1);
			_inputTextField.TintColor = _colorScheme;
			_scrollView.AddSubview (_inputTextField);
		}

		private void LayoutCollapsedLabelWithCurrentX(ref float currentX)
		{
			//var label = new UILabel(new RectangleF(currentX, _toLabel.Frame.Y, Frame.Width - currentX - _horizontalInset, _toLabel.Frame.Height));
			var label = new UILabel(new RectangleF(currentX, Frame.Y, Frame.Width - currentX - _horizontalInset, Frame.Height));
			label.Font = UIFont.FromName ("HelveticaNeue", 15.5f);
			label.Text = CollapsedText ();
			label.TextColor = _colorScheme;
			label.MinimumScaleFactor = 5 / label.Font.PointSize;
			label.AdjustsFontSizeToFitWidth = true;
			AddSubview (label);
			_collapsedLabel = label;
		}
//
//		private void LayoutToLabelInView(UIView view, PointF originX, ref float currentX)
//		{
//			_toLabel.RemoveFromSuperview ();
//			_toLabel = ToLabel ();
//			//Origin
//			view.AddSubview (_toLabel);
//			currentX += _toLabel.Hidden ? _toLabel.Frame.X : _toLabel.Frame.X + DefaultToLabelPadding;
//		}

		private void LayoutTokensWithCurrentX(ref float currentX, ref float currentY)
		{
			for (int i = 0; i < NumberOfTokens(); i++)
			{
				var title = TitleForTokenAtIndex (i);

				var nibObjects = NSBundle.MainBundle.LoadNib("VENToken", this, null);
				var token = (VENToken)Runtime.GetNSObject(nibObjects.ValueAt(0));
				token.SetupInit ();
				token.ColorScheme = _colorScheme;
				token.OnDidTapToken = DidTapToken;
				token.SetTitleText (title);
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
				}
				currentX += token.Frame.Width + _tokenPadding;
				_scrollView.AddSubview (token);
				_tokens.Add (token);
				Console.WriteLine ("Id " + token.Id);
			}
		}

		private float HeightForToken()
		{
			return 30.0f;
		}

		private void LayoutInvisibleTextField()
		{
			_invisibleTextField = new VENBackspaceTextField ();
			_invisibleTextField.Delegate = new DelegateTest();
			AddSubview (_invisibleTextField);
		}

		private void InputTextFieldBecomeFirstResponder()
		{
			if (_inputTextField.IsFirstResponder)
			{
				return;
			}
			_inputTextField.BecomeFirstResponder();
			if (Delegate != null)
			{
			}
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
				_inputTextField.Font = UIFont.FromName ("HelveticaNeue", 15.5f);
				_inputTextField.AccessibilityLabel = "Toeee";
				_inputTextField.AutocorrectionType = UITextAutocorrectionType.No;
				_inputTextField.TintColor = _colorScheme;
				_inputTextField.Placeholder = PlaceholderText;
				_inputTextField.AddTarget (InputTextFieldDidChange, UIControlEvent.EditingChanged);
			}
			return _inputTextField;
		}

		private void SetInputTextFieldKeyboardType(UIKeyboardType inputTextFieldKeyboardType)
		{
			_inputTextFieldKeyboardType = inputTextFieldKeyboardType;
			_inputTextField.KeyboardType = _inputTextFieldKeyboardType;
		}

		private void InputTextFieldDidChange(object sender, EventArgs ea)
		{

		}

		private void HandleSingleTap(UITapGestureRecognizer gestureRecognizer)
		{
			BecomeFirstResponder ();
		}

		public void DidTapToken(VENToken token)
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
			_inputTextField.Placeholder = _tokens.Count != 0 ? string.Empty : PlaceholderText;
		}

		private void FocusInputTextField()
		{
			PointF contentOffset = _scrollView.ContentOffset;
			float targetY = _inputTextField.Frame.Y + HeightForToken () - _maxHeight;
			if (targetY > contentOffset.Y)
			{
				_scrollView.SetContentOffset (new PointF(contentOffset.X, targetY), false);
			}
		}

		private string TitleForTokenAtIndex(int index)
		{
			if (DataSource != null)
			{
				return DataSource.TokenField (this, index);
			}
			return string.Empty;
		}

		private int NumberOfTokens()
		{
			if (DataSource != null)
			{
				return DataSource.NumberOfTokensInTokenField (this);
			}
			return 0;
		}

		private string CollapsedText()
		{
			if (DataSource != null)
			{
				return DataSource.TokenFieldCollapsedText (this);
			}
			return string.Empty;
		}

		private bool TextFieldShouldReturn(UITextField textField)
		{
			if (Delegate != null && textField.Text.Length > 0)
			{
				Delegate.DidEnterText (this, textField.Text);
			}
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

		private void TextFieldDidEnterBackspace(VENBackspaceTextField textField)
		{
			Console.WriteLine ("TextFieldDidEnterBackspace");
		}
	}

	public class DelegateTest : UITextFieldDelegate
	{
		public override bool ShouldChangeCharacters (UITextField textField, NSRange range, string replacementString)
		{
			Console.WriteLine ("ShouldChangeCharacters");
			return true;
		}


		 public override bool ShouldClear (UITextField textField)
		{
			Console.WriteLine ("ShouldClear");
			// NOTE: Don't call the base implementation on a Model class
			// see http://docs.xamarin.com/guides/ios/application_fundamentals/delegates,_protocols,_and_events
			return true;
		}
	}
}