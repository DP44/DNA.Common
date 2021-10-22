using System;
using System.IO;
using DNA.IO;
using Microsoft.Xna.Framework;

namespace DNA.Drawing
{
	public struct Triangle3D
	{
		private Vector3 _a;

		private Vector3 _b;

		private Vector3 _c;

		public Vector3 Normal
		{
			get
			{
				return Vector3.Cross(this._b - this._a, this._c - this._a);
			}
		}

		public LineF3D AB
		{
			get
			{
				return new LineF3D(this._a, this._b);
			}
		}

		public LineF3D AC
		{
			get
			{
				return new LineF3D(this._a, this._c);
			}
		}

		public LineF3D BC
		{
			get
			{
				return new LineF3D(this._b, this._c);
			}
		}

		public Vector3 A
		{
			get
			{
				return this._a;
			}
			set
			{
				this._a = value;
			}
		}

		public Vector3 B
		{
			get
			{
				return this._b;
			}
			set
			{
				this._b = value;
			}
		}

		public Vector3 C
		{
			get
			{
				return this._c;
			}
			set
			{
				this._c = value;
			}
		}

		public Vector3 Centroid
		{
			get
			{
				return (this._a + this._b + this._c) / 3f;
			}
		}

		public float Area
		{
			get
			{
				Vector3 vector = this._a - this._b;
				Vector3 vector2 = this._c - this._b;
				return Vector3.Cross(vector, vector2).Length();
			}
		}

		public BoundingBox GetBoundingBox()
		{
			Vector3 min = new Vector3(Math.Min(Math.Min(this._a.X, this._b.X), this._c.X), Math.Min(Math.Min(this._a.Y, this._b.Y), this._c.Y), Math.Min(Math.Min(this._a.Z, this._b.Z), this._c.Z));
			Vector3 max = new Vector3(Math.Max(Math.Max(this._a.X, this._b.X), this._c.X), Math.Max(Math.Max(this._a.Y, this._b.Y), this._c.Y), Math.Max(Math.Max(this._a.Z, this._b.Z), this._c.Z));
			return new BoundingBox(min, max);
		}

		public BoundingSphere GetBoundingSphere()
		{
			BoundingBox boundingBox = this.GetBoundingBox();
			Vector3 vector = boundingBox.Min + (boundingBox.Max - boundingBox.Min) / 2f;
			float val = (this._a - vector).LengthSquared();
			float val2 = (this._b - vector).LengthSquared();
			float val3 = (this._c - vector).LengthSquared();
			float radius = (float)Math.Sqrt((double)Math.Max(Math.Max(val, val2), val3));
			return new BoundingSphere(vector, radius);
		}

		public static Triangle3D Transform(Triangle3D triangle, Matrix matrix)
		{
			return new Triangle3D(Vector3.Transform(triangle._a, matrix), Vector3.Transform(triangle._b, matrix), Vector3.Transform(triangle._c, matrix));
		}

		public static Triangle3D Read(BinaryReader reader)
		{
			return new Triangle3D(reader.ReadVector3(), reader.ReadVector3(), reader.ReadVector3());
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write(this.A);
			writer.Write(this.B);
			writer.Write(this.C);
		}

		public Triangle3D(Vector3 a, Vector3 b, Vector3 c)
		{
			this._a = a;
			this._b = b;
			this._c = c;
		}

		public Vector3 BarycentricCoordinate(Vector3 point)
		{
			Vector3 result = default(Vector3);
			Vector3 vector = this._b - this._a;
			Vector3 vector2 = this._c - this._a;
			Vector3 vector3 = point - this._a;
			Vector3 vector4 = Vector3.Cross(vector, vector3);
			Vector3 vector5 = Vector3.Cross(vector2, vector3);
			float num = 1f / Vector3.Cross(vector, vector2).Length();
			result.Y = vector5.Length() * num;
			result.Z = vector4.Length() * num;
			result.X = 1f - result.Y - result.Z;
			return result;
		}

		public Plane GetPlane()
		{
			return new Plane(this.B, this.A, this.C);
		}

		public float? Intersects(Ray ray)
		{
			Vector3 vector = this._b - this._a;
			Vector3 vector2 = this._c - this._a;
			Vector3 vector3 = Vector3.Cross(vector, vector2);
			Vector3 direction = ray.Direction;
			Vector3 vector4 = ray.Position - this._a;
			float num = -Vector3.Dot(vector3, vector4);
			float num2 = Vector3.Dot(vector3, direction);
			if (num2 == 0f)
			{
				return null;
			}
			float num3 = num / num2;
			if (num3 < 0f)
			{
				return null;
			}
			Vector3 value = ray.Position + num3 * direction;
			float num4 = Vector3.Dot(vector, vector);
			float num5 = Vector3.Dot(vector, vector2);
			float num6 = Vector3.Dot(vector2, vector2);
			Vector3 vector5 = value - this._a;
			float num7 = Vector3.Dot(vector5, vector);
			float num8 = Vector3.Dot(vector5, vector2);
			float num9 = num5 * num5 - num4 * num6;
			float num10 = (num5 * num8 - num6 * num7) / num9;
			if ((double)num10 < 0.0 || (double)num10 > 1.0)
			{
				return null;
			}
			float num11 = (num5 * num7 - num4 * num8) / num9;
			if ((double)num11 < 0.0 || (double)(num10 + num11) > 1.0)
			{
				return null;
			}
			return new float?(num3);
		}

		public Triangle3D[] SliceHorizontal(float yValue, int precisionDigits)
		{
			Plane plane = DrawingTools.PlaneFromPointNormal(new Vector3(0f, yValue, 0f), new Vector3(0f, 1f, 0f));
			LineF3D ab = this.AB;
			LineF3D ac = this.AC;
			LineF3D bc = this.BC;
			float num;
			bool flag;
			if (ab.Intersects(plane, out num, out flag, precisionDigits))
			{
				if (flag)
				{
					return new Triangle3D[]
					{
						this
					};
				}
				if (num == 0f)
				{
					float num2;
					if (!bc.Intersects(plane, out num2, out flag, precisionDigits))
					{
						return new Triangle3D[]
						{
							this
						};
					}
					if (num2 == 0f || num2 == 1f)
					{
						return new Triangle3D[]
						{
							this
						};
					}
					Vector3 value = bc.GetValue(num2);
					value.Y = yValue;
					return new Triangle3D[]
					{
						new Triangle3D(this._a, this._b, value),
						new Triangle3D(this._a, value, this._c)
					};
				}
				else if (num == 1f)
				{
					float num2;
					if (!ac.Intersects(plane, out num2, out flag, precisionDigits))
					{
						return new Triangle3D[]
						{
							this
						};
					}
					Vector3 value = ac.GetValue(num2);
					value.Y = yValue;
					if (num2 == 0f || num2 == 1f)
					{
						return new Triangle3D[]
						{
							new Triangle3D(ac.GetValue(0f), this._b, value)
						};
					}
					return new Triangle3D[]
					{
						new Triangle3D(this._b, this._c, value),
						new Triangle3D(this._b, value, this._a)
					};
				}
				else
				{
					Vector3 value2 = ab.GetValue(num);
					value2.Y = yValue;
					float num2;
					if (ac.Intersects(plane, out num2, out flag, precisionDigits))
					{
						if (num2 == 1f)
						{
							return new Triangle3D[]
							{
								new Triangle3D(value2, this._b, this._c),
								new Triangle3D(value2, this._c, this._a)
							};
						}
						Vector3 value = ac.GetValue(num2);
						value.Y = yValue;
						return new Triangle3D[]
						{
							new Triangle3D(value, this._a, value2),
							new Triangle3D(value2, this._b, this._c),
							new Triangle3D(value, value2, this._c)
						};
					}
					else
					{
						if (bc.Intersects(plane, out num2, out flag, precisionDigits))
						{
							Vector3 value = bc.GetValue(num2);
							value.Y = yValue;
							return new Triangle3D[]
							{
								new Triangle3D(value2, this._b, value),
								new Triangle3D(this._a, value2, value),
								new Triangle3D(this._a, value, this._c)
							};
						}
						throw new Exception("Slice Error");
					}
				}
			}
			else
			{
				if (!bc.Intersects(plane, out num, out flag, precisionDigits))
				{
					return new Triangle3D[]
					{
						this
					};
				}
				if (num == 1f)
				{
					return new Triangle3D[]
					{
						this
					};
				}
				Vector3 value2 = bc.GetValue(num);
				value2.Y = yValue;
				float num2;
				if (ac.Intersects(plane, out num2, out flag, precisionDigits))
				{
					Vector3 value = ac.GetValue(num2);
					value.Y = yValue;
					return new Triangle3D[]
					{
						new Triangle3D(value, this._a, value2),
						new Triangle3D(value2, this._b, this._a),
						new Triangle3D(value, value2, this._c)
					};
				}
				throw new Exception("Slice Error");
			}
		}

		public Triangle3D[] SliceVertical(float xValue, int precisionDigits)
		{
			Plane plane = DrawingTools.PlaneFromPointNormal(new Vector3(xValue, 0f, 0f), new Vector3(1f, 0f, 0f));
			LineF3D ab = this.AB;
			LineF3D ac = this.AC;
			LineF3D bc = this.BC;
			float num;
			bool flag;
			if (ab.Intersects(plane, out num, out flag, precisionDigits))
			{
				if (flag)
				{
					return new Triangle3D[]
					{
						this
					};
				}
				if (num == 0f)
				{
					float num2;
					if (!bc.Intersects(plane, out num2, out flag, precisionDigits))
					{
						return new Triangle3D[]
						{
							this
						};
					}
					if (num2 == 0f || num2 == 1f)
					{
						return new Triangle3D[]
						{
							this
						};
					}
					Vector3 value = bc.GetValue(num2);
					value.X = xValue;
					return new Triangle3D[]
					{
						new Triangle3D(this._a, this._b, value),
						new Triangle3D(this._a, value, this._c)
					};
				}
				else if (num == 1f)
				{
					float num2;
					if (!ac.Intersects(plane, out num2, out flag, precisionDigits))
					{
						return new Triangle3D[]
						{
							this
						};
					}
					Vector3 value = ac.GetValue(num2);
					value.X = xValue;
					if (num2 == 0f || num2 == 1f)
					{
						return new Triangle3D[]
						{
							new Triangle3D(ac.GetValue(0f), this._b, value)
						};
					}
					return new Triangle3D[]
					{
						new Triangle3D(this._b, this._c, value),
						new Triangle3D(this._b, value, this._a)
					};
				}
				else
				{
					Vector3 value2 = ab.GetValue(num);
					value2.X = xValue;
					float num2;
					if (ac.Intersects(plane, out num2, out flag, precisionDigits))
					{
						if (num2 == 1f)
						{
							return new Triangle3D[]
							{
								new Triangle3D(value2, this._b, this._c),
								new Triangle3D(value2, this._c, this._a)
							};
						}
						Vector3 value = ac.GetValue(num2);
						value.X = xValue;
						return new Triangle3D[]
						{
							new Triangle3D(value, this._a, value2),
							new Triangle3D(value2, this._b, this._c),
							new Triangle3D(value, value2, this._c)
						};
					}
					else
					{
						if (bc.Intersects(plane, out num2, out flag, precisionDigits))
						{
							Vector3 value = bc.GetValue(num2);
							value.X = xValue;
							return new Triangle3D[]
							{
								new Triangle3D(value2, this._b, value),
								new Triangle3D(this._a, value2, value),
								new Triangle3D(this._a, value, this._c)
							};
						}
						throw new Exception("Slice Error");
					}
				}
			}
			else
			{
				if (!bc.Intersects(plane, out num, out flag, precisionDigits))
				{
					return new Triangle3D[]
					{
						this
					};
				}
				if (num == 1f)
				{
					return new Triangle3D[]
					{
						this
					};
				}
				Vector3 value2 = bc.GetValue(num);
				value2.X = xValue;
				float num2;
				if (ac.Intersects(plane, out num2, out flag, precisionDigits))
				{
					Vector3 value = ac.GetValue(num2);
					value.X = xValue;
					return new Triangle3D[]
					{
						new Triangle3D(value, this._a, value2),
						new Triangle3D(value2, this._b, this._a),
						new Triangle3D(value, value2, this._c)
					};
				}
				throw new Exception("Slice Error");
			}
		}

		public Triangle3D[] Slice(Plane plane, int precisionDigits)
		{
			plane.DotCoordinate(this.A);
			plane.DotCoordinate(this.B);
			plane.DotCoordinate(this.C);
			LineF3D ab = this.AB;
			LineF3D ac = this.AC;
			LineF3D bc = this.BC;
			float num;
			bool flag;
			if (ab.Intersects(plane, out num, out flag, precisionDigits))
			{
				if (flag)
				{
					return new Triangle3D[]
					{
						this
					};
				}
				if (num == 0f)
				{
					float num2;
					if (!bc.Intersects(plane, out num2, out flag, precisionDigits))
					{
						return new Triangle3D[]
						{
							this
						};
					}
					if (flag)
					{
						throw new PrecisionException();
					}
					if (num2 == 0f || num2 == 1f)
					{
						return new Triangle3D[]
						{
							this
						};
					}
					Vector3 value = bc.GetValue(num2);
					return new Triangle3D[]
					{
						new Triangle3D(this._a, this._b, value),
						new Triangle3D(this._a, value, this._c)
					};
				}
				else if (num == 1f)
				{
					float num2;
					if (!ac.Intersects(plane, out num2, out flag, precisionDigits))
					{
						return new Triangle3D[]
						{
							this
						};
					}
					if (flag)
					{
						throw new PrecisionException();
					}
					Vector3 value = ac.GetValue(num2);
					if (num2 == 0f || num2 == 1f)
					{
						return new Triangle3D[]
						{
							new Triangle3D(ac.GetValue(0f), this._b, value)
						};
					}
					return new Triangle3D[]
					{
						new Triangle3D(this._b, this._c, value),
						new Triangle3D(this._b, value, this._a)
					};
				}
				else
				{
					Vector3 value2 = ab.GetValue(num);
					float num2;
					if (ac.Intersects(plane, out num2, out flag, precisionDigits))
					{
						if (num2 <= 0f || flag)
						{
							throw new PrecisionException();
						}
						if (num2 == 1f)
						{
							return new Triangle3D[]
							{
								new Triangle3D(value2, this._b, this._c),
								new Triangle3D(value2, this._c, this._a)
							};
						}
						Vector3 value = ac.GetValue(num2);
						return new Triangle3D[]
						{
							new Triangle3D(value, this._a, value2),
							new Triangle3D(value2, this._b, this._c),
							new Triangle3D(value, value2, this._c)
						};
					}
					else
					{
						if (!bc.Intersects(plane, out num2, out flag, precisionDigits))
						{
							throw new PrecisionException();
						}
						if (num2 <= 0f || num2 >= 1f || flag)
						{
							throw new PrecisionException();
						}
						Vector3 value = bc.GetValue(num2);
						return new Triangle3D[]
						{
							new Triangle3D(value2, this._b, value),
							new Triangle3D(this._a, value2, value),
							new Triangle3D(this._a, value, this._c)
						};
					}
				}
			}
			else
			{
				if (!bc.Intersects(plane, out num, out flag, precisionDigits))
				{
					return new Triangle3D[]
					{
						this
					};
				}
				if (num == 0f || flag)
				{
					throw new PrecisionException();
				}
				if (num == 1f)
				{
					return new Triangle3D[]
					{
						this
					};
				}
				Vector3 value2 = bc.GetValue(num);
				float num2;
				if (!ac.Intersects(plane, out num2, out flag, precisionDigits))
				{
					throw new PrecisionException();
				}
				if (num2 <= 0f || num2 >= 1f || flag)
				{
					throw new PrecisionException();
				}
				Vector3 value = ac.GetValue(num2);
				return new Triangle3D[]
				{
					new Triangle3D(value, this._a, value2),
					new Triangle3D(value2, this._a, this._b),
					new Triangle3D(value, value2, this._c)
				};
			}
		}

		public override string ToString()
		{
			return string.Concat(new string[]
			{
				this.A.ToString(),
				"-",
				this.B.ToString(),
				"-",
				this.C.ToString()
			});
		}
	}
}
