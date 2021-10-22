using System;
using System.ComponentModel;
using System.Management;

namespace DNA.Diagnostics.Instrumentation
{
	[TypeConverter(typeof(ExpandableObjectConverter)), Serializable]
	public class ProcessorInfo
	{
		private string _manufacturer;

		private string _uniqueID;

		private string _processorID;

		private string _name;

		private int _maxClockSpeed;

		private int _numberOfCores;

		private int _numberOfLogicalProcessors;

		public string Manufacturer
		{
			get
			{
				return this._manufacturer;
			}
		}

		public string UniqueID
		{
			get
			{
				return this._uniqueID;
			}
		}

		public string ProcessorID
		{
			get
			{
				return this._processorID;
			}
		}

		public int MaxClockSpeed
		{
			get
			{
				return this._maxClockSpeed;
			}
		}

		public string Name
		{
			get
			{
				return this._name;
			}
		}

		public int NumberOfCores
		{
			get
			{
				return this._numberOfCores;
			}
		}

		public int NumberOfLogicalProcessors
		{
			get
			{
				return this._numberOfLogicalProcessors;
			}
		}

		internal ProcessorInfo(ManagementObject mo)
		{
			if (mo.Properties["UniqueId"].Value == null)
			{
				this._uniqueID = null;
			}
			else
			{
				this._uniqueID = mo.Properties["UniqueId"].Value.ToString();
			}
			this._manufacturer = mo.Properties["Manufacturer"].Value.ToString();
			this._processorID = mo.Properties["ProcessorId"].Value.ToString();
			this._maxClockSpeed = int.Parse(mo.Properties["MaxClockSpeed"].Value.ToString());
			if (mo.Properties["Name"].Value == null)
			{
				this._name = null;
			}
			else
			{
				this._name = mo.Properties["Name"].Value.ToString();
			}
			this._numberOfCores = int.Parse(mo.Properties["NumberOfCores"].Value.ToString());
			this._numberOfLogicalProcessors = int.Parse(mo.Properties["NumberOfLogicalProcessors"].Value.ToString());
		}
	}
}
