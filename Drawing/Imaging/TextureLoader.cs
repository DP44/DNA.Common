using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using DNA.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace DNA.Drawing.Imaging
{
	public static class TextureLoader
	{
		[Serializable]
		public class FormatException : Exception
		{
			public FormatException()
			{
			}

			public FormatException(string message) : base(message)
			{
			}

			public FormatException(string message, Exception inner) : base(message, inner)
			{
			}

			protected FormatException(SerializationInfo info, StreamingContext context) : base(info, context)
			{
			}
		}

		private struct DDS_PIXELFORMAT
		{
			public uint Size;

			public uint Flags;

			public uint FourCC;

			public uint RGBBitCount;

			public uint RBitMask;

			public uint GBitMask;

			public uint BBitMask;

			public uint ABitMask;

			public void Read(string filename, BinaryReader reader)
			{
				this.Size = reader.ReadUInt32();
				this.Flags = reader.ReadUInt32();
				this.FourCC = reader.ReadUInt32();
				this.RGBBitCount = reader.ReadUInt32();
				this.RBitMask = reader.ReadUInt32();
				this.GBitMask = reader.ReadUInt32();
				this.BBitMask = reader.ReadUInt32();
				this.ABitMask = reader.ReadUInt32();
				if ((this.Flags & 68U) == 0U)
				{
					throw new TextureLoader.FormatException("Pixel format must be either Uncompressed ARGB or DXT1, 3, 5 in DDS file" + filename);
				}
				if (this.Size != 32U)
				{
					throw new TextureLoader.FormatException("Wrong header size in DDS file " + filename);
				}
				if ((this.Flags & 4U) != 0U && this.FourCC == 808540228U)
				{
					throw new TextureLoader.FormatException("Cannot load DX10 DDS file " + filename);
				}
			}
		}

		private struct DDS_HEADER
		{
			public uint Size;

			public uint Flags;

			public uint Height;

			public uint Width;

			public uint PitchOrLinearSize;

			public uint Depth;

			public uint MipMapCount;

			public uint Reserved1;

			public uint Reserved2;

			public uint Reserved3;

			public uint Reserved4;

			public uint Reserved5;

			public uint Reserved6;

			public uint Reserved7;

			public uint Reserved8;

			public uint Reserved9;

			public uint Reserved10;

			public uint Reserved11;

			public TextureLoader.DDS_PIXELFORMAT PixelFormat;

			public uint Caps;

			public uint Caps2;

			public uint Caps3;

			public uint Caps4;

			public uint Reserved12;

			public void Read(string filename, BinaryReader reader)
			{
				this.Size = reader.ReadUInt32();
				if (this.Size != 124U)
				{
					throw new TextureLoader.FormatException("Wrong header size in DDS file " + filename);
				}
				this.Flags = reader.ReadUInt32();
				this.Height = reader.ReadUInt32();
				this.Width = reader.ReadUInt32();
				this.PitchOrLinearSize = reader.ReadUInt32();
				this.Depth = reader.ReadUInt32();
				this.MipMapCount = reader.ReadUInt32();
				this.Reserved1 = reader.ReadUInt32();
				this.Reserved2 = reader.ReadUInt32();
				this.Reserved3 = reader.ReadUInt32();
				this.Reserved4 = reader.ReadUInt32();
				this.Reserved5 = reader.ReadUInt32();
				this.Reserved6 = reader.ReadUInt32();
				this.Reserved7 = reader.ReadUInt32();
				this.Reserved8 = reader.ReadUInt32();
				this.Reserved9 = reader.ReadUInt32();
				this.Reserved10 = reader.ReadUInt32();
				this.Reserved11 = reader.ReadUInt32();
				this.PixelFormat.Read(filename, reader);
				this.Caps = reader.ReadUInt32();
				this.Caps2 = reader.ReadUInt32();
				this.Caps3 = reader.ReadUInt32();
				this.Caps4 = reader.ReadUInt32();
				this.Reserved12 = reader.ReadUInt32();
				if ((this.Caps & 4096U) == 0U)
				{
					throw new TextureLoader.FormatException("Texture flag not present in DDS file" + filename);
				}
			}
		}

		private struct DDSFile
		{
			public uint Magic;

			public TextureLoader.DDS_HEADER Header;

			public void Read(string filename, BinaryReader reader)
			{
				this.Magic = reader.ReadUInt32();
				if (this.Magic != 542327876U)
				{
					throw new TextureLoader.FormatException("Wrong magic number in DDS file " + filename);
				}
				this.Header.Read(filename, reader);
			}
		}

		private static int[] _mipmapOffsets = new int[]
		{
			0,
			1,
			0,
			1
		};

		private static bool IsPowerOfTwo(int x)
		{
			int num = 0;
			while (x != 0)
			{
				num += (x & 1);
				x >>= 1;
			}
			return num == 1;
		}

		private static Byte4[] MakeMipmap(Byte4[] source, ref int width, ref int height, bool treatDataAsNormals)
		{
			if (width == 1 || height == 1)
			{
				return null;
			}
			int num = 0;
			TextureLoader._mipmapOffsets[2] = width;
			TextureLoader._mipmapOffsets[3] = width + 1;
			Byte4[] array = new Byte4[width * height / 4];
			for (int i = 0; i < height; i += 2)
			{
				int num2 = i * width;
				for (int j = 0; j < width; j += 2)
				{
					Vector4 vector = Vector4.Zero;
					int num3 = 0;
					if (treatDataAsNormals)
					{
						for (int k = 0; k < 4; k++)
						{
							vector += source[num2 + TextureLoader._mipmapOffsets[k]].ToVector4();
						}
					}
					else
					{
						for (int l = 0; l < 4; l++)
						{
							Vector4 value = source[num2 + TextureLoader._mipmapOffsets[l]].ToVector4();
							if (value.W > 5f)
							{
								vector += value;
								num3++;
							}
						}
					}
					if (treatDataAsNormals)
					{
						float num4 = vector.W / 4f;
						vector.W = 0f;
						vector.Normalize();
						vector *= 255f;
						vector.W = MathHelper.Clamp((float)Math.Floor((double)num4), 0f, 255f);
					}
					else
					{
						if (num3 > 0)
						{
							vector *= 1f / (float)num3;
						}
						if (vector.W >= 128f)
						{
							vector.W = 255f;
						}
						else
						{
							vector.W = 0f;
						}
					}
					vector.X = MathHelper.Clamp((float)Math.Floor((double)vector.X), 0f, 255f);
					vector.Y = MathHelper.Clamp((float)Math.Floor((double)vector.Y), 0f, 255f);
					vector.Z = MathHelper.Clamp((float)Math.Floor((double)vector.Z), 0f, 255f);
					array[num++] = new Byte4(vector);
					num2 += 2;
				}
			}
			width >>= 1;
			height >>= 1;
			return array;
		}

		private static void SwapChannels(byte[] data)
		{
			for (int i = 0; i < data.Length; i += 4)
			{
				byte b = data[i];
				data[i] = data[i + 2];
				data[i + 2] = b;
			}
		}

		private static Texture2D LoadThroughBitmap(GraphicsDevice gd, string filename, bool makeMipmaps, bool normalizeMipmaps)
		{
			Texture2D result = null;
			Bitmap bitmap = Image.FromFile(filename) as Bitmap;
			PixelFormat pixelFormat = bitmap.PixelFormat;
			if (pixelFormat != PixelFormat.Format32bppArgb && pixelFormat != PixelFormat.Format32bppRgb)
			{
				throw new TextureLoader.FormatException(string.Concat(new string[]
				{
					"Map ",
					filename,
					" has wrong format (",
					bitmap.PixelFormat.ToString(),
					") must be 32 bit ARGB"
				}));
			}
			int width = bitmap.Width;
			int height = bitmap.Height;
			BitmapData bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
			int num = bitmapData.Stride * bitmapData.Height;
			int stride = bitmapData.Stride;
			byte[] tmp = new byte[num];
			Marshal.Copy(bitmapData.Scan0, tmp, 0, num);
			bitmap.UnlockBits(bitmapData);
			bitmap.Dispose();
			if (pixelFormat == PixelFormat.Format32bppRgb)
			{
				for (int i = 3; i < tmp.Length; i += 4)
				{
					tmp[i] = byte.MaxValue;
				}
			}
			TextureLoader.SwapChannels(tmp);
			bool mipMap = makeMipmaps && TextureLoader.IsPowerOfTwo(width) && TextureLoader.IsPowerOfTwo(height);
			result = new Texture2D(gd, width, height, mipMap, SurfaceFormat.Color);
			if (makeMipmaps && TextureLoader.IsPowerOfTwo(width) && TextureLoader.IsPowerOfTwo(height))
			{
				Byte4[] array = new Byte4[width * height];
				array.AsUintArray(delegate(uint[] value)
				{
					Buffer.BlockCopy(tmp, 0, value, 0, tmp.Length);
				});
				int num2 = width;
				int num3 = height;
				int num4 = 0;
				while (array != null)
				{
					result.SetData<Byte4>(num4++, null, array, 0, array.Length);
					array = TextureLoader.MakeMipmap(array, ref num2, ref num3, normalizeMipmaps);
				}
			}
			else
			{
				tmp.AsByte4Array(delegate(Byte4[] value)
				{
					result.SetData<Byte4>(value);
				});
			}
			return result;
		}

		private static Texture2D LoadFromDDS(GraphicsDevice gd, string filename)
		{
			Texture2D texture2D = null;
			using (FileStream fileStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				BinaryReader binaryReader = new BinaryReader(fileStream);
				TextureLoader.DDSFile ddsfile = default(TextureLoader.DDSFile);
				ddsfile.Read(filename, binaryReader);
				bool flag = false;
				bool flag2 = false;
				SurfaceFormat surfaceFormat;
				if ((ddsfile.Header.PixelFormat.Flags & 64U) != 0U)
				{
					if (ddsfile.Header.PixelFormat.RGBBitCount == 32U)
					{
						surfaceFormat = SurfaceFormat.Color;
					}
					else
					{
						if (ddsfile.Header.PixelFormat.RGBBitCount != 16U)
						{
							throw new TextureLoader.FormatException("Uncompressed DDS Surface format must be 32bit RGBA or ABGR, or  16 bit BGR565, BGRA4444, or BGRA5551: " + filename);
						}
						flag2 = true;
						uint rgbbitCount = ddsfile.Header.PixelFormat.RGBBitCount;
						if (rgbbitCount != 1984U)
						{
							if (rgbbitCount != 2016U)
							{
								if (rgbbitCount != 3840U)
								{
									throw new TextureLoader.FormatException("Uncompressed DDS Surface format must be 32bit RGBA or ABGR, or  16 bit BGR565, BGRA4444, or BGRA5551: " + filename);
								}
								surfaceFormat = SurfaceFormat.Bgra4444;
							}
							else
							{
								surfaceFormat = SurfaceFormat.Bgr565;
							}
						}
						else
						{
							surfaceFormat = SurfaceFormat.Bgra5551;
						}
					}
				}
				else
				{
					flag = true;
					uint fourCC = ddsfile.Header.PixelFormat.FourCC;
					if (fourCC != 827611204U)
					{
						if (fourCC != 861165636U)
						{
							if (fourCC != 894720068U)
							{
								throw new TextureLoader.FormatException("Compressed DDS Surface format must be DXT1, DXT3, DXT5: " + filename);
							}
							surfaceFormat = SurfaceFormat.Dxt5;
						}
						else
						{
							surfaceFormat = SurfaceFormat.Dxt3;
						}
					}
					else
					{
						surfaceFormat = SurfaceFormat.Dxt1;
					}
				}
				texture2D = new Texture2D(gd, (int)ddsfile.Header.Width, (int)ddsfile.Header.Height, ddsfile.Header.MipMapCount > 0U, surfaceFormat);
				int num = (int)ddsfile.Header.Width;
				int num2 = (int)ddsfile.Header.Height;
				for (int i = 0; i < (int)(1U + ddsfile.Header.MipMapCount); i++)
				{
					if (flag)
					{
						int num3 = Math.Max(1, (num + 3) / 4) * Math.Max(1, (num2 + 3) / 4) * ((surfaceFormat == SurfaceFormat.Dxt1) ? 8 : 16);
						byte[] array = binaryReader.ReadBytes(num3);
						if (array.Length != 0)
						{
							texture2D.SetData<byte>(i, null, array, 0, array.Length);
						}
					}
					else if (flag2)
					{
						int num3 = num * num2 * 2;
						short[] array2 = new short[num3 / 2];
						byte[] array = binaryReader.ReadBytes(num3);
						if (array.Length != 0)
						{
							Buffer.BlockCopy(array, 0, array2, 0, array.Length);
							texture2D.SetData<short>(i, null, array2, 0, array2.Length);
						}
					}
					else
					{
						int num3 = num * num2 * 4;
						int[] array3 = new int[num3 / 4];
						byte[] array = binaryReader.ReadBytes(num3);
						if (array.Length != 0)
						{
							Buffer.BlockCopy(array, 0, array3, 0, array.Length);
							texture2D.SetData<int>(i, null, array3, 0, array3.Length);
						}
					}
					num /= 2;
					num2 /= 2;
				}
			}
			return texture2D;
		}

		public static Texture2D LoadFromFile(GraphicsDevice gd, string filename, bool makeMipmaps, bool normalizeMipmaps)
		{
			string a = Path.GetExtension(filename).ToLower();
			Texture2D result;
			if (a == ".dds")
			{
				result = TextureLoader.LoadFromDDS(gd, filename);
			}
			else
			{
				result = TextureLoader.LoadThroughBitmap(gd, filename, makeMipmaps, normalizeMipmaps);
			}
			return result;
		}
	}
}
