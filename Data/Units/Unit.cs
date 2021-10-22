using System;
using System.Collections.Generic;

namespace DNA.Data.Units
{
	public abstract class Unit
	{
		private string[] _abbrevs;
		private string _name;

		protected static Dictionary<string, Unit> _lookupTable = new Dictionary<string, Unit>();

		public string[] Abbrevations
		{
			get
			{
				return this._abbrevs;
			}
		}

		public string Name
		{
			get
			{
				return this._name;
			}
		}

		protected abstract object SetUnit(double value);

		public static void RegisterUnit(Unit unit)
		{
			foreach (string key in unit.Abbrevations)
			{
				if (Unit._lookupTable.ContainsKey(key))
				{
					throw new ArgumentException("Unit Already Registered");
				}

				Unit._lookupTable[key] = unit;
			}
		}

		public static Unit ParseUnit(string unitStr)
		{
			return Unit._lookupTable[unitStr];
		}

		public static object Convert(double val, string unit)
		{
			return Unit.Convert(val, Unit.ParseUnit(unit));
		}

		public static object Convert(double val, Unit unit)
		{
			return unit.SetUnit(val);
		}

		public override string ToString()
		{
			return this._name;
		}

		public Unit(string[] abbrivations, string name)
		{
			this._abbrevs = abbrivations;
			this._name = name;
		}
	}
}
