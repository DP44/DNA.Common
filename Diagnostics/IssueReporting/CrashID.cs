using System;
using System.Text;
using DNA.Security.Cryptography;

namespace DNA.Diagnostics.IssueReporting
{
	public class CrashID
	{
		private Hash _hash;

		private CrashID(Hash hash)
		{
			this._hash = hash;
		}

		public static CrashID Parse(string idText)
		{
			MD5HashProvider md5HashProvider = new MD5HashProvider();
			return new CrashID(md5HashProvider.Parse(idText));
		}

		public static CrashID FromInfo(string type, string message, string stackTrace)
		{
			MD5HashProvider md5HashProvider = new MD5HashProvider();
			UTF8Encoding utf8Encoding = new UTF8Encoding();
			string s = type + message + stackTrace;
			byte[] bytes = utf8Encoding.GetBytes(s);
			return new CrashID(md5HashProvider.CreateHash(bytes));
		}

		public override string ToString()
		{
			return this._hash.ToString();
		}
	}
}
