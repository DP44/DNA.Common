using System;

namespace DNA.IO.VersionControl
{
	public class NullSourceControlProvider : ISourceControlProvider
	{
		public void Edit(string path) {}

		public bool Add(string path)
		{
			return true;
		}

		public int Add(string[] path)
		{
			return path.Length;
		}

		public void Delete(string path) {}
		public void SubmitChanges() {}

		public bool IsLocalPathMapped(string path)
		{
			return true;
		}
	}
}
