using System;
using System.Text;
using DNA.Security.Cryptography;

namespace DNA.Diagnostics.Instrumentation
{
	public class MachineID
	{
		private Hash _hash;

		private static MachineID _localID;

		public static MachineID LocalID
		{
			get
			{
				if (MachineID._localID == null)
				{
					MachineID._localID = new MachineID(MachineInfo.LocalInfo);
				}

				return MachineID._localID;
			}
		}

		public byte[] ToByteArray() => 
			(byte[])this._hash.Data.Clone();

		public override string ToString() => 
			this._hash.ToString();

		public MachineID(string midString) => 
			this._hash = new MD5HashProvider().Parse(midString);

		public MachineID(byte[] data) => 
			this._hash = new MD5HashProvider().CreateHash(data);

		public MachineID(MachineInfo info)
		{
			string str = "";
		
			if (info.Processors != null && info.Processors.Length > 0)
			{
				str = info.Processors[0].ProcessorID;
			}
		
			string str2 = "";
		
			foreach (NetworkAdapterInfo networkAdapterInfo in info.NetworkAdapters)
			{
				if (networkAdapterInfo.Physical && 
					!string.IsNullOrEmpty(networkAdapterInfo.MACAddress))
				{
					str2 = networkAdapterInfo.MACAddress;
					break;
				}
			}
		
			string text = "";
		
			foreach (HardDiskInfo hardDiskInfo in info.HardDisks)
			{
				if (!string.IsNullOrEmpty(hardDiskInfo.SerialNumber))
				{
					if (hardDiskInfo.BusType == "ATA" || hardDiskInfo.BusType == "SCSI")
					{
						text = hardDiskInfo.SerialNumber;
						break;
					}
				
					if (string.IsNullOrEmpty(text))
					{
						text = hardDiskInfo.SerialNumber;
					}
				}
			}
		
			string text2 = "";
			
			foreach (HardDiskInfo hardDiskInfo2 in info.HardDisks)
			{
				if (!string.IsNullOrEmpty(hardDiskInfo2.Model))
				{
					if (hardDiskInfo2.BusType == "ATA" || hardDiskInfo2.BusType == "SCSI")
					{
						text2 = hardDiskInfo2.Model;
						break;
					}
				
					if (string.IsNullOrEmpty(text2))
					{
						text2 = hardDiskInfo2.Model;
					}
				}
			}
			
			MD5HashProvider md5HashProvider = new MD5HashProvider();
			string s = str2 + text + str + text2;
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			this._hash = md5HashProvider.Compute(bytes);
		}
	}
}
