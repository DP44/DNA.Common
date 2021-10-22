using System;
using DNA.Net.Lidgren;

namespace DNA.Net.GamerServices
{
	public class VersionCheckedMessage
	{
		public enum ReadResultCode
		{
			Unset = -1,
			Success,
			GameNameInvalid,
			VersionInvalid,
			LocalVersionIsHIgher,
			LocalVersionIsLower
		}

		public VersionCheckedMessage.ReadResultCode ReadResult = 
			VersionCheckedMessage.ReadResultCode.Unset;

		public int NetworkVersion;
		public string GameName;

		public bool ReadAndValidate(NetBuffer msg, string validGameName, int validNetworkVersion)
		{
			this.ReadResult = VersionCheckedMessage.ReadResultCode.Unset;
			
			try
			{
				this.GameName = msg.ReadString();
			
				if (this.GameName != validGameName)
				{
					this.ReadResult = VersionCheckedMessage.ReadResultCode.GameNameInvalid;
				}
			}
			catch
			{
				this.ReadResult = VersionCheckedMessage.ReadResultCode.GameNameInvalid;
			}
			
			if (this.ReadResult == VersionCheckedMessage.ReadResultCode.Unset)
			{
				try
				{
					this.NetworkVersion = msg.ReadInt32();
					
					if (validNetworkVersion > this.NetworkVersion)
					{
						this.ReadResult = 
							VersionCheckedMessage.ReadResultCode.LocalVersionIsHIgher;
					}
					else if (validNetworkVersion < this.NetworkVersion)
					{
						this.ReadResult = 
							VersionCheckedMessage.ReadResultCode.LocalVersionIsLower;
					}
				}
				catch
				{
					this.ReadResult = VersionCheckedMessage.ReadResultCode.VersionInvalid;
				}
			
				if (this.ReadResult == VersionCheckedMessage.ReadResultCode.Unset)
				{
					this.ReadResult = VersionCheckedMessage.ReadResultCode.Success;
				}
			}

			return this.ReadResult == VersionCheckedMessage.ReadResultCode.Success;
		}

		public void Write(NetBuffer msg, string validGameName, int validNetworkVersion)
		{
			// This could be what we're seeing from when we connect to a server.
			msg.Write(validGameName);
			msg.Write(validNetworkVersion);
		}
	}
}
