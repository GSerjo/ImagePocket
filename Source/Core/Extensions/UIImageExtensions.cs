using System;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using System.Drawing;

namespace Core
{
	public static class UIImageExtensions
	{
		public static UIImage ScaleAndRotateImage(this UIImage imageIn, UIImageOrientation orIn) {
			int kMaxResolution = 2048;

			CGImage imgRef = imageIn.CGImage;
			float width = imgRef.Width;
			float height = imgRef.Height;
			CGAffineTransform transform = CGAffineTransform.MakeIdentity ();
			RectangleF bounds = new RectangleF( 0, 0, width, height );

			if ( width > kMaxResolution || height > kMaxResolution )
			{
				float ratio = width/height;

				if (ratio > 1)
				{
					bounds.Width  = kMaxResolution;
					bounds.Height = bounds.Width / ratio;
				}
				else
				{
					bounds.Height = kMaxResolution;
					bounds.Width  = bounds.Height * ratio;
				}
			}

			float scaleRatio = bounds.Width / width;
			SizeF imageSize = new SizeF( width, height);
			UIImageOrientation orient = orIn;
			float boundHeight;

			switch(orient)
			{
			case UIImageOrientation.Up:                                        //EXIF = 1
				transform = CGAffineTransform.MakeIdentity();
				break;

			case UIImageOrientation.UpMirrored:                                //EXIF = 2
				transform = CGAffineTransform.MakeTranslation (imageSize.Width, 0f);
				transform = CGAffineTransform.MakeScale(-1.0f, 1.0f);
				break;

			case UIImageOrientation.Down:                                      //EXIF = 3
				transform = CGAffineTransform.MakeTranslation (imageSize.Width, imageSize.Height);
				transform = CGAffineTransform.Rotate(transform, (float)Math.PI);
				break;

			case UIImageOrientation.DownMirrored:                              //EXIF = 4
				transform = CGAffineTransform.MakeTranslation (0f, imageSize.Height);
				transform = CGAffineTransform.MakeScale(1.0f, -1.0f);
				break;

			case UIImageOrientation.LeftMirrored:                              //EXIF = 5
				boundHeight = bounds.Height;
				bounds.Height = bounds.Width;
				bounds.Width = boundHeight;
				transform = CGAffineTransform.MakeTranslation (imageSize.Height, imageSize.Width);
				transform = CGAffineTransform.MakeScale(-1.0f, 1.0f);
				transform = CGAffineTransform.Rotate(transform, 3.0f * (float)Math.PI/ 2.0f);
				break;

			case UIImageOrientation.Left:                                      //EXIF = 6
				boundHeight = bounds.Height;
				bounds.Height = bounds.Width;
				bounds.Width = boundHeight;
				transform = CGAffineTransform.MakeTranslation (0.0f, imageSize.Width);
				transform = CGAffineTransform.Rotate(transform, 3.0f * (float)Math.PI / 2.0f);
				break;

			case UIImageOrientation.RightMirrored:                             //EXIF = 7
				boundHeight = bounds.Height;
				bounds.Height = bounds.Width;
				bounds.Width = boundHeight;
				transform = CGAffineTransform.MakeScale(-1.0f, 1.0f);
				transform = CGAffineTransform.Rotate(transform, (float)Math.PI / 2.0f);
				break;

			case UIImageOrientation.Right:                                     //EXIF = 8
				boundHeight = bounds.Height;
				bounds.Height = bounds.Width;
				bounds.Width = boundHeight;
				transform = CGAffineTransform.MakeTranslation(imageSize.Height, 0.0f);
				transform = CGAffineTransform.Rotate(transform, (float)Math.PI  / 2.0f);
				break;

			default:
				throw new Exception("Invalid image orientation");
			}

			UIGraphics.BeginImageContext(bounds.Size);

			CGContext context = UIGraphics.GetCurrentContext ();

			if ( orient == UIImageOrientation.Right || orient == UIImageOrientation.Left )
			{
				context.ScaleCTM(-scaleRatio, scaleRatio);
				context.TranslateCTM(-height, 0);
			}
			else
			{
				context.ScaleCTM(scaleRatio, -scaleRatio);
				context.TranslateCTM(0, -height);
			}

			context.ConcatCTM(transform);
			context.DrawImage (new RectangleF (0, 0, width, height), imgRef);

			UIImage imageCopy = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();

			return imageCopy;
		}


		public static UIImage ScaleImage(this UIImage image, int maxSize)
		{

			UIImage res;

			using (CGImage imageRef = image.CGImage)
			{
				CGImageAlphaInfo alphaInfo = imageRef.AlphaInfo;
				CGColorSpace colorSpaceInfo = CGColorSpace.CreateDeviceRGB();
				if (alphaInfo == CGImageAlphaInfo.None)
				{
					alphaInfo = CGImageAlphaInfo.NoneSkipLast;
				}

				int width, height;

				width = imageRef.Width;
				height = imageRef.Height;


				if (height >= width)
				{
					width = (int)Math.Floor((double)width * ((double)maxSize / (double)height));
					height = maxSize;
				}
				else
				{
					height = (int)Math.Floor((double)height * ((double)maxSize / (double)width));
					width = maxSize;
				}


				CGBitmapContext bitmap;

				if (image.Orientation == UIImageOrientation.Up || image.Orientation == UIImageOrientation.Down)
				{
					bitmap = new CGBitmapContext(IntPtr.Zero, width, height, imageRef.BitsPerComponent, imageRef.BytesPerRow, colorSpaceInfo, alphaInfo);
				}
				else
				{
					bitmap = new CGBitmapContext(IntPtr.Zero, height, width, imageRef.BitsPerComponent, imageRef.BytesPerRow, colorSpaceInfo, alphaInfo);
				}

				switch (image.Orientation)
				{
				case UIImageOrientation.Left:
					bitmap.RotateCTM((float)Math.PI / 2);
					bitmap.TranslateCTM(0, -height);
					break;
				case UIImageOrientation.Right:
					bitmap.RotateCTM(-((float)Math.PI / 2));
					bitmap.TranslateCTM(-width, 0);
					break;
				case UIImageOrientation.Up:
					break;
				case UIImageOrientation.Down:
					bitmap.TranslateCTM(width, height);
					bitmap.RotateCTM(-(float)Math.PI);
					break;
				}

				bitmap.DrawImage(new Rectangle(0, 0, width, height), imageRef);


				res = UIImage.FromImage(bitmap.ToImage());
				bitmap = null;

			}
			return res;
		}
	}
}