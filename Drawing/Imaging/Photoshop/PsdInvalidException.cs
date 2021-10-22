using System;

namespace DNA.Drawing.Imaging.Photoshop
{
	[Serializable]
	public class PsdInvalidException : Exception
	{
		public PsdInvalidException()
		{
		}

		public PsdInvalidException(string message) : base(message)
		{
		}
	}
}
