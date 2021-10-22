using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace DNA.Net.Lidgren
{
	public static class XNAExtensions
	{
		public static void Write(this NetBuffer message, Point value)
		{
			message.Write(value.X);
			message.Write(value.Y);
		}

		public static Point ReadPoint(this NetBuffer message)
		{
			return new Point(message.ReadInt32(), message.ReadInt32());
		}

		public static void WriteHalfPrecision(this NetBuffer message, float value)
		{
			message.Write(new HalfSingle(value).PackedValue);
		}

		public static float ReadHalfPrecisionSingle(this NetBuffer message)
		{
			HalfSingle halfSingle = default(HalfSingle);
			halfSingle.PackedValue = message.ReadUInt16();
			return halfSingle.ToSingle();
		}

		public static void Write(this NetBuffer message, Vector2 vector)
		{
			message.Write(vector.X);
			message.Write(vector.Y);
		}

		public static Vector2 ReadVector2(this NetBuffer message)
		{
			Vector2 result;
			result.X = message.ReadSingle();
			result.Y = message.ReadSingle();
			return result;
		}

		public static void Write(this NetBuffer message, Vector3 vector)
		{
			message.Write(vector.X);
			message.Write(vector.Y);
			message.Write(vector.Z);
		}

		public static void WriteHalfPrecision(this NetBuffer message, Vector3 vector)
		{
			message.Write(new HalfSingle(vector.X).PackedValue);
			message.Write(new HalfSingle(vector.Y).PackedValue);
			message.Write(new HalfSingle(vector.Z).PackedValue);
		}

		public static Vector3 ReadVector3(this NetBuffer message)
		{
			Vector3 result;
			result.X = message.ReadSingle();
			result.Y = message.ReadSingle();
			result.Z = message.ReadSingle();
			return result;
		}

		public static Vector3 ReadHalfPrecisionVector3(this NetBuffer message)
		{
			HalfSingle halfSingle = default(HalfSingle);
			halfSingle.PackedValue = message.ReadUInt16();
			HalfSingle halfSingle2 = default(HalfSingle);
			halfSingle2.PackedValue = message.ReadUInt16();
			HalfSingle halfSingle3 = default(HalfSingle);
			halfSingle3.PackedValue = message.ReadUInt16();
			Vector3 result;
			result.X = halfSingle.ToSingle();
			result.Y = halfSingle2.ToSingle();
			result.Z = halfSingle3.ToSingle();
			return result;
		}

		public static void Write(this NetBuffer message, Vector4 vector)
		{
			message.Write(vector.X);
			message.Write(vector.Y);
			message.Write(vector.Z);
			message.Write(vector.W);
		}

		public static Vector4 ReadVector4(this NetBuffer message)
		{
			Vector4 result;
			result.X = message.ReadSingle();
			result.Y = message.ReadSingle();
			result.Z = message.ReadSingle();
			result.W = message.ReadSingle();
			return result;
		}

		public static void WriteUnitVector3(this NetBuffer message, Vector3 unitVector, int numberOfBits)
		{
			float x = unitVector.X;
			float y = unitVector.Y;
			float z = unitVector.Z;
			double num = 0.31830988618379069;
			float value = (float)(Math.Atan2((double)x, (double)y) * num);
			float value2 = (float)(Math.Atan2((double)z, Math.Sqrt((double)(x * x + y * y))) * (num * 2.0));
			int num2 = numberOfBits / 2;
			message.WriteSignedSingle(value, num2);
			message.WriteSignedSingle(value2, numberOfBits - num2);
		}

		public static Vector3 ReadUnitVector3(this NetBuffer message, int numberOfBits)
		{
			int num = numberOfBits / 2;
			float num2 = message.ReadSignedSingle(num) * 3.14159274f;
			float num3 = message.ReadSignedSingle(numberOfBits - num) * 1.57079637f;
			Vector3 result;
			result.X = (float)(Math.Sin((double)num2) * Math.Cos((double)num3));
			result.Y = (float)(Math.Cos((double)num2) * Math.Cos((double)num3));
			result.Z = (float)Math.Sin((double)num3);
			return result;
		}

		public static void WriteRotation(this NetBuffer message, Quaternion quaternion, int bitsPerElement)
		{
			if (quaternion.X > 1f)
			{
				quaternion.X = 1f;
			}
			if (quaternion.Y > 1f)
			{
				quaternion.Y = 1f;
			}
			if (quaternion.Z > 1f)
			{
				quaternion.Z = 1f;
			}
			if (quaternion.W > 1f)
			{
				quaternion.W = 1f;
			}
			if (quaternion.X < -1f)
			{
				quaternion.X = -1f;
			}
			if (quaternion.Y < -1f)
			{
				quaternion.Y = -1f;
			}
			if (quaternion.Z < -1f)
			{
				quaternion.Z = -1f;
			}
			if (quaternion.W < -1f)
			{
				quaternion.W = -1f;
			}
			message.WriteSignedSingle(quaternion.X, bitsPerElement);
			message.WriteSignedSingle(quaternion.Y, bitsPerElement);
			message.WriteSignedSingle(quaternion.Z, bitsPerElement);
			message.WriteSignedSingle(quaternion.W, bitsPerElement);
		}

		public static Quaternion ReadRotation(this NetBuffer message, int bitsPerElement)
		{
			Quaternion result;
			result.X = message.ReadSignedSingle(bitsPerElement);
			result.Y = message.ReadSignedSingle(bitsPerElement);
			result.Z = message.ReadSignedSingle(bitsPerElement);
			result.W = message.ReadSignedSingle(bitsPerElement);
			return result;
		}

		public static void WriteMatrix(this NetBuffer message, ref Matrix matrix)
		{
			Quaternion quaternion = Quaternion.CreateFromRotationMatrix(matrix);
			message.WriteRotation(quaternion, 24);
			message.Write(matrix.M41);
			message.Write(matrix.M42);
			message.Write(matrix.M43);
		}

		public static void WriteMatrix(this NetBuffer message, Matrix matrix)
		{
			Quaternion quaternion = Quaternion.CreateFromRotationMatrix(matrix);
			message.WriteRotation(quaternion, 24);
			message.Write(matrix.M41);
			message.Write(matrix.M42);
			message.Write(matrix.M43);
		}

		public static Matrix ReadMatrix(this NetBuffer message)
		{
			Quaternion quaternion = message.ReadRotation(24);
			Matrix result = Matrix.CreateFromQuaternion(quaternion);
			result.M41 = message.ReadSingle();
			result.M42 = message.ReadSingle();
			result.M43 = message.ReadSingle();
			return result;
		}

		public static void ReadMatrix(this NetBuffer message, ref Matrix destination)
		{
			Quaternion quaternion = message.ReadRotation(24);
			destination = Matrix.CreateFromQuaternion(quaternion);
			destination.M41 = message.ReadSingle();
			destination.M42 = message.ReadSingle();
			destination.M43 = message.ReadSingle();
		}

		public static void Write(this NetBuffer message, BoundingSphere bounds)
		{
			message.Write(bounds.Center.X);
			message.Write(bounds.Center.Y);
			message.Write(bounds.Center.Z);
			message.Write(bounds.Radius);
		}

		public static BoundingSphere ReadBoundingSphere(this NetBuffer message)
		{
			BoundingSphere result;
			result.Center.X = message.ReadSingle();
			result.Center.Y = message.ReadSingle();
			result.Center.Z = message.ReadSingle();
			result.Radius = message.ReadSingle();
			return result;
		}
	}
}
