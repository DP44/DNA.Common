using System;
using System.ComponentModel;
using System.Management;

namespace DNA.Diagnostics.Instrumentation
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class LogicalDriveInfo
	{
		private string _name;

		private LogicalDriveType _driveType;

		private string _fileSystem;

		private ulong _freeSpace;

		private ulong _size;

		private string _volumeSerialNumber;

		public string Name
		{
			get
			{
				return this._name;
			}
		}

		public LogicalDriveType DriveType
		{
			get
			{
				return this._driveType;
			}
		}

		public string FileSystem
		{
			get
			{
				return this._fileSystem;
			}
		}

		public string VolumeSerialNumber
		{
			get
			{
				return this._volumeSerialNumber;
			}
		}

		public ulong FreeSpace
		{
			get
			{
				return this._freeSpace;
			}
		}

		public ulong Size
		{
			get
			{
				return this._size;
			}
		}

		public override string ToString()
		{
			return this.Name + " " + this.DriveType.ToString();
		}

		internal LogicalDriveInfo(ManagementObject mo)
		{
			this._name = mo["Caption"].ToString();
			this._driveType = (LogicalDriveType)((uint)mo["DriveType"]);
			if (mo["FileSystem"] != null)
			{
				this._fileSystem = mo["FileSystem"].ToString();
			}
			if (mo["FreeSpace"] != null)
			{
				this._freeSpace = (ulong)mo["FreeSpace"];
			}
			if (mo["Size"] != null)
			{
				this._size = (ulong)mo["Size"];
			}
			if (mo["VolumeSerialNumber"] != null)
			{
				this._volumeSerialNumber = mo["VolumeSerialNumber"].ToString();
			}
		}
	}
}
