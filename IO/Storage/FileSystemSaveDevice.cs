using System;
using System.IO;

namespace DNA.IO.Storage
{
	public class FileSystemSaveDevice : SaveDevice
	{
		private string _rootPath;

		public FileSystemSaveDevice(string rootPath, byte[] key) : base(key)
		{
			this._rootPath = Path.GetFullPath(rootPath);
			
			if (this._rootPath[this._rootPath.Length - 1] != Path.DirectorySeparatorChar)
			{
				this._rootPath += Path.DirectorySeparatorChar;
			}
			
			if (!Directory.Exists(this._rootPath))
			{
				Directory.CreateDirectory(this._rootPath);
			}
		}

		private string MakeRootRelative(string path)
		{
			return Path.Combine(this._rootPath, path);
		}

		protected override Stream DeviceOpenFile(string fileName, FileMode mode, FileAccess access, FileShare share)
		{
			fileName = this.MakeRootRelative(fileName);
			return File.Open(fileName, mode, access, share);
		}

		protected override void DeviceDeleteFile(string fileName)
		{
			File.Delete(this.MakeRootRelative(fileName));
		}

		protected override bool DeviceFileExists(string fileName)
		{
			return File.Exists(this.MakeRootRelative(fileName));
		}

		protected override bool DeviceDirectoryExists(string dirName)
		{
			return Directory.Exists(this.MakeRootRelative(dirName));
		}

		protected override string[] DeviceGetDirectoryNames()
		{
			string[] directories = Directory.GetDirectories(this._rootPath);
			
			foreach (string directory in directories)
			{
				directory = directory.Substring(this._rootPath.Length);
			}
			
			return directories;
		}

		protected override string[] DeviceGetDirectoryNames(string pattern)
		{
			pattern = this.MakeRootRelative(pattern);
			string directoryName = Path.GetDirectoryName(pattern);
			string fileName = Path.GetFileName(pattern);
			string[] directories;
			
			if (string.IsNullOrEmpty(fileName))
			{
				directories = Directory.GetDirectories(directoryName);
			}
			else
			{
				directories = Directory.GetDirectories(directoryName, fileName);
			}

			foreach (string directory in directories)
			{
				directory = directory].Substring(this._rootPath.Length);
			}

			return directories;
		}

		protected override string[] DeviceGetFileNames()
		{
			string[] files = Directory.GetFiles(this._rootPath);

			foreach (string file in files)
			{
				file = file.Substring(this._rootPath.Length);
			}

			return files;
		}

		protected override string[] DeviceGetFileNames(string pattern)
		{
			pattern = this.MakeRootRelative(pattern);
			string directoryName = Path.GetDirectoryName(pattern);
			string fileName = Path.GetFileName(pattern);
			string[] files;
			
			if (string.IsNullOrEmpty(fileName))
			{
				files = Directory.GetFiles(directoryName);
			}
			else
			{
				files = Directory.GetFiles(directoryName, fileName);
			}
			
			foreach (string file in files)
			{
				file = file.Substring(this._rootPath.Length);
			}

			return files;
		}

		protected override void DeviceCreateDirectory(string path)
		{
			Directory.CreateDirectory(this.MakeRootRelative(path));
		}

		protected override void DeviceDeleteDirectory(string path)
		{
			Directory.Delete(this.MakeRootRelative(path), true);
		}

		public override void Flush() {}

		public override void DeleteStorage()
		{
			try
			{
				Directory.Delete(this._rootPath, true);
				Directory.CreateDirectory(this._rootPath);
			}
			catch
			{
				// Swallow exceptions.
			}
		}

		public override void DeviceDispose() {}
	}
}
