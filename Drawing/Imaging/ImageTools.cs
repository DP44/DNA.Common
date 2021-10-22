using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace DNA.Drawing.Imaging
{
	public static class ImageTools
	{
		public enum CropOptions
		{
			Start,
			Middle,
			End
		}

		private static ImageCodecInfo GetEncoderFormat(string formatName)
		{
			ImageCodecInfo[] imageEncoders = ImageCodecInfo.GetImageEncoders();
			foreach (ImageCodecInfo imageCodecInfo in imageEncoders)
			{
				if (string.Compare(imageCodecInfo.FormatDescription, formatName, true) == 0)
				{
					return imageCodecInfo;
				}
			}
			throw new ArgumentException("Cannot Find Codec " + formatName);
		}

		public static void SaveJpg(Bitmap bitmap, Stream outStream, Percentage quality)
		{
			ImageCodecInfo encoderFormat = ImageTools.GetEncoderFormat("JPEG");
			EncoderParameters encoderParameters = new EncoderParameters(1);
			encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, (long)quality.Percent);
			bitmap.Save(outStream, encoderFormat, encoderParameters);
		}

		public static string GetFileExtenstion(ImageFormat format)
		{
			if (format.Guid == ImageFormat.Bmp.Guid)
			{
				return ".bmp";
			}
			if (format.Guid == ImageFormat.Emf.Guid)
			{
				return ".emf";
			}
			if (format.Guid == ImageFormat.Exif.Guid)
			{
				return ".emf";
			}
			if (format.Guid == ImageFormat.Gif.Guid)
			{
				return ".gif";
			}
			if (format.Guid == ImageFormat.Icon.Guid)
			{
				return ".ico";
			}
			if (format.Guid == ImageFormat.Jpeg.Guid)
			{
				return ".jpg";
			}
			if (format.Guid == ImageFormat.Png.Guid)
			{
				return ".png";
			}
			return null;
		}

		public static Image ReshapeImageBuffered(Image orginalImage, Size newSize, Color bufferColor, PixelFormat destFormat)
		{
			Bitmap bitmap = new Bitmap(newSize.Width, newSize.Height, destFormat);
			using (Graphics graphics = Graphics.FromImage(bitmap))
			{
				graphics.Clear(bufferColor);
				float num = (float)orginalImage.Width / (float)orginalImage.Height;
				float num2 = (float)newSize.Width / (float)newSize.Height;
				if (num > num2)
				{
					float num3 = (float)newSize.Width / (float)orginalImage.Width;
					int num4 = (int)(num3 * (float)orginalImage.Height);
					int num5 = newSize.Height - num4;
					Rectangle destRect = new Rectangle(0, num5 / 2, newSize.Width, num4);
					graphics.DrawImage(orginalImage, destRect, 0, 0, orginalImage.Width, orginalImage.Height, GraphicsUnit.Pixel);
				}
				else
				{
					float num6 = (float)newSize.Height / (float)orginalImage.Height;
					int num7 = (int)(num6 * (float)orginalImage.Width);
					int num8 = newSize.Width - num7;
					Rectangle destRect2 = new Rectangle(num8 / 2, 0, num7, newSize.Height);
					graphics.DrawImage(orginalImage, destRect2, 0, 0, orginalImage.Width, orginalImage.Height, GraphicsUnit.Pixel);
				}
			}
			return bitmap;
		}

		public static Image ReshapeImageCropped(Image sourceImage, Size newSize, ImageTools.CropOptions cropOptions)
		{
			float cropAspectRatio = (float)newSize.Width / (float)newSize.Height;
			Image image = ImageTools.CropToAspectRatio(sourceImage, cropAspectRatio, cropOptions);
			return ImageTools.Resample(image, Percentage.FromFraction((float)newSize.Width / (float)image.Width));
		}

		public static string GetFileExtenstion(Image image)
		{
			return ImageTools.GetFileExtenstion(image.RawFormat);
		}

		public static Bitmap ChangeFormat(Image source, PixelFormat format)
		{
			Bitmap bitmap = new Bitmap(source.Width, source.Height, format);
			using (Graphics graphics = Graphics.FromImage(bitmap))
			{
				graphics.DrawImageUnscaled(source, 0, 0);
			}
			return bitmap;
		}

		public static Image Resample(Image source, Percentage percentage)
		{
			int width = (int)Math.Round((double)(percentage.Fraction * (float)source.Width));
			int height = (int)Math.Round((double)(percentage.Fraction * (float)source.Height));
			return ImageTools.Resample(source, new Size(width, height));
		}

		public static Image Resample(Image source, Size newSize)
		{
			Bitmap bitmap = new Bitmap(source, newSize);
			using (Graphics graphics = Graphics.FromImage(bitmap))
			{
				Rectangle srcRect = new Rectangle(0, 0, source.Width, source.Height);
				Rectangle destRect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
				graphics.DrawImage(source, destRect, srcRect, GraphicsUnit.Pixel);
			}
			return bitmap;
		}

		public static Image CropToAspectRatio(Image source, float cropAspectRatio, ImageTools.CropOptions options)
		{
			float num = (float)source.Width / (float)source.Height;
			int num2;
			int num3;
			int y;
			int x;
			if (cropAspectRatio < num)
			{
				num2 = source.Height;
				num3 = (int)((float)source.Height * cropAspectRatio);
				y = 0;
				switch (options)
				{
				case ImageTools.CropOptions.Start:
					x = 0;
					break;
				case ImageTools.CropOptions.Middle:
					x = (source.Width - num3) / 2;
					break;
				case ImageTools.CropOptions.End:
					x = num3 - source.Width;
					break;
				default:
					throw new ArgumentException("Not a valid Crop option");
				}
			}
			else
			{
				num3 = source.Width;
				num2 = (int)((float)source.Width / cropAspectRatio);
				x = 0;
				switch (options)
				{
				case ImageTools.CropOptions.Start:
					y = 0;
					break;
				case ImageTools.CropOptions.Middle:
					y = (source.Height - num2) / 2;
					break;
				case ImageTools.CropOptions.End:
					y = num2 - source.Height;
					break;
				default:
					throw new ArgumentException("Not a valid Crop option");
				}
			}
			Bitmap bitmap = new Bitmap(source, num3, num2);
			using (Graphics graphics = Graphics.FromImage(bitmap))
			{
				Rectangle srcRect = new Rectangle(x, y, num3, num2);
				Rectangle destRect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
				graphics.DrawImage(source, destRect, srcRect, GraphicsUnit.Pixel);
			}
			return bitmap;
		}

		public static Bitmap ExtractSubImage(Bitmap sourceBmp, Rectangle sourceRect)
		{
			Bitmap bitmap = new Bitmap(sourceRect.Width, sourceRect.Height, sourceBmp.PixelFormat);
			Graphics graphics = Graphics.FromImage(bitmap);
			Rectangle destRect = new Rectangle(new Point(0, 0), sourceRect.Size);
			graphics.DrawImage(sourceBmp, destRect, sourceRect, GraphicsUnit.Pixel);
			return bitmap;
		}

		public static bool IsEditable(PixelFormat format)
		{
			return format != PixelFormat.Format1bppIndexed && format != PixelFormat.Format4bppIndexed && format != PixelFormat.Format8bppIndexed && format != PixelFormat.Undefined && format != PixelFormat.Undefined && format != PixelFormat.Format16bppArgb1555 && format != PixelFormat.Format16bppGrayScale;
		}

		public static Bitmap WaterMark(Image orginal, Image watermark, Rectangle markLocation)
		{
			Bitmap bitmap;
			if (ImageTools.IsEditable(orginal.PixelFormat))
			{
				bitmap = ImageTools.ChangeFormat(orginal, orginal.PixelFormat);
			}
			else
			{
				bitmap = ImageTools.ChangeFormat(orginal, PixelFormat.Format32bppArgb);
			}
			using (Graphics graphics = Graphics.FromImage(bitmap))
			{
				float[][] array = new float[5][];
				float[][] array2 = array;
				int num = 0;
				float[] array3 = new float[5];
				array3[0] = 1f;
				array3[3] = 1f;
				array2[num] = array3;
				float[][] array4 = array;
				int num2 = 1;
				float[] array5 = new float[5];
				array5[1] = 1f;
				array5[3] = 1f;
				array4[num2] = array5;
				float[][] array6 = array;
				int num3 = 2;
				float[] array7 = new float[5];
				array7[2] = 1f;
				array7[3] = 1f;
				array6[num3] = array7;
				float[][] array8 = array;
				int num4 = 3;
				float[] array9 = new float[5];
				array8[num4] = array9;
				array[4] = new float[]
				{
					0f,
					0f,
					0f,
					0f,
					1f
				};
				float[][] newColorMatrix = array;
				ColorMatrix newColorMatrix2 = new ColorMatrix(newColorMatrix);
				ImageAttributes imageAttributes = new ImageAttributes();
				imageAttributes.SetColorMatrix(newColorMatrix2, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
				graphics.DrawImage(watermark, markLocation, 0, 0, watermark.Width, watermark.Height, GraphicsUnit.Pixel, imageAttributes);
			}
			return bitmap;
		}
	}
}
