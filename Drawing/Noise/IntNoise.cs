using System;

namespace DNA.Drawing.Noise
{
	public class IntNoise
	{
		private static int[] _permute = new int[1024];

		/// <summary>
		/// 
		/// </summary>
		public IntNoise()
		{
			this.Initalize(new Random());
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public IntNoise(Random r)
		{
			this.Initalize(r);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		private void Initalize(Random r)
		{
			for (int i = 0; i < 256; i++)
			{
				IntNoise._permute[256 + i] = (IntNoise._permute[i] = r.Next(256));
			}
			
			for (int j = 0; j < 512; j++)
			{
				IntNoise._permute[512 + j] = IntNoise._permute[j];
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public int ComputeNoise(IntVector3 v)
		{
			return this.ComputeNoise(v.X, v.Y, v.Z);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public int ComputeNoise(int x, int y, int z)
		{
			int xNormalized = x & 255;
			int yNormalized = y & 255;
			int zNormalized = z & 255;
			int xy = IntNoise._permute[xNormalized] + yNormalized;
			int xyz = IntNoise._permute[xy] + zNormalized;
			return IntNoise._permute[xyz];
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public int ComputeNoise(int x, int y)
		{
			int xNormalized = x & 255;
			int yNormalized = y & 255;
			int xy = IntNoise._permute[xNormalized] + yNormalized;
			return IntNoise._permute[xy];
		}
	}
}
