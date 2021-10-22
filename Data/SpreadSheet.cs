using System;
using System.Collections.Generic;

namespace DNA.Data
{
	public class SpreadSheet<T>
	{
		public class Page
		{
			private T[,] Cells = new T[0, 0];

			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			public T this[int row, int column]
			{
				get
				{
					return this.Cells[row, column];
				}
				set
				{
					this.Cells[row, column] = value;
				}
			}

			/// <summary>
			/// 
			/// </summary>
			public int RowCount
			{
				get
				{
					return this.Cells.GetLength(0);
				}
			}

			/// <summary>
			/// 
			/// </summary>
			public int ColumnCount
			{
				get
				{
					return this.Cells.GetLength(1);
				}
			}

			/// <summary>
			/// 
			/// </summary>
			public Page() {}

			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			public Page(int rows, int cols)
			{
				this.Init(rows, cols);
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			public void Init(int rows, int columns)
			{
				this.Cells = new T[rows, columns];
			}
		}

		public List<SpreadSheet<T>.Page> Pages = new List<SpreadSheet<T>.Page>();
	}
}
