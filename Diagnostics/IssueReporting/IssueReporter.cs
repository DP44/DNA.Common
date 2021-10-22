using System;

namespace DNA.Diagnostics.IssueReporting
{
	public class IssueReporter
	{
		public virtual void ReportBug()
		{
		}

		public virtual void ReportCrash(Exception e)
		{
		}

		public virtual void ReportStat(string stat, string value)
		{
		}
	}
}
