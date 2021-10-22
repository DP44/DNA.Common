using System;
using System.Collections.Generic;
using System.IO;
using DNA.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing.Collision
{
	public class SphereTree : CollisionMap
	{
		public class SphereTreeReader : ContentTypeReader<SphereTree>
		{
			protected override SphereTree Read(ContentReader input, SphereTree existingInstance)
			{
				if (existingInstance != null)
				{
					existingInstance.Load(input);
					return existingInstance;
				}
				SphereTree sphereTree = new SphereTree();
				sphereTree.Load(input);
				return sphereTree;
			}
		}

		public class SphereTreeNode
		{
			public BoundingSphere Sphere;

			public SphereTree.SphereTreeNode[] Children = new SphereTree.SphereTreeNode[8];

			public List<Triangle3D> Triangles = new List<Triangle3D>();

			public static int RayTriangleTests;

			public bool IsLeaf
			{
				get
				{
					for (int i = 0; i < this.Children.Length; i++)
					{
						if (this.Children[i] != null)
						{
							return false;
						}
					}
					return true;
				}
			}

			public SphereTreeNode()
			{
			}

			public static SphereTree.SphereTreeNode Load(BinaryReader reader)
			{
				Vector3 center = reader.ReadVector3();
				float radius = reader.ReadSingle();
				SphereTree.SphereTreeNode sphereTreeNode = new SphereTree.SphereTreeNode(new BoundingSphere(center, radius));
				int num = reader.ReadInt32();
				sphereTreeNode.Triangles = new List<Triangle3D>(num);
				for (int i = 0; i < num; i++)
				{
					sphereTreeNode.Triangles.Add(Triangle3D.Read(reader));
				}
				byte b = reader.ReadByte();
				for (int j = 0; j < 8; j++)
				{
					if (((int)b & 1 << j) != 0)
					{
						sphereTreeNode.Children[j] = SphereTree.SphereTreeNode.Load(reader);
					}
				}
				return sphereTreeNode;
			}

			public void Save(BinaryWriter writer)
			{
				writer.Write(this.Sphere.Center);
				writer.Write(this.Sphere.Radius);
				writer.Write(this.Triangles.Count);
				foreach (Triangle3D triangle3D in this.Triangles)
				{
					triangle3D.Write(writer);
				}
				byte b = 0;
				for (int i = 0; i < 8; i++)
				{
					if (this.Children[i] != null)
					{
						b |= (byte)(1 << i);
					}
				}
				writer.Write(b);
				for (int j = 0; j < 8; j++)
				{
					if (this.Children[j] != null)
					{
						this.Children[j].Save(writer);
					}
				}
			}

			public SphereTreeNode(BoundingSphere sphere, IList<Triangle3D> tris)
			{
				this.Sphere = sphere;
				this.Triangles.AddRange(tris);
			}

			public SphereTreeNode(BoundingSphere sphere, Triangle3D tri)
			{
				this.Sphere = sphere;
				this.Triangles.Add(tri);
			}

			public SphereTreeNode(BoundingSphere sphere)
			{
				this.Sphere = sphere;
			}

			public void FindCollisions(BoundingSphere sphere, List<CollisionMap.ContactPoint> contacts)
			{
				if (!this.Sphere.Intersects(sphere))
				{
					return;
				}
				for (int i = 0; i < this.Children.Length; i++)
				{
					if (this.Children[i] != null)
					{
						this.Children[i].FindCollisions(sphere, contacts);
					}
				}
				for (int j = 0; j < this.Triangles.Count; j++)
				{
					Triangle3D tri = this.Triangles[j];
					Plane plane = tri.GetPlane();
					Vector3 normal = tri.Normal;
					float num = Math.Abs(plane.DotCoordinate(sphere.Center));
					Vector3 vector = sphere.Center - tri.A;
					Vector3.Dot(normal, vector);
					Vector3 value = num * normal;
					Vector3 value2 = sphere.Center + value;
					Vector3 vector2 = tri.B - tri.A;
					Vector3 vector3 = tri.C - tri.A;
					float num2 = Vector3.Dot(vector2, vector2);
					float num3 = Vector3.Dot(vector2, vector3);
					float num4 = Vector3.Dot(vector3, vector3);
					Vector3 vector4 = value2 - tri.A;
					float num5 = Vector3.Dot(vector4, vector2);
					float num6 = Vector3.Dot(vector4, vector3);
					float num7 = num3 * num3 - num2 * num4;
					float num8 = (num3 * num6 - num4 * num5) / num7;
					if ((double)num8 >= 0.0 && (double)num8 <= 1.0)
					{
						float num9 = (num3 * num5 - num2 * num6) / num7;
						if ((double)num9 >= 0.0 && (double)(num8 + num9) <= 1.0 && num <= sphere.Radius)
						{
							Vector3 dir = (sphere.Radius - num) * normal;
							contacts.Add(new CollisionMap.ContactPoint(dir, tri));
						}
					}
				}
			}

			public void FindCollisions(BoundingSphere sphere, Ellipsoid ellipsoid, List<CollisionMap.ContactPoint> contacts)
			{
				if (!this.Sphere.Intersects(sphere))
				{
					return;
				}
				for (int i = 0; i < this.Children.Length; i++)
				{
					if (this.Children[i] != null)
					{
						this.Children[i].FindCollisions(sphere, ellipsoid, contacts);
					}
				}
				Triangle3D triangle3D = default(Triangle3D);
				for (int j = 0; j < this.Triangles.Count; j++)
				{
					Plane plane = this.Triangles[j].GetPlane();
					ellipsoid.TransformWorldSpaceTriToUnitSphereSpace(this.Triangles[j], ref triangle3D);
					Plane plane2 = ellipsoid.TransformWorldSpacePlaneToUnitSphereSpace(plane);
					float d = plane2.D;
					if (d <= 1f)
					{
						Vector3.Dot(plane2.Normal, triangle3D.A);
						Vector3 value = -d * plane2.Normal;
						Vector3 vector = triangle3D.B - triangle3D.A;
						Vector3 vector2 = triangle3D.C - triangle3D.A;
						float num = Vector3.Dot(vector, vector);
						float num2 = Vector3.Dot(vector, vector2);
						float num3 = Vector3.Dot(vector2, vector2);
						Vector3 vector3 = value - triangle3D.A;
						float num4 = Vector3.Dot(vector3, vector);
						float num5 = Vector3.Dot(vector3, vector2);
						float num6 = num2 * num2 - num * num3;
						float num7 = (num2 * num5 - num3 * num4) / num6;
						if ((double)num7 >= 0.0 && (double)num7 <= 1.0)
						{
							float num8 = (num2 * num4 - num * num5) / num6;
							if ((double)num8 >= 0.0 && (double)(num7 + num8) <= 1.0)
							{
								contacts.Add(new CollisionMap.ContactPoint(plane2.Normal, this.Triangles[j]));
								return;
							}
						}
						Vector3 vector4 = Vector3.Zero;
						LineF3D lineF3D = new LineF3D(triangle3D.A, triangle3D.B);
						LineF3D lineF3D2 = new LineF3D(triangle3D.A, triangle3D.C);
						LineF3D lineF3D3 = new LineF3D(triangle3D.B, triangle3D.C);
						Vector3 vector5 = lineF3D.ClosetPointTo(Vector3.Zero);
						Vector3 vector6 = lineF3D2.ClosetPointTo(Vector3.Zero);
						Vector3 vector7 = lineF3D3.ClosetPointTo(Vector3.Zero);
						float num9 = vector5.LengthSquared();
						float num10 = vector6.LengthSquared();
						float num11 = vector7.LengthSquared();
						bool flag = false;
						float num12 = 1f;
						if (num9 <= num12)
						{
							num12 = num9;
							vector4 = vector5;
							flag = true;
						}
						if (num10 <= num12)
						{
							num12 = num10;
							vector4 = vector6;
							flag = true;
						}
						if (num11 <= num12)
						{
							vector4 = vector7;
							flag = true;
						}
						if (flag)
						{
							vector4 = ellipsoid.TransformUnitSphereSpaceVectorToWorldSpace(Vector3.Negate(vector4));
							contacts.Add(new CollisionMap.ContactPoint(vector4, this.Triangles[j]));
						}
					}
				}
			}

			public static bool RaySphereIntersection(BoundingSphere sphere, Ray ray, out float t1out, out float t2out)
			{
				Vector3 vector = ray.Position - sphere.Center;
				float num = Vector3.Dot(vector, vector) - sphere.Radius * sphere.Radius;
				float num2 = Vector3.Dot(ray.Direction, vector);
				t1out = float.MinValue;
				t2out = float.MaxValue;
				float num3;
				if (num <= 0f)
				{
					num3 = num2 * num2 - num;
					float num4 = (float)Math.Sqrt((double)num3);
					t1out = float.MinValue;
					t2out = -num2 + num4;
					return true;
				}
				if (num2 >= 0f)
				{
					return false;
				}
				num3 = num2 * num2 - num;
				if (num3 < 0f)
				{
					return false;
				}
				if (num3 > 0f)
				{
					float num4 = (float)Math.Sqrt((double)num3);
					t1out = -num2 - num4;
					t2out = -num2 + num4;
					if (t1out > t2out)
					{
						float num5 = t1out;
						t1out = t2out;
						t2out = num5;
					}
					return true;
				}
				t2out = (t1out = -num2);
				return true;
			}

			public CollisionMap.RayQueryResult? CollidesWith(Ray ray, float minT, float maxT)
			{
				float num;
				float num2;
				if (!SphereTree.SphereTreeNode.RaySphereIntersection(this.Sphere, ray, out num, out num2))
				{
					return null;
				}
				if (num >= 0f && num > maxT)
				{
					return null;
				}
				CollisionMap.RayQueryResult? result = null;
				for (int i = 0; i < this.Children.Length; i++)
				{
					if (this.Children[i] != null)
					{
						CollisionMap.RayQueryResult? rayQueryResult = this.Children[i].CollidesWith(ray, minT, maxT);
						if (rayQueryResult != null && rayQueryResult.Value.T < maxT)
						{
							maxT = rayQueryResult.Value.T;
							result = rayQueryResult;
						}
					}
				}
				for (int j = 0; j < this.Triangles.Count; j++)
				{
					SphereTree.SphereTreeNode.RayTriangleTests++;
					float? num3 = this.Triangles[j].Intersects(ray);
					if (num3 != null)
					{
						float value = num3.Value;
						if (value >= minT && value < maxT)
						{
							maxT = value;
							result = new CollisionMap.RayQueryResult?(new CollisionMap.RayQueryResult(value, this.Triangles[j]));
						}
					}
				}
				return result;
			}

			public void RefineBoundingSphere()
			{
				List<Vector3> points = new List<Vector3>();
				this.GetPoints(points);
				BoundingSphere sphere = BoundingSphere.CreateFromPoints(points);
				this.Sphere = sphere;
			}

			public void GetPoints(List<Vector3> points)
			{
				foreach (Triangle3D triangle3D in this.Triangles)
				{
					points.Add(triangle3D.A);
					points.Add(triangle3D.B);
					points.Add(triangle3D.C);
				}
				foreach (SphereTree.SphereTreeNode sphereTreeNode in this.Children)
				{
					if (sphereTreeNode != null)
					{
						sphereTreeNode.GetPoints(points);
					}
				}
			}

			public void GetTriangles(List<Triangle3D> tris)
			{
				tris.AddRange(this.Triangles);
				foreach (SphereTree.SphereTreeNode sphereTreeNode in this.Children)
				{
					if (sphereTreeNode != null)
					{
						sphereTreeNode.GetTriangles(tris);
					}
				}
			}
		}

		private const float mergeBias = 0.5f;

		private const int GridSize = 128;

		private const int Version = 1;

		public SphereTree.SphereTreeNode _root;

		private readonly string FileID = "TASSPT";

		public SphereTree(Model model)
		{
			List<Triangle3D> tris = model.ExtractModelTris(false);
			this.Build(tris);
		}

		public SphereTree()
		{
			this._root = new SphereTree.SphereTreeNode();
		}

		public void Build(IList<Triangle3D> tris)
		{
			List<Vector3> list = new List<Vector3>();
			for (int i = 0; i < tris.Count; i++)
			{
				list.Add(tris[i].A);
				list.Add(tris[i].B);
				list.Add(tris[i].C);
			}
			BoundingBox boundingBox = BoundingBox.CreateFromPoints(list);
			Vector3 value = boundingBox.Max - boundingBox.Min;
			float num = Math.Max(value.X, Math.Max(value.Y, value.Z));
			Vector3 value2 = boundingBox.Min + value / 2f;
			Vector3 value3 = new Vector3(num, num, num);
			new BoundingBox(value2 - value3 / 2f, value2 + value3 / 2f);
			float num2 = num / 128f;
			int num3 = 128;
			SphereTree.SphereTreeNode[,,] array = new SphereTree.SphereTreeNode[num3, num3, num3];
			List<SphereTree.SphereTreeNode> list2 = new List<SphereTree.SphereTreeNode>();
			foreach (Triangle3D item in tris)
			{
				Vector3 vector = item.Centroid - boundingBox.Min;
				int num4 = (int)(vector.X / num2);
				int num5 = (int)(vector.Y / num2);
				int num6 = (int)(vector.Z / num2);
				if (array[num4, num5, num6] == null)
				{
					list2.Add(array[num4, num5, num6] = new SphereTree.SphereTreeNode());
				}
				array[num4, num5, num6].Triangles.Add(item);
			}
			foreach (SphereTree.SphereTreeNode sphereTreeNode in list2)
			{
				sphereTreeNode.RefineBoundingSphere();
			}
			SphereTree.SphereTreeNode[,,] array2;
			do
			{
				num3 /= 2;
				array2 = new SphereTree.SphereTreeNode[num3, num3, num3];
				list2.Clear();
				for (int j = 0; j < num3; j++)
				{
					for (int k = 0; k < num3; k++)
					{
						for (int l = 0; l < num3; l++)
						{
							SphereTree.SphereTreeNode sphereTreeNode2 = new SphereTree.SphereTreeNode();
							bool flag = true;
							for (int m = 0; m < 8; m++)
							{
								sphereTreeNode2.Children[m] = array[j * 2 + ((m & 4) >> 2), k * 2 + ((m & 2) >> 1), l * 2 + (m & 1)];
								if (sphereTreeNode2.Children[m] != null)
								{
									flag = false;
								}
							}
							if (!flag)
							{
								array2[j, k, l] = sphereTreeNode2;
								list2.Add(sphereTreeNode2);
							}
						}
					}
				}
				foreach (SphereTree.SphereTreeNode sphereTreeNode3 in list2)
				{
					sphereTreeNode3.RefineBoundingSphere();
				}
				array = array2;
			}
			while (num3 > 1);
			this._root = array2[0, 0, 0];
		}

		public override void FindCollisions(Ellipsoid ellipsoid, List<CollisionMap.ContactPoint> contacts)
		{
			contacts.Clear();
			BoundingSphere boundingSphere = ellipsoid.GetBoundingSphere();
			this._root.FindCollisions(boundingSphere, ellipsoid, contacts);
		}

		public override void FindCollisions(BoundingSphere sphere, List<CollisionMap.ContactPoint> contacts)
		{
			contacts.Clear();
			this._root.FindCollisions(sphere, contacts);
		}

		public override void GetTriangles(List<Triangle3D> tris)
		{
			tris.Clear();
			this._root.GetTriangles(tris);
		}

		public override void Save(BinaryWriter writer)
		{
			writer.Write(this.FileID);
			writer.Write(1);
			this._root.Save(writer);
		}

		public override void Load(BinaryReader reader)
		{
			string a = reader.ReadString();
			if (a != this.FileID)
			{
				throw new Exception("Bad SPT file Format");
			}
			int num = reader.ReadInt32();
			if (num != 1)
			{
				throw new Exception("Bad SPT version");
			}
			this._root = SphereTree.SphereTreeNode.Load(reader);
		}

		protected override CollisionMap.RayQueryResult? CollidesWith(Ray ray, float min, float max)
		{
			return this._root.CollidesWith(ray, min, max);
		}
	}
}
