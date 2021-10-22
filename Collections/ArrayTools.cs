using System;
using System.Collections.Generic;

namespace DNA.Collections
{
	public static class ArrayTools
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static T[][] AllocSquareJaggedArray<T>(int x, int y)
		{
			T[][] array = new T[x][];
			
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new T[y];
			}
			
			return array;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static void Randomize<T>(IList<T> array, Random rand)
		{
			if (array is T[])
			{
				T[] randomizedArray = (T[])array;
				
				for (int i = 0; i < randomizedArray.Length; i++)
				{
					int randValue = i + rand.Next(randomizedArray.Length - i);
					T t = randomizedArray[randValue];
					randomizedArray[randValue] = randomizedArray[i];
					randomizedArray[i] = t;
				}
				
				return;
			}

			for (int j = 0; j < array.Count; j++)
			{
				int randValue = j + rand.Next(array.Count - j);
				T arrayValue = array[randValue];
				array[randValue] = array[j];
				array[j] = arrayValue;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static void Randomize<T>(IList<T> array) =>
			ArrayTools.Randomize<T>(array, new Random());

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static void QSort<T>(IList<T> list, Comparison<T> comparison) =>
			ArrayTools.QSort_r<T>(list, comparison, 0, list.Count - 1, Order.Ascending);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		private static void QSort_r<T>(IList<T> list, Comparison<T> comparison, 
									   int d, int h, Order direction)
		{
			if (list.Count == 0)
			{
				return;
			}
			
			int num = h;
			int num2 = d;
			T y = list[(d + h) / 2];
			
			do
			{
				if (direction == Order.Ascending)
				{
					while (comparison(list[num2], y) < 0)
					{
						num2++;
					}
					
					while (comparison(list[num], y) > 0)
					{
						num--;
					}
				}
				else
				{
					while (comparison(list[num2], y) > 0)
					{
						num2++;
					}
					
					while (comparison(list[num], y) < 0)
					{
						num--;
					}
				}
				
				if (num >= num2)
				{
					if (num != num2)
					{
						T value = list[num];
						list[num] = list[num2];
						list[num2] = value;
					}
					
					num--;
					num2++;
				}
			}
			while (num2 <= num);
			
			if (d < num)
			{
				ArrayTools.QSort_r<T>(list, comparison, d, num, direction);
			}
			
			if (num2 < h)
			{
				ArrayTools.QSort_r<T>(list, comparison, num2, h, direction);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static void QSort<T>(IList<T> list) where T : IComparable<T> =>
			ArrayTools.QSort_r<T>(list, 0, list.Count - 1, Order.Ascending);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		private static void QSort_r<T>(IList<T> list, int d, int h, Order direction) 
			where T : IComparable<T>
		{
			if (list.Count == 0)
			{
				return;
			}
			
			int num1 = h;
			int num2 = d;
			T other = list[(d + h) / 2];
			
			do
			{
				if (direction == Order.Ascending)
				{
					while (list[num2].CompareTo(other) < 0)
					{
						num2++;
					}
					
					while (list[num1].CompareTo(other) > 0)
					{
						num1--;
					}
				}
				else
				{
					while (list[num2].CompareTo(other) > 0)
					{
						num2++;
					}
					
					while (list[num1].CompareTo(other) < 0)
					{
						num1--;
					}
				}

				if (num1 >= num2)
				{
					if (num1 != num2)
					{
						T obj = list[num1];
						list[num1] = list[num2];
						list[num2] = obj;
					}

					num1--;
					num2++;
				}
			}
			while (num2 <= num1);

			if (d < num1)
			{
				ArrayTools.QSort_r<T>(list, d, num1, direction);
			}
			
			if (num2 >= h)
			{
				return;
			}
			
			ArrayTools.QSort_r<T>(list, num2, h, direction);
		}
	}
}
