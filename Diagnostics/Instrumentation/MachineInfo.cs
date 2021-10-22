using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Management;
using System.Net;
using System.Text;
using DNA.Runtime.InteropServices.Win32;
using DNA.Runtime.InteropServices.Win32.IPHelper;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Diagnostics.Instrumentation
{
	public class MachineInfo
	{
		private const int MaxIDEDrives = 16;

		private static MachineInfo _localInfo;

		private bool _wmiSupported;

		private MemoryInfo _memoryInfo;

		private HardDiskInfo[] _hardDisks = new HardDiskInfo[0];

		private LogicalDriveInfo[] _logicalDrives = new LogicalDriveInfo[0];

		private NetworkAdapterInfo[] _networkAdapters = new NetworkAdapterInfo[0];

		private ProcessorInfo[] _processors = new ProcessorInfo[0];

		private string _machineName;

		private string _currentUser;

		private Version _CLRVersion;

		private OperatingSystem _osVersion;

		private bool _64BitOS;

		private MachineID _machineID;

		public static MachineInfo LocalInfo
		{
			get
			{
				if (MachineInfo._localInfo == null)
				{
					MachineInfo._localInfo = new MachineInfo();
					MachineInfo._localInfo.GetLocalInfo();
				}
				else
				{
					MachineInfo._localInfo.UpdateLocalInfo();
				}
				return MachineInfo._localInfo;
			}
		}

		public bool ManagementSupported
		{
			get
			{
				return this._wmiSupported;
			}
		}

		public string MachineName
		{
			get
			{
				return this._machineName;
			}
		}

		public string CurrentUser
		{
			get
			{
				return this._currentUser;
			}
		}

		public OperatingSystem OSVersion
		{
			get
			{
				return this._osVersion;
			}
		}

		public bool IsOS64Bit
		{
			get
			{
				return this._64BitOS;
			}
		}

		public Version CLRVersion
		{
			get
			{
				return this._CLRVersion;
			}
		}

		public MemoryInfo MemoryInfo
		{
			get
			{
				return this._memoryInfo;
			}
		}

		public HardDiskInfo[] HardDisks
		{
			get
			{
				return this._hardDisks;
			}
		}

		public NetworkAdapterInfo[] NetworkAdapters
		{
			get
			{
				return this._networkAdapters;
			}
		}

		public ProcessorInfo[] Processors
		{
			get
			{
				return this._processors;
			}
		}

		public LogicalDriveInfo[] LogicalDrives
		{
			get
			{
				return this._logicalDrives;
			}
		}

		public string GetReport()
		{
			StringBuilder stringBuilder = new StringBuilder();
			StringWriter stringWriter = new StringWriter(stringBuilder);
			try
			{
				stringWriter.WriteLine("Computer Details:");
				stringWriter.WriteLine("Machine Name:\t" + this.MachineName);
				stringWriter.WriteLine("Current Time:\t" + DateTime.Now.ToString("F", CultureInfo.CreateSpecificCulture("en-US")));
				stringWriter.WriteLine("Current User:\t" + this.CurrentUser);
				stringWriter.WriteLine(string.Concat(new string[]
				{
					"OS:\t\t",
					this.OSVersion.Platform.ToString(),
					"(",
					this.OSVersion.Version.ToString(),
					")"
				}));
				stringWriter.WriteLine("64bit:\t\t" + (this.IsOS64Bit ? "Yes" : "No"));
				stringWriter.WriteLine("CLR:\t\t" + this.CLRVersion);
				stringWriter.WriteLine();
				if (this._wmiSupported)
				{
					int num = (int)Math.Round((double)(this.MemoryInfo.MemoryLoad * 100f));
					stringWriter.WriteLine("Memory:");
					stringWriter.WriteLine("\tLoad:\t\t\t" + num.ToString() + "%");
					stringWriter.WriteLine("\tWorking Set:\t\t" + this.MemoryInfo.WorkingSet.ToString("n0") + "\tbytes");
					stringWriter.WriteLine("\tTotal:");
					stringWriter.WriteLine("\t\tPhysical:\t\t" + this.MemoryInfo.TotalPhysical.ToString("n0") + "\tbytes");
					stringWriter.WriteLine("\t\tPage File:\t\t" + this.MemoryInfo.TotalPageFile.ToString("n0") + "\tbytes");
					stringWriter.WriteLine("\t\tVirtual:\t\t" + this.MemoryInfo.TotalVirtual.ToString("n0") + "\tbytes");
					stringWriter.WriteLine("\tAvailible:");
					stringWriter.WriteLine("\t\tPhysical:\t\t" + this.MemoryInfo.AvailiblePhysical.ToString("n0") + "\tbytes");
					stringWriter.WriteLine("\t\tPage File:\t\t" + this.MemoryInfo.AvailiblePageFile.ToString("n0") + "\tbytes");
					stringWriter.WriteLine("\t\tVirtual:\t\t" + this.MemoryInfo.AvailibleVirtual.ToString("n0") + "\tbytes");
					stringWriter.WriteLine("\t\tExtended Virtual:\t" + this.MemoryInfo.AvailibleExtendedVirtual.ToString("n0") + "\tbytes");
					stringWriter.WriteLine();
					stringWriter.WriteLine("Proccessor Info:");
					stringWriter.WriteLine("\tCount:\t" + this.Processors.Length);
					for (int i = 0; i < this.Processors.Length; i++)
					{
						ProcessorInfo processorInfo = this.Processors[i];
						stringWriter.WriteLine("\tProcessor[" + i.ToString() + "]:");
						stringWriter.WriteLine("\t\tName:\t\t" + processorInfo.Name);
						stringWriter.WriteLine("\t\tManufacturer:\t" + processorInfo.Manufacturer);
						stringWriter.WriteLine("\t\tID:\t\t" + processorInfo.ProcessorID);
						stringWriter.WriteLine("\t\tClockSpeedID:\t" + processorInfo.MaxClockSpeed.ToString("n0") + " Mhz");
						stringWriter.WriteLine("\t\tCores:\t\t" + processorInfo.NumberOfCores.ToString());
						stringWriter.WriteLine("\t\tLogicalProcessors:\t" + processorInfo.NumberOfLogicalProcessors.ToString());
						string str = "NoID";
						if (processorInfo.UniqueID != null)
						{
							str = processorInfo.UniqueID;
						}
						stringWriter.WriteLine("\t\tUniqueID:\t" + str);
					}
					stringWriter.WriteLine();
					stringWriter.WriteLine("HardDisks:");
					stringWriter.WriteLine("\tCount:\t" + this.HardDisks.Length.ToString());
					for (int j = 0; j < this.HardDisks.Length; j++)
					{
						HardDiskInfo hardDiskInfo = this.HardDisks[j];
						stringWriter.WriteLine("\tDisk[" + j.ToString() + "]:");
						stringWriter.WriteLine("\t\tModel:\t\t\t" + hardDiskInfo.Model);
						stringWriter.WriteLine("\t\tBusType:\t\t\t" + hardDiskInfo.BusType);
						string str2 = "No Serial Number";
						if (hardDiskInfo.SerialNumber != null)
						{
							str2 = hardDiskInfo.SerialNumber;
						}
						stringWriter.WriteLine("\t\tSerial Number:\t\t" + str2);
						string str3 = "No Serial Number";
						if (hardDiskInfo.SerialNumber != null)
						{
							str3 = hardDiskInfo.FriendlySerialNumber;
						}
						stringWriter.WriteLine("\t\tFriendly Serial Number:\t" + str3);
					}
					stringWriter.WriteLine();
					stringWriter.WriteLine("Logical Drives:");
					stringWriter.WriteLine("\tCount:" + this.LogicalDrives.Length);
					for (int k = 0; k < this.LogicalDrives.Length; k++)
					{
						LogicalDriveInfo logicalDriveInfo = this.LogicalDrives[k];
						stringWriter.WriteLine("\t" + logicalDriveInfo.Name);
						stringWriter.WriteLine("\t\tType:\t\t" + logicalDriveInfo.DriveType.ToString());
						stringWriter.WriteLine("\t\tTotal Size:\t" + logicalDriveInfo.Size.ToString("n0") + " bytes");
						stringWriter.WriteLine("\t\tFree Space:\t" + logicalDriveInfo.FreeSpace.ToString("n0") + " bytes");
					}
					stringWriter.WriteLine();
					stringWriter.WriteLine("Network Adapters:");
					stringWriter.WriteLine("\tCount:" + this.NetworkAdapters.Length.ToString());
					for (int l = 0; l < this.NetworkAdapters.Length; l++)
					{
						NetworkAdapterInfo networkAdapterInfo = this.NetworkAdapters[l];
						stringWriter.WriteLine("\tAdapter[" + l.ToString() + "]:");
						stringWriter.WriteLine("\t\tMAC Address:\t" + networkAdapterInfo.MACAddress);
						stringWriter.WriteLine("\t\tIP Addresses:");
						foreach (IPAddress ipaddress in networkAdapterInfo.IPAddresses)
						{
							stringWriter.WriteLine("\t\t\t" + ipaddress.ToString());
						}
					}
				}
				else
				{
					stringWriter.WriteLine("WMI Not Supported");
				}
				stringWriter.WriteLine();
				stringWriter.WriteLine("Video Hardware Details:");
				int count = GraphicsAdapter.Adapters.Count;
				stringWriter.WriteLine("\tAdapter Count:\t" + count.ToString());
				for (int n = 0; n < count; n++)
				{
					GraphicsAdapter graphicsAdapter = GraphicsAdapter.Adapters[n];
					stringWriter.WriteLine("\tAdapter[" + n.ToString() + "]:");
					stringWriter.WriteLine("\t\tName:\t\t" + graphicsAdapter.DeviceName);
					stringWriter.WriteLine("\t\tDescription:\t" + graphicsAdapter.Description);
					stringWriter.WriteLine("\t\tRevision:\t\t" + graphicsAdapter.Revision.ToString());
					stringWriter.WriteLine("\t\tHiDef Support:\t" + (graphicsAdapter.IsProfileSupported(GraphicsProfile.HiDef) ? "Yes" : "No"));
					stringWriter.WriteLine("\t\tReach Support:\t" + (graphicsAdapter.IsProfileSupported(GraphicsProfile.Reach) ? "Yes" : "No"));
					stringWriter.WriteLine("\t\tID:\t\t" + graphicsAdapter.DeviceId.ToString());
					stringWriter.WriteLine("\t\tVendor ID:\t" + graphicsAdapter.VendorId.ToString("X"));
					stringWriter.WriteLine("\t\tSub System ID:\t" + graphicsAdapter.SubSystemId.ToString("X"));
					stringWriter.WriteLine("\t\tWideScreen:\t" + (graphicsAdapter.IsWideScreen ? "Yes" : "No"));
					stringWriter.WriteLine();
					stringWriter.WriteLine("\tSupported Display Modes:");
					foreach (DisplayMode displayMode in graphicsAdapter.SupportedDisplayModes)
					{
						stringWriter.WriteLine("\t\t" + displayMode.ToString());
					}
					stringWriter.WriteLine();
					stringWriter.WriteLine("\tCurrent Display Mode:" + graphicsAdapter.CurrentDisplayMode.ToString());
					stringWriter.WriteLine();
					stringWriter.WriteLine();
				}
			}
			catch (Exception ex)
			{
				stringWriter.WriteLine(ex.Message + "\n" + ex.StackTrace);
			}
			stringWriter.Flush();
			return stringBuilder.ToString();
		}

		private static int CompareProcsByMemory(Process p1, Process p2)
		{
			return p2.WorkingSet64.CompareTo(p1.WorkingSet64);
		}

		public override string ToString()
		{
			return this.GetReport();
		}

		private void UpdateLocalInfo()
		{
			this.GetMemory();
		}

		private void GetLocalInfo()
		{
			this._machineName = Environment.MachineName;
			this._currentUser = Environment.UserDomainName + "\\" + Environment.UserName;
			this._osVersion = Environment.OSVersion;
			this._CLRVersion = Environment.Version;
			this._64BitOS = Environment.Is64BitOperatingSystem;
			try
			{
				this.GetProcessors();
				this.GetLogicalDrives();
				this._wmiSupported = true;
			}
			catch
			{
				this._logicalDrives = new LogicalDriveInfo[0];
				this._processors = new ProcessorInfo[0];
				this._wmiSupported = false;
			}
			this.GetHardDisks();
			this.GetNetworkAdapters();
			this.UpdateLocalInfo();
			this._machineID = new MachineID(this);
		}

		private void GetProcessors()
		{
			List<ProcessorInfo> list = new List<ProcessorInfo>();
			ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
			foreach (ManagementBaseObject managementBaseObject in managementObjectSearcher.Get())
			{
				ManagementObject mo = (ManagementObject)managementBaseObject;
				list.Add(new ProcessorInfo(mo));
			}
			this._processors = list.ToArray();
		}

		private void GetMemory()
		{
			Win32MemoryStatusEx stat = Win32API.GlobalMemoryStatus();
			this._memoryInfo = new MemoryInfo(stat);
		}

		private void GetHardDisks()
		{
			List<HardDiskInfo> hardDiskInfoList = new List<HardDiskInfo>();
			
			if (this._wmiSupported)
			{
				foreach (ManagementObject wmi_HD 
					in new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive").Get())
				{
					hardDiskInfoList.Add(HardDiskInfo.FromManagmentObject(wmi_HD));
				}
			}
			else
			{
				for (int index = 0; index < 16; ++index)
				{
					string physicalDriveName = "\\\\.\\PhysicalDrive" + index.ToString();
					
					try
					{
						hardDiskInfoList.Add(HardDiskInfo.GetPhysicalDriveInfo(physicalDriveName));
					}
					catch
					{
						// Swallow exceptions.
					}
				}
			}

			this._hardDisks = hardDiskInfoList.ToArray();
		}

		private void GetLogicalDrives()
		{
			List<LogicalDriveInfo> list = new List<LogicalDriveInfo>();
			ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_LogicalDisk");
			foreach (ManagementBaseObject managementBaseObject in managementObjectSearcher.Get())
			{
				ManagementObject mo = (ManagementObject)managementBaseObject;
				list.Add(new LogicalDriveInfo(mo));
			}
			this._logicalDrives = list.ToArray();
		}

		private static void PrintObjectProperties(ManagementObject mo)
		{
			foreach (PropertyData propertyData in mo.Properties)
			{
				if (propertyData.Value != null)
				{
					Console.WriteLine(propertyData.Name + " = " + propertyData.Value.ToString());
				}
			}
			Console.WriteLine();
		}

		private void GetNetworkAdapters()
		{
			if (this._wmiSupported)
			{
				ManagementClass managementClass = new ManagementClass("Win32_NetworkAdapter");
				ManagementObjectCollection instances = managementClass.GetInstances();
				Dictionary<string, ManagementObject> dictionary = new Dictionary<string, ManagementObject>();
				Dictionary<string, bool> dictionary2 = new Dictionary<string, bool>();
				foreach (ManagementBaseObject managementBaseObject in instances)
				{
					ManagementObject managementObject = (ManagementObject)managementBaseObject;
					try
					{
						ushort num = (ushort)managementObject["AdapterTypeID"];
						if (num != 0)
						{
							continue;
						}
					}
					catch
					{
						continue;
					}
					bool flag = false;
					string str = (string)managementObject["DeviceID"];
					try
					{
						flag = (bool)managementObject["PhysicalAdapter"];
						if (!flag)
						{
							continue;
						}
					}
					catch
					{
						ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("ASSOCIATORS OF {Win32_NetworkAdapter.DeviceID='" + str + "'} WHERE ResultClass = Win32_PnPEntity");
						ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();
						if (managementObjectCollection.Count != 0)
						{
							flag = true;
						}
					}
					if (flag)
					{
						string key = (string)managementObject["MacAddress"];
						dictionary[key] = managementObject;
						ManagementObjectSearcher managementObjectSearcher2 = new ManagementObjectSearcher("ASSOCIATORS OF {Win32_NetworkAdapter.DeviceID='" + str + "'} WHERE ResultClass = Win32_IRQResource");
						ManagementObjectCollection managementObjectCollection2 = managementObjectSearcher2.Get();
						if (managementObjectCollection2.Count != 0)
						{
							dictionary2[key] = true;
						}
						else if (!dictionary2.ContainsKey(key))
						{
							dictionary2[key] = false;
						}
					}
				}
				managementClass = new ManagementClass("Win32_NetworkAdapterConfiguration");
				instances = managementClass.GetInstances();
				Dictionary<string, NetworkAdapterInfo> dictionary3 = new Dictionary<string, NetworkAdapterInfo>();
				foreach (ManagementBaseObject managementBaseObject2 in instances)
				{
					ManagementObject managementObject2 = (ManagementObject)managementBaseObject2;
					try
					{
						string key2 = (string)managementObject2["MacAddress"];
						if (dictionary.ContainsKey(key2))
						{
							NetworkAdapterInfo networkAdapterInfo = NetworkAdapterInfo.FromManagmentObject(managementObject2, dictionary2[key2]);
							NetworkAdapterInfo networkAdapterInfo2;
							if (dictionary3.TryGetValue(networkAdapterInfo.MACAddress, out networkAdapterInfo2))
							{
								if (networkAdapterInfo.IPAddresses.Length > networkAdapterInfo2.IPAddresses.Length)
								{
									dictionary3[networkAdapterInfo.MACAddress] = networkAdapterInfo;
								}
							}
							else
							{
								dictionary3[networkAdapterInfo.MACAddress] = networkAdapterInfo;
							}
						}
					}
					catch
					{
					}
				}
				NetworkAdapterInfo[] array = new NetworkAdapterInfo[dictionary3.Values.Count];
				dictionary3.Values.CopyTo(array, 0);
				this._networkAdapters = array;
				return;
			}
			this._networkAdapters = IPHelperAPI.GetAdaptersInfo();
		}

		private MachineInfo()
		{
		}
	}
}
