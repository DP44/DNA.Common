using System;

namespace DNA 
{
	public class FastRand 
	{
		private const int IA = 16807;
		private const int IM = 2147483647;
		private const float AM = 4.656613E-10f;
		private const int IQ = 127773;
		private const int IR = 2836;
		private const int MASK = 123459876;

		private int _idnum;

		public int Seed 
		{
			set => 
				this._idnum = value;
		}

		public FastRand() => 
			this._idnum = (int)DateTime.Now.Ticks;

		public FastRand(int seed) 
		{
			this._idnum = seed;
			this.GetNextValue();
		}

		public float GetNextValue()
		{
			float next;
			
			do
			{
				this._idnum ^= 123459876;
				int x = this._idnum / 127773;
				this._idnum = 16807 * (this._idnum - x * 127773) - 2836 * x;
				
				if (this._idnum < 0)
				{
					this._idnum += int.MaxValue;
				}
				
				float y = 4.656613E-10f * (float)this._idnum;
				
				this._idnum ^= 123459876;
				next = 1f - y;
			}
			while ((double)next >= 1.0);

			return next;
		}

		public int GetNextValue(int min, int max) => 
			(int)((double)(max - min) * this.GetNextValue()) + min;

		public float GetNextValue(float min, float max) => 
			this.GetNextValue() * (max - min) + min;
	}
}
