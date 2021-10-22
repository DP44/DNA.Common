using System;

namespace DNA.Drawing.Imaging.Photoshop
{
	[Serializable]
	public class RleException : Exception
	{
		public RleException()
		{
		}

		public RleException(string message) : base(message)
		{
		}
	}
}
