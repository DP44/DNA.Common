using System;

namespace DNA.Security.Cryptography.Asn1
{
	public class OidTokenizer
	{
		private string oid;

		private int index;

		public OidTokenizer(string oid)
		{
			this.oid = oid;
		}

		public bool HasMoreTokens
		{
			get
			{
				return this.index != -1;
			}
		}

		public string NextToken()
		{
			if (this.index == -1)
			{
				return null;
			}
			int num = this.oid.IndexOf('.', this.index);
			if (num == -1)
			{
				string result = this.oid.Substring(this.index);
				this.index = -1;
				return result;
			}
			string result2 = this.oid.Substring(this.index, num - this.index);
			this.index = num + 1;
			return result2;
		}
	}
}
