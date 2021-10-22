using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace DNA.Runtime.InteropServices.Win32.Shell
{
	public static class ShellAPI
	{
		[DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
		public static extern Win32ErrorCode SHCreateStreamOnFile(string pszFile, StorageMode grfMode, out IStream ppstm);

		[DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
		private static extern Win32ErrorCode SHCreateStreamOnFileEx(string pszFile, StorageMode grfMode, Win32FileFlags dwAttributes, bool fCreate, IStream pstmTemplate, out IStream ppstm);

		[DllImport("Shlwapi.dll", CharSet = CharSet.Auto, EntryPoint = "PathRelativePathToW", SetLastError = true)]
		private static extern bool _PathRelativePathTo(StringBuilder pszPath, string pszFrom, FileAttributes dwAttrFrom, string pszTo, FileAttributes dwAttrTo);

		public static string PathRelativePathTo(string fromPath, bool isFromDirectory, string toPath, bool isToDirectory)
		{
			StringBuilder relativePath = new StringBuilder(260, 260);
			
			if (!ShellAPI._PathRelativePathTo(relativePath, fromPath, isFromDirectory ? FileAttributes.Directory : FileAttributes.Normal, toPath, isToDirectory ? FileAttributes.Directory : FileAttributes.Normal))
			{
				throw new Exception("PathRelativePathTo error");
			}
			
			return relativePath.ToString();
		}
	}
}
