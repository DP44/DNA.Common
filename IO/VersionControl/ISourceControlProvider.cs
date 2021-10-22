using System;

namespace DNA.IO.VersionControl
{
	public interface ISourceControlProvider
	{
		void Edit(string path);
		bool Add(string path);
		int Add(string[] paths);
		void Delete(string path);
		void SubmitChanges();
		bool IsLocalPathMapped(string path);
	}
}
