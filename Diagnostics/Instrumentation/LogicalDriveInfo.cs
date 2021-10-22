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

		public string Name => 
			this._name;

		public LogicalDriveType DriveType => 
			this._driveType;

		public string FileSystem => 
			this._fileSystem;

		public string VolumeSerialNumber => 
			this._volumeSerialNumber;

		public ulong FreeSpace => 
			this._freeSpace;

		public ulong Size => 
			this._size;

		public override string ToString() => 
			this.Name + " " + this.DriveType.ToString();

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
