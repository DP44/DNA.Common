using System;
using System.Security;

namespace DNA.Security.Cryptography.Utilities
{
	internal sealed class Platform
	{
		internal static readonly string NewLine = Platform.GetNewLine();

		private Platform()
		{
		}

		internal static Exception CreateNotImplementedException(string message)
		{
			return new NotImplementedException(message);
		}

		internal static string GetEnvironmentVariable(string variable)
		{
			string result;
			try
			{
				result = Environment.GetEnvironmentVariable(variable);
			}
			catch (SecurityException)
			{
				result = null;
			}
			return result;
		}

		private static string GetNewLine()
		{
			return Environment.NewLine;
		}
	}
}
