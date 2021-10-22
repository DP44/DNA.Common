using System;
using System.Collections.Generic;
using System.IO;
using DNA.IO.Storage;

namespace DNA
{
	public class CheatCode
	{
		public class CheatCodeManager
		{
			private static string CodeFileName = "cdat.user";

			private SaveDevice _saveDevice;

			public Dictionary<string, CheatCode> Codes = new Dictionary<string, CheatCode>();

			public CheatCodeManager(SaveDevice saveDevice)
			{
				this._saveDevice = saveDevice;
			}

			public CheatCode RegisterCode(string systemName, string reward, string unlockCode, object tag)
			{
				CheatCode cheatCode = new CheatCode(this, systemName, reward, unlockCode, tag);
				this.Codes[cheatCode.SystemName] = cheatCode;
				return cheatCode;
			}

			public List<CheatCode> GetRedeemedCodes()
			{
				List<CheatCode> list = new List<CheatCode>();
				foreach (KeyValuePair<string, CheatCode> keyValuePair in this.Codes)
				{
					if (keyValuePair.Value.Redeemed)
					{
						list.Add(keyValuePair.Value);
					}
				}
				return list;
			}

			public CheatCode GetDisplayCode(string name, string description, string unlockCode, object tag)
			{
				return new CheatCode(null, name, description, unlockCode, tag);
			}

			public void LoadCodes()
			{
				try
				{
					this._saveDevice.Load(CheatCode.CheatCodeManager.CodeFileName, delegate(Stream stream)
					{
						BinaryReader binaryReader = new BinaryReader(stream);
						int num = binaryReader.ReadInt32();
						for (int i = 0; i < num; i++)
						{
							string key = binaryReader.ReadString();
							CheatCode cheatCode;
							if (this.Codes.TryGetValue(key, out cheatCode))
							{
								cheatCode._redeemed = true;
							}
						}
					});
				}
				catch
				{
				}
			}

			private void SaveCodes()
			{
				try
				{
					this._saveDevice.Save(CheatCode.CheatCodeManager.CodeFileName, true, true, delegate(Stream stream)
					{
						List<CheatCode> redeemedCodes = this.GetRedeemedCodes();
						if (redeemedCodes.Count == 0)
						{
							return;
						}
						BinaryWriter binaryWriter = new BinaryWriter(stream);
						binaryWriter.Write(redeemedCodes.Count);
						foreach (CheatCode cheatCode in redeemedCodes)
						{
							binaryWriter.Write(cheatCode.SystemName);
						}
						binaryWriter.Flush();
					});
				}
				catch
				{
				}
			}

			public void Redeem(CheatCode code)
			{
				code._redeemed = true;
				this.SaveCodes();
			}

			public CheatCode Redeem(string code, out string reason)
			{
				string a = code.ToUpper();
				reason = "Invalid Code";
				foreach (KeyValuePair<string, CheatCode> keyValuePair in this.Codes)
				{
					if (a == keyValuePair.Value.Cheatcode)
					{
						if (!keyValuePair.Value.Redeemed)
						{
							keyValuePair.Value._redeemed = true;
							reason = "Success";
							this.SaveCodes();
							return keyValuePair.Value;
						}
						reason = "Code Already Redeemed";
					}
				}
				return null;
			}
		}

		private string _systemName;

		private string _reward;

		private bool _redeemed;

		private string _cheatCode;

		public object Tag;

		private CheatCode.CheatCodeManager _manager;

		public string Reward
		{
			get
			{
				return this._reward;
			}
		}

		public string SystemName
		{
			get
			{
				return this._systemName;
			}
		}

		public bool Redeemed
		{
			get
			{
				return this._redeemed;
			}
		}

		public string Cheatcode
		{
			get
			{
				return this._cheatCode;
			}
		}

		public CheatCode(CheatCode.CheatCodeManager manager, string systemName, string rewardDescription, string code, object tag)
		{
			this._manager = manager;
			this._reward = rewardDescription;
			this._systemName = systemName;
			this.Tag = tag;
			this._cheatCode = code.ToUpper();
		}
	}
}
