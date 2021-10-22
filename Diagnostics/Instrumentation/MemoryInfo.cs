using System;
using System.ComponentModel;
using DNA.Runtime.InteropServices.Win32;

namespace DNA.Diagnostics.Instrumentation
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class MemoryInfo
	{
		private Win32MemoryStatusEx _memstat;

		private long _workingSet;

		public float MemoryLoad
		{
			get
			{
				return this._memstat.MemoryLoad / 100f;
			}
		}

		public long WorkingSet
		{
			get
			{
				return this._workingSet;
			}
		}

		public ulong TotalPhysical
		{
			get
			{
				return this._memstat.TotalPhysical;
			}
		}

		public ulong AvailiblePhysical
		{
			get
			{
				return this._memstat.AvailiblePhysical;
			}
		}

		public ulong TotalPageFile
		{
			get
			{
				return this._memstat.TotalPageFile;
			}
		}

		public ulong AvailiblePageFile
		{
			get
			{
				return this._memstat.AvailiblePageFile;
			}
		}

		public ulong TotalVirtual
		{
			get
			{
				return this._memstat.TotalVirtual;
			}
		}

		public ulong AvailibleVirtual
		{
			get
			{
				return this._memstat.AvailibleVirtual;
			}
		}

		public ulong AvailibleExtendedVirtual
		{
			get
			{
				return this._memstat.AvailibleExtendedVirtual;
			}
		}

		public MemoryInfo(Win32MemoryStatusEx stat)
		{
			this._memstat = stat;
			this._workingSet = Environment.WorkingSet;
		}
	}
}
