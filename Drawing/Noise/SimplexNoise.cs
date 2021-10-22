using System;
using Microsoft.Xna.Framework;

namespace DNA.Drawing.Noise
{
	public class SimplexNoise
	{
		private const float Sqrt3 = 1.73205078f;

		private const float Sqrt5 = 2.236068f;

		private static readonly int[][] s_gradientVectors3;

		private static readonly int[][] s_gradientVectors4;

		private int[] _permute = new int[512];

		private static readonly int[][] s_simplex;

		public float ComputeNoise(float x, float y)
		{
			float num = 0.3660254f;
			float num2 = (x + y) * num;
			int num3 = SimplexNoise.FastFloor(x + num2);
			int num4 = SimplexNoise.FastFloor(y + num2);
			float num5 = 0.211324871f;
			float num6 = (float)(num3 + num4) * num5;
			float num7 = (float)num3 - num6;
			float num8 = (float)num4 - num6;
			float num9 = x - num7;
			float num10 = y - num8;
			int num11;
			int num12;
			if (num9 > num10)
			{
				num11 = 1;
				num12 = 0;
			}
			else
			{
				num11 = 0;
				num12 = 1;
			}
			float num13 = num9 - (float)num11 + num5;
			float num14 = num10 - (float)num12 + num5;
			float num15 = num9 - 1f + 2f * num5;
			float num16 = num10 - 1f + 2f * num5;
			int num17 = num3 & 255;
			int num18 = num4 & 255;
			int num19 = this._permute[num17 + this._permute[num18]] % 12;
			int num20 = this._permute[num17 + num11 + this._permute[num18 + num12]] % 12;
			int num21 = this._permute[num17 + 1 + this._permute[num18 + 1]] % 12;
			float num22 = 0.5f - num9 * num9 - num10 * num10;
			float num23;
			if (num22 < 0f)
			{
				num23 = 0f;
			}
			else
			{
				num22 *= num22;
				num23 = num22 * num22 * SimplexNoise.Dot(SimplexNoise.s_gradientVectors3[num19], num9, num10);
			}
			float num24 = 0.5f - num13 * num13 - num14 * num14;
			float num25;
			if (num24 < 0f)
			{
				num25 = 0f;
			}
			else
			{
				num24 *= num24;
				num25 = num24 * num24 * SimplexNoise.Dot(SimplexNoise.s_gradientVectors3[num20], num13, num14);
			}
			float num26 = 0.5f - num15 * num15 - num16 * num16;
			float num27;
			if (num26 < 0f)
			{
				num27 = 0f;
			}
			else
			{
				num26 *= num26;
				num27 = num26 * num26 * SimplexNoise.Dot(SimplexNoise.s_gradientVectors3[num21], num15, num16);
			}
			return 70f * (num23 + num25 + num27);
		}

		public float ComputeNoise(float x, float y, float z)
		{
			float num = 0.333333343f;
			float num2 = (x + y + z) * num;
			int num3 = SimplexNoise.FastFloor(x + num2);
			int num4 = SimplexNoise.FastFloor(y + num2);
			int num5 = SimplexNoise.FastFloor(z + num2);
			float num6 = 0.166666672f;
			float num7 = (float)(num3 + num4 + num5) * num6;
			float num8 = (float)num3 - num7;
			float num9 = (float)num4 - num7;
			float num10 = (float)num5 - num7;
			float num11 = x - num8;
			float num12 = y - num9;
			float num13 = z - num10;
			int num14;
			int num15;
			int num16;
			int num17;
			int num18;
			int num19;
			if (num11 >= num12)
			{
				if (num12 >= num13)
				{
					num14 = 1;
					num15 = 0;
					num16 = 0;
					num17 = 1;
					num18 = 1;
					num19 = 0;
				}
				else if (num11 >= num13)
				{
					num14 = 1;
					num15 = 0;
					num16 = 0;
					num17 = 1;
					num18 = 0;
					num19 = 1;
				}
				else
				{
					num14 = 0;
					num15 = 0;
					num16 = 1;
					num17 = 1;
					num18 = 0;
					num19 = 1;
				}
			}
			else if (num12 < num13)
			{
				num14 = 0;
				num15 = 0;
				num16 = 1;
				num17 = 0;
				num18 = 1;
				num19 = 1;
			}
			else if (num11 < num13)
			{
				num14 = 0;
				num15 = 1;
				num16 = 0;
				num17 = 0;
				num18 = 1;
				num19 = 1;
			}
			else
			{
				num14 = 0;
				num15 = 1;
				num16 = 0;
				num17 = 1;
				num18 = 1;
				num19 = 0;
			}
			float num20 = num11 - (float)num14 + num6;
			float num21 = num12 - (float)num15 + num6;
			float num22 = num13 - (float)num16 + num6;
			float num23 = num11 - (float)num17 + 2f * num6;
			float num24 = num12 - (float)num18 + 2f * num6;
			float num25 = num13 - (float)num19 + 2f * num6;
			float num26 = num11 - 1f + 3f * num6;
			float num27 = num12 - 1f + 3f * num6;
			float num28 = num13 - 1f + 3f * num6;
			int num29 = num3 & 255;
			int num30 = num4 & 255;
			int num31 = num5 & 255;
			int num32 = this._permute[num29 + this._permute[num30 + this._permute[num31]]] % 12;
			int num33 = this._permute[num29 + num14 + this._permute[num30 + num15 + this._permute[num31 + num16]]] % 12;
			int num34 = this._permute[num29 + num17 + this._permute[num30 + num18 + this._permute[num31 + num19]]] % 12;
			int num35 = this._permute[num29 + 1 + this._permute[num30 + 1 + this._permute[num31 + 1]]] % 12;
			float num36 = 0.6f - num11 * num11 - num12 * num12 - num13 * num13;
			float num37;
			if (num36 < 0f)
			{
				num37 = 0f;
			}
			else
			{
				num36 *= num36;
				num37 = num36 * num36 * SimplexNoise.Dot(SimplexNoise.s_gradientVectors3[num32], num11, num12, num13);
			}
			float num38 = 0.6f - num20 * num20 - num21 * num21 - num22 * num22;
			float num39;
			if (num38 < 0f)
			{
				num39 = 0f;
			}
			else
			{
				num38 *= num38;
				num39 = num38 * num38 * SimplexNoise.Dot(SimplexNoise.s_gradientVectors3[num33], num20, num21, num22);
			}
			float num40 = 0.6f - num23 * num23 - num24 * num24 - num25 * num25;
			float num41;
			if (num40 < 0f)
			{
				num41 = 0f;
			}
			else
			{
				num40 *= num40;
				num41 = num40 * num40 * SimplexNoise.Dot(SimplexNoise.s_gradientVectors3[num34], num23, num24, num25);
			}
			float num42 = 0.6f - num26 * num26 - num27 * num27 - num28 * num28;
			float num43;
			if (num42 < 0f)
			{
				num43 = 0f;
			}
			else
			{
				num42 *= num42;
				num43 = num42 * num42 * SimplexNoise.Dot(SimplexNoise.s_gradientVectors3[num35], num26, num27, num28);
			}
			return 32f * (num37 + num39 + num41 + num43);
		}

		public float ComputeNoise(float x, float y, float z, float w)
		{
			float num = 0.309017f;
			float num2 = 0.1381966f;
			float num3 = (x + y + z + w) * num;
			int num4 = SimplexNoise.FastFloor(x + num3);
			int num5 = SimplexNoise.FastFloor(y + num3);
			int num6 = SimplexNoise.FastFloor(z + num3);
			int num7 = SimplexNoise.FastFloor(w + num3);
			float num8 = (float)(num4 + num5 + num6 + num7) * num2;
			float num9 = (float)num4 - num8;
			float num10 = (float)num5 - num8;
			float num11 = (float)num6 - num8;
			float num12 = (float)num7 - num8;
			float num13 = x - num9;
			float num14 = y - num10;
			float num15 = z - num11;
			float num16 = w - num12;
			int num17 = (num13 > num14) ? 32 : 0;
			int num18 = (num13 > num15) ? 16 : 0;
			int num19 = (num14 > num15) ? 8 : 0;
			int num20 = (num13 > num16) ? 4 : 0;
			int num21 = (num14 > num16) ? 2 : 0;
			int num22 = (num15 > num16) ? 1 : 0;
			int num23 = num17 + num18 + num19 + num20 + num21 + num22;
			int num24 = (SimplexNoise.s_simplex[num23][0] >= 3) ? 1 : 0;
			int num25 = (SimplexNoise.s_simplex[num23][1] >= 3) ? 1 : 0;
			int num26 = (SimplexNoise.s_simplex[num23][2] >= 3) ? 1 : 0;
			int num27 = (SimplexNoise.s_simplex[num23][3] >= 3) ? 1 : 0;
			int num28 = (SimplexNoise.s_simplex[num23][0] >= 2) ? 1 : 0;
			int num29 = (SimplexNoise.s_simplex[num23][1] >= 2) ? 1 : 0;
			int num30 = (SimplexNoise.s_simplex[num23][2] >= 2) ? 1 : 0;
			int num31 = (SimplexNoise.s_simplex[num23][3] >= 2) ? 1 : 0;
			int num32 = (SimplexNoise.s_simplex[num23][0] >= 1) ? 1 : 0;
			int num33 = (SimplexNoise.s_simplex[num23][1] >= 1) ? 1 : 0;
			int num34 = (SimplexNoise.s_simplex[num23][2] >= 1) ? 1 : 0;
			int num35 = (SimplexNoise.s_simplex[num23][3] >= 1) ? 1 : 0;
			float num36 = num13 - (float)num24 + num2;
			float num37 = num14 - (float)num25 + num2;
			float num38 = num15 - (float)num26 + num2;
			float num39 = num16 - (float)num27 + num2;
			float num40 = num13 - (float)num28 + 2f * num2;
			float num41 = num14 - (float)num29 + 2f * num2;
			float num42 = num15 - (float)num30 + 2f * num2;
			float num43 = num16 - (float)num31 + 2f * num2;
			float num44 = num13 - (float)num32 + 3f * num2;
			float num45 = num14 - (float)num33 + 3f * num2;
			float num46 = num15 - (float)num34 + 3f * num2;
			float num47 = num16 - (float)num35 + 3f * num2;
			float num48 = num13 - 1f + 4f * num2;
			float num49 = num14 - 1f + 4f * num2;
			float num50 = num15 - 1f + 4f * num2;
			float num51 = num16 - 1f + 4f * num2;
			int num52 = num4 & 255;
			int num53 = num5 & 255;
			int num54 = num6 & 255;
			int num55 = num7 & 255;
			int num56 = this._permute[num52 + this._permute[num53 + this._permute[num54 + this._permute[num55]]]] % 32;
			int num57 = this._permute[num52 + num24 + this._permute[num53 + num25 + this._permute[num54 + num26 + this._permute[num55 + num27]]]] % 32;
			int num58 = this._permute[num52 + num28 + this._permute[num53 + num29 + this._permute[num54 + num30 + this._permute[num55 + num31]]]] % 32;
			int num59 = this._permute[num52 + num32 + this._permute[num53 + num33 + this._permute[num54 + num34 + this._permute[num55 + num35]]]] % 32;
			int num60 = this._permute[num52 + 1 + this._permute[num53 + 1 + this._permute[num54 + 1 + this._permute[num55 + 1]]]] % 32;
			float num61 = 0.6f - num13 * num13 - num14 * num14 - num15 * num15 - num16 * num16;
			float num62;
			if (num61 < 0f)
			{
				num62 = 0f;
			}
			else
			{
				num61 *= num61;
				num62 = num61 * num61 * SimplexNoise.Dot(SimplexNoise.s_gradientVectors4[num56], num13, num14, num15, num16);
			}
			float num63 = 0.6f - num36 * num36 - num37 * num37 - num38 * num38 - num39 * num39;
			float num64;
			if (num63 < 0f)
			{
				num64 = 0f;
			}
			else
			{
				num63 *= num63;
				num64 = num63 * num63 * SimplexNoise.Dot(SimplexNoise.s_gradientVectors4[num57], num36, num37, num38, num39);
			}
			float num65 = 0.6f - num40 * num40 - num41 * num41 - num42 * num42 - num43 * num43;
			float num66;
			if (num65 < 0f)
			{
				num66 = 0f;
			}
			else
			{
				num65 *= num65;
				num66 = num65 * num65 * SimplexNoise.Dot(SimplexNoise.s_gradientVectors4[num58], num40, num41, num42, num43);
			}
			float num67 = 0.6f - num44 * num44 - num45 * num45 - num46 * num46 - num47 * num47;
			float num68;
			if (num67 < 0f)
			{
				num68 = 0f;
			}
			else
			{
				num67 *= num67;
				num68 = num67 * num67 * SimplexNoise.Dot(SimplexNoise.s_gradientVectors4[num59], num44, num45, num46, num47);
			}
			float num69 = 0.6f - num48 * num48 - num49 * num49 - num50 * num50 - num51 * num51;
			float num70;
			if (num69 < 0f)
			{
				num70 = 0f;
			}
			else
			{
				num69 *= num69;
				num70 = num69 * num69 * SimplexNoise.Dot(SimplexNoise.s_gradientVectors4[num60], num48, num49, num50, num51);
			}
			return 27f * (num62 + num64 + num66 + num68 + num70);
		}

		public float ComputeNoise(Vector2 v)
		{
			return this.ComputeNoise(v.X, v.Y);
		}

		public float ComputeNoise(Vector3 v)
		{
			return this.ComputeNoise(v.X, v.Y, v.Z);
		}

		public float ComputeNoise(Vector4 v)
		{
			return this.ComputeNoise(v.X, v.Y, v.Z, v.W);
		}

		private static int FastFloor(float x)
		{
			if (x <= 0f)
			{
				return (int)x - 1;
			}
			return (int)x;
		}

		private static float Dot(int[] g, float x, float y)
		{
			return (float)g[0] * x + (float)g[1] * y;
		}

		private static float Dot(int[] g, float x, float y, float z)
		{
			return (float)g[0] * x + (float)g[1] * y + (float)g[2] * z;
		}

		private static float Dot(int[] g, float x, float y, float z, float w)
		{
			return (float)g[0] * x + (float)g[1] * y + (float)g[2] * z + (float)g[3] * w;
		}

		private void Initalize(Random r)
		{
			for (int i = 0; i < 256; i++)
			{
				this._permute[256 + i] = (this._permute[i] = r.Next(256));
			}
		}

		public SimplexNoise()
		{
			this.Initalize(new Random());
		}

		public SimplexNoise(Random r)
		{
			this.Initalize(r);
		}

		// Note: this type is marked as 'beforefieldinit'.
		static SimplexNoise()
		{
			int[][] array = new int[12][];
			int[][] array2 = array;
			int num = 0;
			int[] array3 = new int[3];
			array3[0] = 1;
			array3[1] = 1;
			array2[num] = array3;
			int[][] array4 = array;
			int num2 = 1;
			int[] array5 = new int[3];
			array5[0] = -1;
			array5[1] = 1;
			array4[num2] = array5;
			int[][] array6 = array;
			int num3 = 2;
			int[] array7 = new int[3];
			array7[0] = 1;
			array7[1] = -1;
			array6[num3] = array7;
			int[][] array8 = array;
			int num4 = 3;
			int[] array9 = new int[3];
			array9[0] = -1;
			array9[1] = -1;
			array8[num4] = array9;
			array[4] = new int[]
			{
				1,
				0,
				1
			};
			array[5] = new int[]
			{
				-1,
				0,
				1
			};
			array[6] = new int[]
			{
				1,
				0,
				-1
			};
			array[7] = new int[]
			{
				-1,
				0,
				-1
			};
			array[8] = new int[]
			{
				0,
				1,
				1
			};
			array[9] = new int[]
			{
				0,
				-1,
				1
			};
			array[10] = new int[]
			{
				0,
				1,
				-1
			};
			array[11] = new int[]
			{
				0,
				-1,
				-1
			};
			SimplexNoise.s_gradientVectors3 = array;
			SimplexNoise.s_gradientVectors4 = new int[][]
			{
				new int[]
				{
					0,
					1,
					1,
					1
				},
				new int[]
				{
					0,
					1,
					1,
					-1
				},
				new int[]
				{
					0,
					1,
					-1,
					1
				},
				new int[]
				{
					0,
					1,
					-1,
					-1
				},
				new int[]
				{
					0,
					-1,
					1,
					1
				},
				new int[]
				{
					0,
					-1,
					1,
					-1
				},
				new int[]
				{
					0,
					-1,
					-1,
					1
				},
				new int[]
				{
					0,
					-1,
					-1,
					-1
				},
				new int[]
				{
					1,
					0,
					1,
					1
				},
				new int[]
				{
					1,
					0,
					1,
					-1
				},
				new int[]
				{
					1,
					0,
					-1,
					1
				},
				new int[]
				{
					1,
					0,
					-1,
					-1
				},
				new int[]
				{
					-1,
					0,
					1,
					1
				},
				new int[]
				{
					-1,
					0,
					1,
					-1
				},
				new int[]
				{
					-1,
					0,
					-1,
					1
				},
				new int[]
				{
					-1,
					0,
					-1,
					-1
				},
				new int[]
				{
					1,
					1,
					0,
					1
				},
				new int[]
				{
					1,
					1,
					0,
					-1
				},
				new int[]
				{
					1,
					-1,
					0,
					1
				},
				new int[]
				{
					1,
					-1,
					0,
					-1
				},
				new int[]
				{
					-1,
					1,
					0,
					1
				},
				new int[]
				{
					-1,
					1,
					0,
					-1
				},
				new int[]
				{
					-1,
					-1,
					0,
					1
				},
				new int[]
				{
					-1,
					-1,
					0,
					-1
				},
				new int[]
				{
					1,
					1,
					1,
					0
				},
				new int[]
				{
					1,
					1,
					-1,
					0
				},
				new int[]
				{
					1,
					-1,
					1,
					0
				},
				new int[]
				{
					1,
					-1,
					-1,
					0
				},
				new int[]
				{
					-1,
					1,
					1,
					0
				},
				new int[]
				{
					-1,
					1,
					-1,
					0
				},
				new int[]
				{
					-1,
					-1,
					1,
					0
				},
				new int[]
				{
					-1,
					-1,
					-1,
					0
				}
			};
			int[][] array10 = new int[64][];
			array10[0] = new int[]
			{
				0,
				1,
				2,
				3
			};
			array10[1] = new int[]
			{
				0,
				1,
				3,
				2
			};
			int[][] array11 = array10;
			int num5 = 2;
			int[] array12 = new int[4];
			array11[num5] = array12;
			array10[3] = new int[]
			{
				0,
				2,
				3,
				1
			};
			int[][] array13 = array10;
			int num6 = 4;
			int[] array14 = new int[4];
			array13[num6] = array14;
			int[][] array15 = array10;
			int num7 = 5;
			int[] array16 = new int[4];
			array15[num7] = array16;
			int[][] array17 = array10;
			int num8 = 6;
			int[] array18 = new int[4];
			array17[num8] = array18;
			array10[7] = new int[]
			{
				1,
				2,
				3,
				0
			};
			array10[8] = new int[]
			{
				0,
				2,
				1,
				3
			};
			int[][] array19 = array10;
			int num9 = 9;
			int[] array20 = new int[4];
			array19[num9] = array20;
			array10[10] = new int[]
			{
				0,
				3,
				1,
				2
			};
			array10[11] = new int[]
			{
				0,
				3,
				2,
				1
			};
			int[][] array21 = array10;
			int num10 = 12;
			int[] array22 = new int[4];
			array21[num10] = array22;
			int[][] array23 = array10;
			int num11 = 13;
			int[] array24 = new int[4];
			array23[num11] = array24;
			int[][] array25 = array10;
			int num12 = 14;
			int[] array26 = new int[4];
			array25[num12] = array26;
			array10[15] = new int[]
			{
				1,
				3,
				2,
				0
			};
			int[][] array27 = array10;
			int num13 = 16;
			int[] array28 = new int[4];
			array27[num13] = array28;
			int[][] array29 = array10;
			int num14 = 17;
			int[] array30 = new int[4];
			array29[num14] = array30;
			int[][] array31 = array10;
			int num15 = 18;
			int[] array32 = new int[4];
			array31[num15] = array32;
			int[][] array33 = array10;
			int num16 = 19;
			int[] array34 = new int[4];
			array33[num16] = array34;
			int[][] array35 = array10;
			int num17 = 20;
			int[] array36 = new int[4];
			array35[num17] = array36;
			int[][] array37 = array10;
			int num18 = 21;
			int[] array38 = new int[4];
			array37[num18] = array38;
			int[][] array39 = array10;
			int num19 = 22;
			int[] array40 = new int[4];
			array39[num19] = array40;
			int[][] array41 = array10;
			int num20 = 23;
			int[] array42 = new int[4];
			array41[num20] = array42;
			array10[24] = new int[]
			{
				1,
				2,
				0,
				3
			};
			int[][] array43 = array10;
			int num21 = 25;
			int[] array44 = new int[4];
			array43[num21] = array44;
			array10[26] = new int[]
			{
				1,
				3,
				0,
				2
			};
			int[][] array45 = array10;
			int num22 = 27;
			int[] array46 = new int[4];
			array45[num22] = array46;
			int[][] array47 = array10;
			int num23 = 28;
			int[] array48 = new int[4];
			array47[num23] = array48;
			int[][] array49 = array10;
			int num24 = 29;
			int[] array50 = new int[4];
			array49[num24] = array50;
			array10[30] = new int[]
			{
				2,
				3,
				0,
				1
			};
			array10[31] = new int[]
			{
				2,
				3,
				1,
				0
			};
			array10[32] = new int[]
			{
				1,
				0,
				2,
				3
			};
			array10[33] = new int[]
			{
				1,
				0,
				3,
				2
			};
			int[][] array51 = array10;
			int num25 = 34;
			int[] array52 = new int[4];
			array51[num25] = array52;
			int[][] array53 = array10;
			int num26 = 35;
			int[] array54 = new int[4];
			array53[num26] = array54;
			int[][] array55 = array10;
			int num27 = 36;
			int[] array56 = new int[4];
			array55[num27] = array56;
			array10[37] = new int[]
			{
				2,
				0,
				3,
				1
			};
			int[][] array57 = array10;
			int num28 = 38;
			int[] array58 = new int[4];
			array57[num28] = array58;
			array10[39] = new int[]
			{
				2,
				1,
				3,
				0
			};
			int[][] array59 = array10;
			int num29 = 40;
			int[] array60 = new int[4];
			array59[num29] = array60;
			int[][] array61 = array10;
			int num30 = 41;
			int[] array62 = new int[4];
			array61[num30] = array62;
			int[][] array63 = array10;
			int num31 = 42;
			int[] array64 = new int[4];
			array63[num31] = array64;
			int[][] array65 = array10;
			int num32 = 43;
			int[] array66 = new int[4];
			array65[num32] = array66;
			int[][] array67 = array10;
			int num33 = 44;
			int[] array68 = new int[4];
			array67[num33] = array68;
			int[][] array69 = array10;
			int num34 = 45;
			int[] array70 = new int[4];
			array69[num34] = array70;
			int[][] array71 = array10;
			int num35 = 46;
			int[] array72 = new int[4];
			array71[num35] = array72;
			int[][] array73 = array10;
			int num36 = 47;
			int[] array74 = new int[4];
			array73[num36] = array74;
			array10[48] = new int[]
			{
				2,
				0,
				1,
				3
			};
			int[][] array75 = array10;
			int num37 = 49;
			int[] array76 = new int[4];
			array75[num37] = array76;
			int[][] array77 = array10;
			int num38 = 50;
			int[] array78 = new int[4];
			array77[num38] = array78;
			int[][] array79 = array10;
			int num39 = 51;
			int[] array80 = new int[4];
			array79[num39] = array80;
			array10[52] = new int[]
			{
				3,
				0,
				1,
				2
			};
			array10[53] = new int[]
			{
				3,
				0,
				2,
				1
			};
			int[][] array81 = array10;
			int num40 = 54;
			int[] array82 = new int[4];
			array81[num40] = array82;
			array10[55] = new int[]
			{
				3,
				1,
				2,
				0
			};
			array10[56] = new int[]
			{
				2,
				1,
				0,
				3
			};
			int[][] array83 = array10;
			int num41 = 57;
			int[] array84 = new int[4];
			array83[num41] = array84;
			int[][] array85 = array10;
			int num42 = 58;
			int[] array86 = new int[4];
			array85[num42] = array86;
			int[][] array87 = array10;
			int num43 = 59;
			int[] array88 = new int[4];
			array87[num43] = array88;
			array10[60] = new int[]
			{
				3,
				1,
				0,
				2
			};
			int[][] array89 = array10;
			int num44 = 61;
			int[] array90 = new int[4];
			array89[num44] = array90;
			array10[62] = new int[]
			{
				3,
				2,
				0,
				1
			};
			array10[63] = new int[]
			{
				3,
				2,
				1,
				0
			};
			SimplexNoise.s_simplex = array10;
		}
	}
}
