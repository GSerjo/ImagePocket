using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace TestApp
{
	public class VENTokenField
	{
		private UIScrollView _scrollView;
		private NSMutableArray _tokens;
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
		private UILabel collapsedLabel;
		private float _maxHeight;
		private float _verticalInset;
		private float _horizontalInset;
		private float _tokenPadding;
		private float _minInputWidth;
		private UIKeyboardType _inputTextFieldKeyboardTYpe;
	}
}

