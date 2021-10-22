using System;

namespace DNA.Diagnostics.IssueReporting
{
	public class BlackScreenIssueReporter : IssueReporter
	{
		private string _errorURL;

		private string _name;

		private Version _version;

		private DateTime _gameStartTime;

		public BlackScreenIssueReporter(string errorURL, string name, Version version, DateTime gameStartTime)
		{
			this._errorURL = errorURL;
			this._name = name;
			this._version = version;
			this._gameStartTime = gameStartTime;
		}

		public override void ReportBug()
		{
		}

		public override void ReportCrash(Exception e)
		{
			using (ExceptionGame exceptionGame = new ExceptionGame(e, this._errorURL, this._name, this._version, this._gameStartTime))
			{
				exceptionGame.Run();
			}
		}

		public override void ReportStat(string stat, string value)
		{
		}
	}
}
