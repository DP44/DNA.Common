using System;
using System.IO;
using System.IO.IsolatedStorage;
using DNA.Net.GamerServices;

namespace DNA.IO.Storage
{
	public class PlayerISOSaveDevice : IsolatedStorageSaveDevice
	{
		public SignedInGamer Gamer { get; private set; }

		internal PlayerISOSaveDevice(SignedInGamer gamer, byte[] key, IsolatedStorageFile container) : base(key)
		{
			this.Gamer = gamer;
			this._currentContainer = container;
			if (!this._currentContainer.DirectoryExists(this.Gamer.Gamertag))
			{
				this._currentContainer.CreateDirectory(this.Gamer.Gamertag);
			}
		}

		private string MakeRootRelative(string path)
		{
			return Path.Combine(this.Gamer.Gamertag, path);
		}

		public override void DeleteStorage()
		{
			lock (this)
			{
				if (this._currentContainer != null)
				{
					string[] directoryNames = this._currentContainer.GetDirectoryNames();
					if (directoryNames.Length > 1)
					{
						try
						{
							if (this._currentContainer.DirectoryExists(this.Gamer.Gamertag))
							{
								this._currentContainer.DeleteDirectory(this.Gamer.Gamertag);
								this._currentContainer.CreateDirectory(this.Gamer.Gamertag);
							}
							goto IL_90;
						}
						catch
						{
							goto IL_90;
						}
					}
					if (directoryNames.Length == 1 && directoryNames[0] == this.Gamer.Gamertag)
					{
						base.DeleteStorage();
					}
				}
				IL_90:;
			}
		}

		protected override Stream DeviceOpenFile(string fileName, FileMode mode, FileAccess access, FileShare share)
		{
			string fileName2 = this.MakeRootRelative(fileName);
			return base.DeviceOpenFile(fileName2, mode, access, share);
		}

		protected override void DeviceDeleteFile(string fileName)
		{
			base.DeviceDeleteFile(this.MakeRootRelative(fileName));
		}

		protected override bool DeviceFileExists(string fileName)
		{
			return base.DeviceFileExists(this.MakeRootRelative(fileName));
		}

		protected override bool DeviceDirectoryExists(string dirName)
		{
			return base.DeviceDirectoryExists(this.MakeRootRelative(dirName));
		}

		protected override string[] DeviceGetDirectoryNames()
		{
			string[] array = base.DeviceGetDirectoryNames(this.Gamer.Gamertag);
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = array[i].Substring((this.Gamer.Gamertag + Path.DirectorySeparatorChar).Length);
			}
			return array;
		}

		protected override string[] DeviceGetFileNames()
		{
			string[] array = base.DeviceGetFileNames(this.Gamer.Gamertag);
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = array[i].Substring((this.Gamer.Gamertag + Path.DirectorySeparatorChar).Length);
			}
			return array;
		}

		protected override string[] DeviceGetFileNames(string pattern)
		{
			pattern = this.MakeRootRelative(pattern);
			string[] array = base.DeviceGetFileNames(pattern);
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = array[i].Substring((this.Gamer.Gamertag + Path.DirectorySeparatorChar).Length);
			}
			return array;
		}

		protected override string[] DeviceGetDirectoryNames(string pattern)
		{
			pattern = this.MakeRootRelative(pattern);
			string[] array = base.DeviceGetDirectoryNames(pattern);
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = array[i].Substring((this.Gamer.Gamertag + Path.DirectorySeparatorChar).Length);
			}
			return array;
		}

		protected override void DeviceCreateDirectory(string path)
		{
			base.DeviceCreateDirectory(this.MakeRootRelative(path));
		}

		protected override void DeviceDeleteDirectory(string path)
		{
			base.DeviceDeleteDirectory(this.MakeRootRelative(path));
		}

		public override void Save(string fileName, bool tamperProof, bool compressed, FileAction saveAction)
		{
			if (!this._currentContainer.DirectoryExists(this.Gamer.Gamertag))
			{
				this._currentContainer.CreateDirectory(this.Gamer.Gamertag);
			}
			base.Save(fileName, tamperProof, compressed, saveAction);
		}
	}
}
