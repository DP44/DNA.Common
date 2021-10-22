using System;
using DNA.Collections;
using Microsoft.Xna.Framework;

namespace DNA
{
	public static class MathTools
	{
		public const float Sqrt2 = 1.414213f;

		private static readonly byte[] BitsSetTable256 = new byte[]
		{
			0, 1, 1, 2, 1, 2, 2, 3, 1, 2, 2, 3, 2, 3, 3, 4, 1, 2, 2, 3, 2, 3, 3,
			4, 2, 3, 3, 4, 3, 4, 4, 5, 1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4,
			4, 5, 2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6, 1, 2, 2, 3, 2, 
			3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5, 2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5,
			4, 5, 5, 6, 2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6, 3, 4, 4,
			5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7, 1, 2, 2, 3, 2, 3, 3, 4, 2, 3,
			3, 4, 3, 4, 4, 5, 2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6, 2, 
			3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6, 3, 4, 4, 5, 4, 5, 5, 6,
			4, 5, 5, 6, 5, 6, 6, 7, 2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5,
			6, 3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7, 3, 4, 4, 5, 4, 5, 
			5, 6, 4, 5, 5, 6, 5, 6, 6, 7, 4, 5, 5, 6, 5, 6, 6, 7, 5, 6, 6, 7, 6, 
			7, 7, 8
		};

		private static readonly byte[] LogTable256 = new byte[]
		{
			0, 0,
			
			1, 1,
			
			2, 2, 2, 2,
			
			3, 3, 3, 3, 3, 3, 3, 3,
			
			4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
			
			5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
			
			6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 
			6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
			
			7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 
			7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 
			7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 
			7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7
		};

		private static readonly int[] MultiplyDeBruijnBitPosition = new int[]
		{
			0,  1,  28, 2,  
			29, 14, 24, 3,  
			30, 22, 20, 15, 
			25, 17, 4,  8,  
			31, 27, 13, 23, 
			21, 19, 16, 7,  
			26, 12, 18, 6,  
			11, 5,  10, 9
		};

		public static Random Rnd = new Random();

		public static float Mod(float value, float modulus)
		{
			return (value % modulus + modulus) % modulus;
		}

		public static int Mod(int value, int modulus)
		{
			return (value % modulus + modulus) % modulus;
		}

		public static double Mod(double value, double modulus)
		{
			return (value % modulus + modulus) % modulus;
		}

		public static Angle AngleBetween(this Quaternion q1, Quaternion q2)
		{
			return Angle.ACos((double)Quaternion.Dot(q1, q2)) * 2f;
		}

		public static Vector3 Floor(this Vector3 v)
		{
			return new Vector3((float)Math.Floor((double)v.X), (float)Math.Floor((double)v.Y), (float)Math.Floor((double)v.Z));
		}

		public static Angle AngleBetween(this Vector3 v1, Vector3 v2)
		{
			v1.Normalize();
			v2.Normalize();
			float dotProduct = Vector3.Dot(v1, v2);
			dotProduct = Math.Min(dotProduct, 1f);
			dotProduct = Math.Max(dotProduct, -1f);
			float rads = (float)Math.Acos((double)dotProduct);
			return Angle.FromRadians(rads);
		}

		public static bool Coincident(this Vector3 v1, Vector3 v2)
		{
			return Vector3.DistanceSquared(v1, v2) < 1E-05f;
		}

		public static bool Coincident(this Vector3 v1, Vector3 v2, float tolerance)
		{
			return Vector3.DistanceSquared(v1, v2) < tolerance;
		}

		public static Quaternion RotationBetween(this Vector3 v1, Vector3 v2)
		{
			v1.Normalize();
			v2.Normalize();
			Vector3 crossProduct = Vector3.Cross(v1, v2);
			return new Quaternion(crossProduct.X, crossProduct.Y, crossProduct.Z, 1f + Vector3.Dot(v1, v2));
		}

		public static Vector3 GetValidUpVector(Vector3 forward)
		{
			if (Math.Abs(forward.X) + Math.Abs(forward.Z) > 0f)
			{
				return Vector3.Up;
			}

			return Vector3.Right;
		}

		public static Matrix CreateWorld(Vector3 position, Vector3 fwd)
		{
			return Matrix.CreateWorld(position, fwd, MathTools.GetValidUpVector(fwd));
		}

		public static double Distance(this Point p1, Point p2)
		{
			int xDist = p2.X - p1.X;
			int yDist = p2.Y - p1.Y;

			return Math.Sqrt((double)(xDist * xDist + yDist * yDist));
		}

		public static float Cross(this Vector2 v1, Vector2 v2)
		{
			return v1.X * v2.Y - v1.Y * v2.X;
		}

		public static int[] RandomArray(int length)
		{
			return MathTools.RandomArray(0, length);
		}

		public static int[] RandomArray(int start, int length)
		{
			return new Random().RandomArray(start, length);
		}

		public static int[] RandomArray(this Random rand, int length)
		{
			return rand.RandomArray(0, length);
		}

		public static bool Decide(this Random rand, float percentTrue)
		{
			return rand.NextDouble() > (double)percentTrue;
		}

		public static int[] RandomArray(this Random rand, int start, int length)
		{
			int[] randomArray = new int[length];
			
			for (int i = 0; i < length; i++)
			{
				randomArray[i] = start + i;
			}

			ArrayTools.Randomize<int>(randomArray, rand);
			return randomArray;
		}

		public static double RandomDouble(this Random rand, double min, double max)
		{
			if (max < min)
			{
				double num = max;
				max = min;
				min = num;
			}

			double num2 = max - min;
			double num3 = num2 * rand.NextDouble();
			return min + num3;
		}

		public static float RandomFloat()
		{
			return (float)MathTools.Rnd.NextDouble();
		}

		public static float RandomFloat(float magnitude)
		{
			return (float)MathTools.Rnd.NextDouble() * magnitude;
		}

		public static float RandomFloat(float min, float max)
		{
			return MathHelper.Lerp(min, max, (float)MathTools.Rnd.NextDouble());
		}

		public static int RandomInt()
		{
			return MathTools.Rnd.Next();
		}

		public static int RandomInt(int max)
		{
			return MathTools.Rnd.Next(0, max);
		}

		public static int RandomInt(int min, int max)
		{
			return MathTools.Rnd.Next(min, max);
		}

		public static bool RandomBool()
		{
			return MathTools.Rnd.Next(0, 2) == 0;
		}

		public static int IntDifference(float a, float b)
		{
			throw new NotImplementedException();
		}

		public static int IntDifference(double a, double b)
		{
			throw new NotImplementedException();
		}

		public static int IntRound(double d)
		{
			return (int)Math.Round(d);
		}

		public static int IntRound(float d)
		{
			return (int)Math.Round((double)d);
		}

		public static float MapAndLerp(float value, float map1, float map2, float lerp1, float lerp2)
		{
			float amount = ((value - map1) / (map2 - map1)).Clamp(0f, 1f);
			return MathHelper.Lerp(lerp1, lerp2, amount);
		}

		public static float GetTimeFixedLerpValue(float deltaTime, float timeToPercentage, float percentage)
		{
			if (timeToPercentage == 0f)
			{
				return 1f;
			}
			float num = deltaTime / timeToPercentage;
			return 1f - (float)Math.Pow((double)percentage, (double)num);
		}

		public static float GetTimeFixedLerpValue(float deltaTime, float timeToPercentage)
		{
			return MathTools.GetTimeFixedLerpValue(deltaTime, timeToPercentage, 0.5f);
		}

		public static float Clamp(this float val, float min, float max)
		{
			if (val < min)
			{
				return min;
			}
			
			if (val > max)
			{
				return max;
			}

			return val;
		}

		public static int Clamp(this int val, int min, int max)
		{
			if (val < min)
			{
				return min;
			}

			if (val > max)
			{
				return max;
			}

			return val;
		}

		public static bool IsPowerOf2(this int v)
		{
			return (v & v - 1) == 0 && v != 0;
		}

		public static bool IsPowerOf2(this uint v)
		{
			return (v & v - 1U) == 0U && v != 0U;
		}

		public static uint NextPowerOf2(this uint v)
		{
			v -= 1U;
			v |= v >> 1;
			v |= v >> 2;
			v |= v >> 4;
			v |= v >> 8;
			v |= v >> 16;
			v += 1U;
			return v;
		}

		public static int LogBase2(this uint v)
		{
			uint num;
			
			if ((num = v >> 16) != 0U)
			{
				uint num2;
				
				if ((num2 = num >> 8) == 0U)
				{
					return (int)(16 + MathTools.LogTable256[(int)((UIntPtr)num)]);
				}
				
				return (int)(24 + MathTools.LogTable256[(int)((UIntPtr)num2)]);
			}
			else
			{
				uint num2;
				
				if ((num2 = v >> 8) == 0U)
				{
					return (int)MathTools.LogTable256[(int)((UIntPtr)v)];
				}
				
				return (int)(8 + MathTools.LogTable256[(int)((UIntPtr)num2)]);
			}
		}

		public static Matrix QuickInvert(this Matrix mat)
		{
			Matrix identity = Matrix.Identity;
			Vector3 translation = mat.Translation;
			identity.M11 = mat.M11;
			identity.M12 = mat.M21;
			identity.M13 = mat.M31;
			identity.M21 = mat.M12;
			identity.M22 = mat.M22;
			identity.M23 = mat.M32;
			identity.M31 = mat.M13;
			identity.M32 = mat.M23;
			identity.M33 = mat.M33;
			identity.M41 = -Vector3.Dot(translation, mat.Right);
			identity.M42 = -Vector3.Dot(translation, mat.Up);
			identity.M43 = -Vector3.Dot(translation, mat.Forward);
			return identity;
		}

		public static int TrailingZeroBits(this uint v)
		{
			return MathTools.MultiplyDeBruijnBitPosition[(int)(checked((IntPtr)(unchecked(((ulong)v & -(ulong)v) * 125613361UL) >> 27)))];
		}

		public static int BitsSet(this uint v)
		{
			return (int)(MathTools.BitsSetTable256[(int)((UIntPtr)(v & 255U))] + MathTools.BitsSetTable256[(int)((UIntPtr)(v >> 8 & 255U))] + MathTools.BitsSetTable256[(int)((UIntPtr)(v >> 16 & 255U))] + MathTools.BitsSetTable256[(int)((UIntPtr)(v >> 24))]);
		}

		public static uint Max(uint a, uint b, uint c)
		{
			if (a <= b)
			{
				if (b <= c)
				{
					return c;
				}
				
				return b;
			}
			else
			{
				if (a <= c)
				{
					return c;
				}
				
				return a;
			}
		}

		public static int Max(int a, int b, int c)
		{
			if (a <= b)
			{
				if (b <= c)
				{
					return c;
				}
				
				return b;
			}
			else
			{
				if (a <= c)
				{
					return c;
				}
				
				return a;
			}
		}

		public static float Max(float a, float b, float c)
		{
			if (a <= b)
			{
				if (b <= c)
				{
					return c;
				}
				
				return b;
			}
			else
			{
				if (a <= c)
				{
					return c;
				}
				
				return a;
			}
		}

		public static decimal Max(decimal a, decimal b, decimal c)
		{
			if (!(a > b))
			{
				if (b <= c)
				{
					return c;
				}
				
				return b;
			}
			else
			{
				if (b <= c)
				{
					return c;
				}
				
				return a;
			}
		}

		public static short Max(short a, short b, short c)
		{
			if (a <= b)
			{
				if (b <= c)
				{
					return c;
				}
				
				return b;
			}
			else
			{
				if (a <= c)
				{
					return c;
				}
				
				return a;
			}
		}

		public static char Max(char a, char b, char c)
		{
			if (a <= b)
			{
				if (b <= c)
				{
					return c;
				}
				
				return b;
			}
			else
			{
				if (a <= c)
				{
					return c;
				}
				
				return a;
			}
		}

		public static byte Max(byte a, byte b, byte c)
		{
			if (a <= b)
			{
				if (b <= c)
				{
					return c;
				}
				
				return b;
			}
			else
			{
				if (a <= c)
				{
					return c;
				}
				
				return a;
			}
		}

		public static uint Min(uint a, uint b, uint c)
		{
			if (a >= b)
			{
				if (b >= c)
				{
					return c;
				}
				
				return b;
			}
			else
			{
				if (a >= c)
				{
					return c;
				}
				
				return a;
			}
		}

		public static int Min(int a, int b, int c)
		{
			if (a >= b)
			{
				if (b >= c)
				{
					return c;
				}
				
				return b;
			}
			else
			{
				if (a >= c)
				{
					return c;
				}
				
				return a;
			}
		}

		public static float Min(float a, float b, float c)
		{
			if (a >= b)
			{
				if (b >= c)
				{
					return c;
				}
				
				return b;
			}
			else
			{
				if (a >= c)
				{
					return c;
				}
				
				return a;
			}
		}

		public static decimal Min(decimal a, decimal b, decimal c)
		{
			if (!(a < b))
			{
				if (a >= c)
				{
					return c;
				}
				
				return b;
			}
			else
			{
				if (a >= c)
				{
					return c;
				}
				
				return a;
			}
		}

		public static short Min(short a, short b, short c)
		{
			if (a >= b)
			{
				if (b >= c)
				{
					return c;
				}
				
				return b;
			}
			else
			{
				if (a >= c)
				{
					return c;
				}
				
				return a;
			}
		}

		public static char Min(char a, char b, char c)
		{
			if (a >= b)
			{
				if (b >= c)
				{
					return c;
				}
				
				return b;
			}
			else
			{
				if (a >= c)
				{
					return c;
				}
				
				return a;
			}
		}

		public static byte Min(byte a, byte b, byte c)
		{
			if (a >= b)
			{
				if (b >= c)
				{
					return c;
				}
				
				return b;
			}
			else
			{
				if (a >= c)
				{
					return c;
				}
				
				return a;
			}
		}

		public static int DecimalToPow2Fraction(double val, out int denom, int maxDenom)
		{
			double num = 1.0;
			int num2 = maxDenom;
			
			for (int i = 1; i < maxDenom; i <<= 1)
			{
				double num3 = val * (double)i;
				double num4 = num3 - Math.Floor(num3);
				
				if (num4 == 0.0)
				{
					denom = i;
					return (int)num3;
				}
				
				if (num4 < num)
				{
					num = num4;
					num2 = i;
				}
			}

			denom = num2;
			return (int)val * num2;
		}

		public static int Factorial(int n)
		{
			int factorialOfN = 1;
			
			for (int i = n; i > 1; i--)
			{
				factorialOfN *= i;
			}
			
			return factorialOfN;
		}

		public static int Permutations(int n, int r)
		{
			return MathTools.Factorial(n) / MathTools.Factorial(n - r);
		}

		public static int Combinations(int n, int r)
		{
			return MathTools.Permutations(n, r) / MathTools.Factorial(r);
		}

		public static int DecimalToFraction(double val, out int denom, int maxDenom)
		{
			double num = 1.0;
			int num2 = maxDenom;
			
			for (int i = 1; i < maxDenom; i++)
			{
				double num3 = val * (double)i;
				double num4 = num3 - Math.Floor(num3);
				
				if (num4 == 0.0)
				{
					denom = i;
					return (int)num3;
				}
				
				if (num4 < num)
				{
					num = num4;
					num2 = i;
				}
			}

			denom = num2;
			return (int)val * num2;
		}

		public static float Square(float x)
		{
			return x * x;
		}

		public static int Square(int x)
		{
			return x * x;
		}

		public static bool CalculateInitialBallisticVector(Vector3 pt1, Vector3 pt2, float vel, out Vector3 res, float gravity)
		{
			float num = gravity * gravity;
			float num2 = pt2.Y - pt1.Y;
			float num3 = (float)Math.Sqrt((double)(MathTools.Square(pt1.X - pt2.X) + MathTools.Square(pt1.Z - pt2.Z)));
			res = Vector3.Zero;
			
			if (num3 == 0f)
			{
				if (num2 >= 0f)
				{
					res = new Vector3(0f, vel, 0f);
				}
				else
				{
					res = new Vector3(0f, -vel, 0f);
				}
				return true;
			}
			
			float num4 = num2 * num2;
			float num5 = num3 * num3;
			float num6 = 2f * num4 + 2f * num5;
			
			if (num6 == 0f)
			{
				return false;
			}
			
			float num7 = vel * vel;
			float num8 = 2f * gravity * num2 * num7 + num7 * num7 - num * num5;
			
			if (num8 < 0f)
			{
				return false;
			}
			
			num8 = (float)Math.Sqrt((double)num8);
			float num9 = gravity * num2 + num7;
			float num10 = num9 - num8;
			float num11 = num9 + num8;
			float num12 = num5 + num4;
			num10 *= num12;
			num11 *= num12;
			num6 = 1.414213f * num3 / num6;
			bool flag = false;
			
			if (num10 >= 0f)
			{
				num10 = (float)Math.Sqrt((double)num10) * num6;
				flag = true;
			}
			
			if (num11 >= 0f)
			{
				num11 = (float)Math.Sqrt((double)num11) * num6;
			
				if (!flag || num11 > num10)
				{
					flag = true;
					num10 = num11;
				}
			}
			
			if (flag)
			{
				if (num10 <= 0f)
				{
					return false;
				}
			
				float num13 = num10 * num10;
			
				if (num13 > num7)
				{
					return false;
				}
				
				float num14 = num3 / num10;
				float num15 = (float)Math.Sqrt((double)(num7 - num13));
				
				if (Math.Abs(num2 - (num15 * num14 + 0.5f * gravity * num14 * num14)) > Math.Abs(num2 - (-num15 * num14 + 0.5f * gravity * num14 * num14)))
				{
					num15 = -num15;
				}
				
				Vector2 vector = new Vector2(pt2.X - pt1.X, pt2.Z - pt1.Z);
				vector.Normalize();
			
				res.X = num10 * vector.X;
				res.Y = num15;
				res.Z = num10 * vector.Y;
			}

			return flag;
		}

		public static float MoveTowardTarget(float current, float target, float rate, float dt)
		{
			float distLeft;
			
			if (target != current)
			{
				float distToTarget = target - current;
				float distToTargetAbs = Math.Abs(distToTarget);
			
				if (distToTargetAbs > rate)
				{
					distToTarget *= rate / distToTargetAbs;
				}
			
				distLeft = current + distToTarget * dt;

				if (Math.Abs(distLeft - target) < 0.01f)
				{
					distLeft = target;
				}
			}
			else
			{
				distLeft = current;
			}

			return distLeft;
		}

		public static float MoveTowardTargetAngle(float current, float target, float rate, float dt)
		{
			float angDistLeft;
			
			if (target != current)
			{
				float angDist = MathHelper.WrapAngle(target - current);
				float angDistAbs = Math.Abs(angDist);
			
				if (angDistAbs > rate)
				{
					angDist *= rate / angDistAbs;
				}

				angDistLeft = MathHelper.WrapAngle(current + angDist * dt);
				
				if (Math.Abs(angDistLeft - target) < 0.01f)
				{
					angDistLeft = target;
				}
			}
			else
			{
				angDistLeft = current;
			}

			return angDistLeft;
		}

		public static Vector3 Hermite1stDerivative(Vector3 Point1, Vector3 Tangent1, Vector3 Point2, Vector3 Tangent2, float t)
		{
			float num = t * t;
			Vector3 value = Point1 * (6f * (num - t));
			value += Tangent1 * (3f * num - 4f * t + 1f);
			value += Point2 * (6f * (t - num));
			return value + Tangent2 * (3f * num - 2f * t);
		}

		public static Vector3 Hermite2ndDerivative(Vector3 Point1, Vector3 Tangent1, Vector3 Point2, Vector3 Tangent2, float t)
		{
			Vector3 value = Point1 * (12f * t - 6f);
			value += Tangent1 * (6f * t - 4f);
			value += Point2 * (6f - 12f * t);
			return value + Tangent2 * (6f * t - 2f);
		}
	}
}
