using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing.Collision
{
	public class BSPTree : CollisionMap
	{
		public class BSPTreeReader : ContentTypeReader<BSPTree>
		{
			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			protected override BSPTree Read(ContentReader input, BSPTree existingInstance)
			{
				if (existingInstance != null)
				{
					existingInstance.Load(input);
					return existingInstance;
				}

				BSPTree bsptree = new BSPTree();
				bsptree.Load(input);
				return bsptree;
			}
		}

		private class BSPTreeNode
		{
			private class SplitCounts
			{
				public int Node;
				public int Positive;
				public int Negative;
				public int Spanning;
				public int Coincident;

				/// <summary>
				/// 
				/// </summary>
				public float DivisionFactor
				{
					get
					{
						if (this.Positive > this.Negative)
						{
							return (float)this.Negative / (float)this.Positive;
						}
						
						if (this.Positive < this.Negative)
						{
							return (float)this.Positive / (float)this.Negative;
						}
						
						return 1f;
					}
				}

				/// <summary>
				/// 
				/// </summary>
				/// <param name=""></param>
				public SplitCounts(int node, int positive, int negative, int spanning, int coincident)
				{
					this.Node = node;
					this.Positive = positive;
					this.Negative = negative;
					this.Spanning = spanning;
					this.Coincident = coincident;
				}
			}

			private class BestSplitFinder
			{
				private int startPoly;
				private int endPoly;
				private float minRelation;
				public float bestRatio = float.MinValue;
				public int leastSplits = int.MaxValue;
				public BSPTree.BSPTreeNode.SplitCounts bestPolygon;
				public bool isConvex;
				private Triangle3D[] triList;
				private Thread _thread;

			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
				public BestSplitFinder(Triangle3D[] tlist, float minr, int sPoly, int epoly)
				{
					this.triList = tlist;
					this.minRelation = minr;
					this.startPoly = sPoly;
					this.endPoly = epoly;
				}

				/// <summary>
				/// 
				/// </summary>
				public void FindSplitAsync()
				{
					if (this.startPoly == 0 && this.endPoly >= this.triList.Length)
					{
						this.FindSplit();
						return;
					}
					this._thread = new Thread(new ThreadStart(this.FindSplit));
					this._thread.Name = "Find Poly Split Worker " + this.startPoly.ToString() + "-" + this.endPoly.ToString();
					this._thread.Start();
				}

				/// <summary>
				/// 
				/// </summary>
				public void EndFindSplit()
				{
					if (this._thread != null)
					{
						this._thread.Join();
					}
				}

				/// <summary>
				/// 
				/// </summary>
				public void FindSplit()
				{
					for (int i = this.startPoly; i < this.endPoly; i++)
					{
						int num = 0;
						int num2 = 0;
						int num3 = 0;
						int num4 = 0;

						Plane plane = this.triList[i].GetPlane();

						for (int j = 0; j < this.triList.Length; j++)
						{
							if (i != j)
							{
								Triangle3D triangle3D = this.triList[j];
								int num5 = 0;
								int num6 = 0;
								float num7 = plane.DotCoordinate(triangle3D.A);
								
								if (num7 > 0f)
								{
									num5++;
								}
								else if (num7 < 0f)
								{
									num6++;
								}
								
								num7 = plane.DotCoordinate(triangle3D.B);
								
								if (num7 > 0f)
								{
									num5++;
								}
								else if (num7 < 0f)
								{
									num6++;
								}
								
								num7 = plane.DotCoordinate(triangle3D.C);
								
								if (num7 > 0f)
								{
									num5++;
								}
								else if (num7 < 0f)
								{
									num6++;
								}
								
								if (num5 > 0 && num6 == 0)
								{
									num++;
								}
								else if (num5 == 0 && num6 > 0)
								{
									num2++;
								}
								else if (num5 != 0 || num6 != 0)
								{
									num3++;
								}
								else
								{
									num4++;
								}
							}
						}

						BSPTree.BSPTreeNode.SplitCounts splitCounts = new BSPTree.BSPTreeNode.SplitCounts(i, num, num2, num3, num4);
						
						if (num2 > 0 || num3 > 0)
						{
							this.isConvex = false;
						}
						
						if (splitCounts.DivisionFactor >= this.minRelation && (splitCounts.Spanning < this.leastSplits || (splitCounts.Spanning == this.leastSplits && splitCounts.DivisionFactor > this.bestRatio)))
						{
							this.bestPolygon = splitCounts;
							this.leastSplits = splitCounts.Spanning;
							this.bestRatio = splitCounts.DivisionFactor;
						}
					}
				}
			}

			public BSPTree.BSPTreeNode Front;
			public BSPTree.BSPTreeNode Back;
			public Plane SplitPlane;
			public Triangle3D[] TriList;
			public static int AvailibleThread = (Environment.ProcessorCount - 1) * 15;
			public static object threadLock = new object();

			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			private static void ChooseDividingPolygon(LinkedList<Triangle3D> splitList)
			{
				if (splitList.Count < 3)
				{
					return;
				}
				
				LinkedListNode<Triangle3D>[] array = new LinkedListNode<Triangle3D>[splitList.Count];
				Triangle3D[] array2 = new Triangle3D[splitList.Count];
				LinkedListNode<Triangle3D> linkedListNode = splitList.First;
				
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i] = linkedListNode.Value;
					array[i] = linkedListNode;
					linkedListNode = linkedListNode.Next;
				}
				
				int num = Environment.ProcessorCount;
				
				if (array2.Length < num * 500)
				{
					num = 1;
				}
				
				int num2 = array2.Length / num + 1;
				BSPTree.BSPTreeNode.SplitCounts splitCounts = null;
				float num3 = 0.8f;
				bool flag = true;
				
				while (splitCounts == null)
				{
					float num4 = float.MinValue;
					int num5 = int.MaxValue;
					List<BSPTree.BSPTreeNode.BestSplitFinder> list = new List<BSPTree.BSPTreeNode.BestSplitFinder>();
					int num6 = 0;
					
					for (int j = 0; j < num; j++)
					{
						int num7 = num6 + num2;
						
						if (num7 > array2.Length)
						{
							num7 = array2.Length;
						}
						
						BSPTree.BSPTreeNode.BestSplitFinder bestSplitFinder = new BSPTree.BSPTreeNode.BestSplitFinder(array2, num3, num6, num7);
						bestSplitFinder.FindSplitAsync();
						list.Add(bestSplitFinder);
						num6 = num7 + 1;
					}
					
					foreach (BSPTree.BSPTreeNode.BestSplitFinder bestSplitFinder2 in list)
					{
						bestSplitFinder2.EndFindSplit();
						
						if (!bestSplitFinder2.isConvex)
						{
							flag = false;
						}
						
						if (bestSplitFinder2.bestPolygon != null)
						{
							BSPTree.BSPTreeNode.SplitCounts bestPolygon = bestSplitFinder2.bestPolygon;
							
							if (bestPolygon.Spanning < num5 || (bestPolygon.Spanning == num5 && bestPolygon.DivisionFactor > num4))
							{
								splitCounts = bestPolygon;
								num5 = bestPolygon.Spanning;
								num4 = bestPolygon.DivisionFactor;
							}
						}
					}

					num3 /= 2f;
					
					if (flag)
					{
						return;
					}
				}

				splitList.Remove(array[splitCounts.Node]);
				splitList.AddFirst(array[splitCounts.Node]);
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			public static void SortList(Plane splitPlane, LinkedList<Triangle3D> triList, LinkedList<Triangle3D> frontList, LinkedList<Triangle3D> backList, int percisionDigits)
			{
				LinkedListNode<Triangle3D> linkedListNode = triList.First;
				
				while (linkedListNode != null)
				{
					int num = percisionDigits;
					bool flag = true;
					
					while (flag)
					{
						bool flag2 = false;
						
						Triangle3D value = linkedListNode.Value;
						
						double num2 = (double)splitPlane.DotCoordinate(value.A);
						double num3 = (double)splitPlane.DotCoordinate(value.B);
						double num4 = (double)splitPlane.DotCoordinate(value.C);
						
						num2 = Math.Round(num2, num);
						num3 = Math.Round(num3, num);
						num4 = Math.Round(num4, num);
						
						if (num2 == 0.0 && num3 == 0.0 && num4 == 0.0)
						{
							Plane plane = new Plane(value.B, value.A, value.C);
							
							if (Vector3.Dot(splitPlane.Normal, plane.Normal) > 0f)
							{
								frontList.AddFirst(value);
							}
							else
							{
								backList.AddLast(value);
							}
							
							flag2 = true;
						}
						else if (num2 <= 0.0 && num3 <= 0.0 && num4 <= 0.0)
						{
							flag2 = true;
							backList.AddLast(value);
						}
						else
						{
							if (num2 >= 0.0 && num3 >= 0.0)
							{
								if (num4 >= 0.0)
								{
									goto IL_1C4;
								}
							}
							try
							{
								Triangle3D[] array = value.Slice(splitPlane, num);
								
								if (array.Length == 1)
								{
									throw new PrecisionException();
								}
								
								for (int i = 0; i < array.Length; i++)
								{
									if (Math.Round((double)array[i].Area, percisionDigits - 1) > 0.0)
									{
										triList.AddLast(array[i]);
									}
								}
							}
							catch (PrecisionException)
							{
								num--;
								
								if (num < 0)
								{
									throw new Exception("Slice Error");
								}
								
								continue;
							}
							
							flag2 = true;
						}
					
					IL_1C4:
						LinkedListNode<Triangle3D> next = linkedListNode.Next;
						
						if (flag2)
						{
							triList.Remove(linkedListNode);
						}
						
						linkedListNode = next;
						flag = false;
					}
				}
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			public void Build(LinkedList<Triangle3D> splitList, int percisionDigits)
			{
				LinkedList<Triangle3D> linkedList = new LinkedList<Triangle3D>();
				LinkedList<Triangle3D> linkedList2 = new LinkedList<Triangle3D>();
				
				BSPTree.BSPTreeNode.ChooseDividingPolygon(splitList);
				
				linkedList.AddFirst(splitList.First.Value);
				
				Triangle3D value = splitList.First.Value;
				
				splitList.RemoveFirst();
				
				this.SplitPlane = new Plane(value.B, value.A, value.C);
				this.SplitPlane.Normalize();
				
				BSPTree.BSPTreeNode.SortList(this.SplitPlane, splitList, linkedList, linkedList2, percisionDigits);
				
				this.TriList = new Triangle3D[linkedList.Count];
				
				linkedList.CopyTo(this.TriList, 0);
				
				Thread thread = null;
				
				bool flag = false;
				bool flag2 = false;
				
				if (splitList.Count > 0)
				{
					this.Front = new BSPTree.BSPTreeNode();
					
					if (thread == null && splitList.Count > 1000 && splitList.Count >= linkedList2.Count && BSPTree.BSPTreeNode.AvailibleThread > 0)
					{
						flag = false;
						
						lock (BSPTree.BSPTreeNode.threadLock)
						{
							BSPTree.BSPTreeNode.AvailibleThread--;
						}
						
						thread = new Thread(delegate(object state)
						{
							object[] array = (object[])state;
							BSPTree.BSPTreeNode bsptreeNode = (BSPTree.BSPTreeNode)array[0];
							bsptreeNode.Build((LinkedList<Triangle3D>)array[1], (int)array[2]);
						});

						thread.Name = "BSP Front Side Worker Thread";
						thread.Start(new object[] { this.Front, splitList, percisionDigits });
					}
					else
					{
						flag = true;
					}
				}

				if (linkedList2.Count > 0)
				{
					this.Back = new BSPTree.BSPTreeNode();
					
					if (thread == null && linkedList2.Count > 1000 && linkedList2.Count >= splitList.Count && BSPTree.BSPTreeNode.AvailibleThread > 0)
					{
						flag2 = false;
						
						lock (BSPTree.BSPTreeNode.threadLock)
						{
							BSPTree.BSPTreeNode.AvailibleThread--;
						}
						
						thread = new Thread(delegate(object state)
						{
							object[] array = (object[])state;
							BSPTree.BSPTreeNode bsptreeNode = (BSPTree.BSPTreeNode)array[0];
							bsptreeNode.Build((LinkedList<Triangle3D>)array[1], (int)array[2]);
						});
						
						thread.Name = "BSP Front Side Worker Thread";
						thread.Start(new object[] { this.Back, linkedList2, percisionDigits });
					}
					else
					{
						flag2 = true;
					}
				}

				if (flag)
				{
					this.Front.Build(splitList, percisionDigits);
				}
				
				if (flag2)
				{
					this.Back.Build(linkedList2, percisionDigits);
				}
				
				if (thread != null)
				{
					thread.Join();
					
					lock (BSPTree.BSPTreeNode.threadLock)
					{
						BSPTree.BSPTreeNode.AvailibleThread++;
					}
				}
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			public static BSPTree.BSPTreeNode Load(BinaryReader reader)
			{
				int num = reader.ReadInt32();
				
				if (num == 0)
				{
					return null;
				}
				
				BSPTree.BSPTreeNode bsptreeNode = new BSPTree.BSPTreeNode();
				bsptreeNode.TriList = new Triangle3D[num];
				
				for (int i = 0; i < num; i++)
				{
					bsptreeNode.TriList[i] = Triangle3D.Read(reader);
				}
				
				Triangle3D triangle3D = bsptreeNode.TriList[bsptreeNode.TriList.Length - 1];
				bsptreeNode.SplitPlane = new Plane(triangle3D.B, triangle3D.A, triangle3D.C);
				bsptreeNode.SplitPlane.Normalize();
				bsptreeNode.Front = BSPTree.BSPTreeNode.Load(reader);
				bsptreeNode.Back = BSPTree.BSPTreeNode.Load(reader);
				
				return bsptreeNode;
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			public void Save(BinaryWriter writer)
			{
				if (this.TriList == null)
				{
					writer.Write(0);
					return;
				}
				
				writer.Write(this.TriList.Length);
				
				foreach (Triangle3D triangle3D in this.TriList)
				{
					triangle3D.Write(writer);
				}
				
				if (this.Front == null)
				{
					writer.Write(0);
				}
				else
				{
					this.Front.Save(writer);
				}
				
				if (this.Back == null)
				{
					writer.Write(0);
					return;
				}
				
				this.Back.Save(writer);
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			private Triangle3D? CollideWithTriangles(Ray ray)
			{
				for (int i = 0; i < this.TriList.Length; i++)
				{
					if (this.TriList[i].Intersects(ray) != null)
					{
						return new Triangle3D?(this.TriList[i]);
					}
				}

				return null;
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			public CollisionMap.RayQueryResult? CollidesWith(Ray ray, float minT, float maxT, int percisionDigits)
			{
				Vector3 value = ray.Position + ray.Direction * minT;
				
				float num = this.SplitPlane.DotCoordinate(value);
				float? num2 = ray.Intersects(this.SplitPlane);
				
				if (num2 != null)
				{
					num2 = new float?((float)Math.Round((double)num2.Value, percisionDigits));
				}
				
				if (num2 == null || num2.Value < minT || num2.Value > maxT)
				{
					if (num >= 0f)
					{
						if (this.Front == null)
						{
							return null;
						}
						
						return this.Front.CollidesWith(ray, minT, maxT, percisionDigits);
					}
					else
					{
						if (this.Back == null)
						{
							return null;
						}
						
						return this.Back.CollidesWith(ray, minT, maxT, percisionDigits);
					}
				}
				else if (num >= 0f)
				{
					CollisionMap.RayQueryResult? result = null;
					
					if (this.Front != null)
					{
						result = this.Front.CollidesWith(ray, minT, Math.Min(maxT, num2.Value), percisionDigits);
					}
					
					if (result != null)
					{
						return result;
					}
					
					Triangle3D? triangle3D = this.CollideWithTriangles(ray);
					
					if (triangle3D != null)
					{
						return new CollisionMap.RayQueryResult?(new CollisionMap.RayQueryResult(num2.Value, triangle3D.Value));
					}
					
					if (this.Back == null)
					{
						return null;
					}
					
					return this.Back.CollidesWith(ray, Math.Max(minT, num2.Value), maxT, percisionDigits);
				}
				else
				{
					CollisionMap.RayQueryResult? result2 = null;
					
					if (this.Back != null)
					{
						result2 = this.Back.CollidesWith(ray, minT, Math.Min(maxT, num2.Value), percisionDigits);
					}
					
					if (result2 != null)
					{
						return result2;
					}
					
					if (this.Front == null)
					{
						return null;
					}
					
					return this.Front.CollidesWith(ray, Math.Max(minT, num2.Value), maxT, percisionDigits);
				}
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			private void CollideWithTriangles(Ellipsoid ellipsoid, float distanceToPlane, List<CollisionMap.ContactPoint> contacts)
			{
				Vector3 normal = this.SplitPlane.Normal;
				float num = ellipsoid.Radius.X * normal.X * ellipsoid.Radius.X * normal.X + ellipsoid.Radius.Y * normal.Y * ellipsoid.Radius.Y * normal.Y + ellipsoid.Radius.Z * normal.Z * ellipsoid.Radius.Z * normal.Z;
				float num2 = distanceToPlane * distanceToPlane;
				
				for (int i = 0; i < this.TriList.Length; i++)
				{
					Triangle3D tri = this.TriList[i];
					Vector3 vector = ellipsoid.Center - tri.A;
					Vector3.Dot(normal, vector);
					Vector3 value = distanceToPlane * normal;
					Vector3 value2 = ellipsoid.Center + value;
					Vector3 vector2 = tri.B - tri.A;
					Vector3 vector3 = tri.C - tri.A;
					float num3 = Vector3.Dot(vector2, vector2);
					float num4 = Vector3.Dot(vector2, vector3);
					float num5 = Vector3.Dot(vector3, vector3);
					Vector3 vector4 = value2 - tri.A;
					float num6 = Vector3.Dot(vector4, vector2);
					float num7 = Vector3.Dot(vector4, vector3);
					float num8 = num4 * num4 - num3 * num5;
					float num9 = (num4 * num7 - num5 * num6) / num8;
					
					if ((double)num9 >= 0.0 && (double)num9 <= 1.0)
					{
						float num10 = (num4 * num6 - num3 * num7) / num8;
						
						if ((double)num10 >= 0.0 && (double)(num9 + num10) <= 1.0 && num2 <= num)
						{
							float num11 = (float)Math.Sqrt((double)num);
							Vector3 dir = (num11 - distanceToPlane) * normal;
							contacts.Add(new CollisionMap.ContactPoint(dir, tri));
						}
					}
				}
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			private void CollideWithTriangles(Ellipsoid ellipsoid, Plane localPlane, float distanceToLocalPlane, List<CollisionMap.ContactPoint> contacts)
			{
				Triangle3D triangle3D = default(Triangle3D);
				
				for (int i = 0; i < this.TriList.Length; i++)
				{
					ellipsoid.TransformWorldSpaceTriToUnitSphereSpace(this.TriList[i], ref triangle3D);
					Vector3.Dot(localPlane.Normal, triangle3D.A);
					Vector3 vector = -distanceToLocalPlane * localPlane.Normal;
					Vector3 vector2 = triangle3D.B - triangle3D.A;
					Vector3 vector3 = triangle3D.C - triangle3D.A;
					float num = Vector3.Dot(vector2, vector2);
					float num2 = Vector3.Dot(vector2, vector3);
					float num3 = Vector3.Dot(vector3, vector3);
					Vector3 vector4 = vector - triangle3D.A;
					float num4 = Vector3.Dot(vector4, vector2);
					float num5 = Vector3.Dot(vector4, vector3);
					float num6 = num2 * num2 - num * num3;
					float num7 = (num2 * num5 - num3 * num4) / num6;
					
					if ((double)num7 >= 0.0 && (double)num7 <= 1.0)
					{
						float num8 = (num2 * num4 - num * num5) / num6;
					
						if ((double)num8 >= 0.0 && (double)(num7 + num8) <= 1.0)
						{
							float p = ellipsoid.CalculateWorldSpacePenetration(vector);
							contacts.Add(new CollisionMap.ContactPoint(this.SplitPlane.Normal, this.TriList[i], p));
							return;
						}
					}
				}

				Vector3 vector5 = Vector3.Zero;
				
				for (int j = 0; j < this.TriList.Length; j++)
				{
					ellipsoid.TransformWorldSpaceTriToUnitSphereSpace(this.TriList[j], ref triangle3D);
					LineF3D lineF3D = new LineF3D(triangle3D.A, triangle3D.B);
					LineF3D lineF3D2 = new LineF3D(triangle3D.A, triangle3D.C);
					LineF3D lineF3D3 = new LineF3D(triangle3D.B, triangle3D.C);
					Vector3 vector6 = lineF3D.ClosetPointTo(Vector3.Zero);
					Vector3 vector7 = lineF3D2.ClosetPointTo(Vector3.Zero);
					Vector3 vector8 = lineF3D3.ClosetPointTo(Vector3.Zero);
					float num9 = vector6.LengthSquared();
					float num10 = vector7.LengthSquared();
					float num11 = vector8.LengthSquared();
					bool flag = false;
					float num12 = 1f;
					
					if (num9 <= num12)
					{
						num12 = num9;
						vector5 = vector6;
						flag = true;
					}
					
					if (num10 <= num12)
					{
						num12 = num10;
						vector5 = vector7;
						flag = true;
					}
					
					if (num11 <= num12)
					{
						vector5 = vector8;
						flag = true;
					}
					
					if (flag)
					{
						float p2 = ellipsoid.CalculateWorldSpacePenetration(vector5);
						vector5 = ellipsoid.TransformUnitSphereSpaceVectorToWorldSpace(Vector3.Negate(vector5));
						contacts.Add(new CollisionMap.ContactPoint(vector5, this.TriList[j], p2));
					}
				}
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			public void FindCollisions(Ellipsoid ellipsoid, List<CollisionMap.ContactPoint> contacts)
			{
				Plane localPlane = ellipsoid.TransformWorldSpacePlaneToUnitSphereSpace(this.SplitPlane);
				float d = localPlane.D;
				
				if (d >= 0f)
				{
					if (this.Front != null)
					{
						this.Front.FindCollisions(ellipsoid, contacts);
					}
					
					if (d <= 1f)
					{
						this.CollideWithTriangles(ellipsoid, localPlane, d, contacts);
						
						if (this.Back != null)
						{
							this.Back.FindCollisions(ellipsoid, contacts);
							return;
						}
					}
				}
				else
				{
					if (this.Back != null)
					{
						this.Back.FindCollisions(ellipsoid, contacts);
					}
					
					if (d >= -1f && this.Front != null)
					{
						this.Front.FindCollisions(ellipsoid, contacts);
					}
				}
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			public void FindCollisions(BoundingSphere sphere, List<CollisionMap.ContactPoint> contacts)
			{
				float num = this.SplitPlane.DotCoordinate(sphere.Center);
				
				if (num >= 0f)
				{
					if (this.Front != null)
					{
						this.Front.FindCollisions(sphere, contacts);
					}
					
					if (num <= sphere.Radius)
					{
						this.CollideWithTriangles(sphere, num, contacts);
						
						if (this.Back != null)
						{
							this.Back.FindCollisions(sphere, contacts);
							return;
						}
					}
				}
				else
				{
					if (this.Back != null)
					{
						this.Back.FindCollisions(sphere, contacts);
					}
					
					if (num >= -sphere.Radius && this.Front != null)
					{
						this.Front.FindCollisions(sphere, contacts);
					}
				}
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			private void CollideWithTriangles(BoundingSphere sphere, float distanceToPlane, List<CollisionMap.ContactPoint> contacts)
			{
				Vector3 normal = this.SplitPlane.Normal;
				float num = distanceToPlane * distanceToPlane;
				float num2 = sphere.Radius * sphere.Radius;
				
				for (int i = 0; i < this.TriList.Length; i++)
				{
					Triangle3D tri = this.TriList[i];
					Vector3 vector = sphere.Center - tri.A;
					Vector3.Dot(normal, vector);
					Vector3 value = distanceToPlane * normal;
					Vector3 value2 = sphere.Center + value;
					Vector3 vector2 = tri.B - tri.A;
					Vector3 vector3 = tri.C - tri.A;
					float num3 = Vector3.Dot(vector2, vector2);
					float num4 = Vector3.Dot(vector2, vector3);
					float num5 = Vector3.Dot(vector3, vector3);
					Vector3 vector4 = value2 - tri.A;
					float num6 = Vector3.Dot(vector4, vector2);
					float num7 = Vector3.Dot(vector4, vector3);
					float num8 = num4 * num4 - num3 * num5;
					float num9 = (num4 * num7 - num5 * num6) / num8;
					
					if ((double)num9 >= 0.0 && (double)num9 <= 1.0)
					{
						float num10 = (num4 * num6 - num3 * num7) / num8;
						
						if ((double)num10 >= 0.0 && (double)(num9 + num10) <= 1.0 && num <= num2)
						{
							Vector3 dir = (sphere.Radius - distanceToPlane) * normal;
							contacts.Add(new CollisionMap.ContactPoint(dir, tri));
						}
					}
				}
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			public void GetData(List<Plane> planes, List<Triangle3D> triangles)
			{
				planes.Add(this.SplitPlane);
				
				if (this.TriList != null)
				{
					triangles.AddRange(this.TriList);
				}
				
				if (this.Front != null)
				{
					this.Front.GetData(planes, triangles);
				}
				
				if (this.Back != null)
				{
					this.Back.GetData(planes, triangles);
				}
			}
		}

		private const int Version = 1;
		private BSPTree.BSPTreeNode _root = new BSPTree.BSPTreeNode();
		private int _percisionDigits;
		private readonly string FileID = "TASBSP";

		/// <summary>
		/// 
		/// </summary>
		public BSPTree()
		{
			this._root = new BSPTree.BSPTreeNode();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public BSPTree(Model model, int percisionDigits)
		{
			this.Build(model, percisionDigits);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void Build(IList<Triangle3D> polys, int percisionDigits)
		{
			LinkedList<Triangle3D> linkedList = new LinkedList<Triangle3D>();
			int num = 0;
			
			foreach (Triangle3D value in polys)
			{
				double num2 = Math.Round((double)value.Area, percisionDigits);
				
				if (num2 != 0.0)
				{
					linkedList.AddLast(value);
				}
				else
				{
					num++;
				}
			}

			this._percisionDigits = percisionDigits;
			this._root = new BSPTree.BSPTreeNode();
			this._root.Build(linkedList, percisionDigits);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void Build(Model model, int percisionDigits)
		{
			List<Triangle3D> polys = model.ExtractModelTris(true);
			this.Build(polys, percisionDigits);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		protected override CollisionMap.RayQueryResult? CollidesWith(Ray ray, float min, float max)
		{
			return this._root.CollidesWith(ray, min, max, this._percisionDigits);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public override void FindCollisions(Ellipsoid ellipsoid, List<CollisionMap.ContactPoint> contacts)
		{
			contacts.Clear();
			this._root.FindCollisions(ellipsoid, contacts);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void AppendCollisions(Ellipsoid ellipsoid, List<CollisionMap.ContactPoint> contacts)
		{
			this._root.FindCollisions(ellipsoid, contacts);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public override void FindCollisions(BoundingSphere sphere, List<CollisionMap.ContactPoint> contacts)
		{
			contacts.Clear();
			this._root.FindCollisions(sphere, contacts);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public override void Save(BinaryWriter writer)
		{
			writer.Write(this.FileID);
			writer.Write(1);
			writer.Write(this._percisionDigits);
			this._root.Save(writer);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public override void Load(BinaryReader reader)
		{
			string a = reader.ReadString();
			
			if (a != this.FileID)
			{
				throw new Exception("Bad BSP file Format");
			}
			
			int num = reader.ReadInt32();
			
			if (num != 1)
			{
				throw new Exception("Bad BSP version");
			}
			
			this._percisionDigits = reader.ReadInt32();
			this._root = BSPTree.BSPTreeNode.Load(reader);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void GetData(List<Plane> planes, List<Triangle3D> triangles)
		{
			this._root.GetData(planes, triangles);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public override void GetTriangles(List<Triangle3D> tris)
		{
			this.GetData(new List<Plane>(), tris);
		}
	}
}
