using System;

namespace DNA.Net.GamerServices
{
	public class SignedOutEventArgs : EventArgs
	{
		public SignedOutEventArgs(SignedInGamer gamer) =>
			throw new NotImplementedException();

		public SignedInGamer Gamer =>
			throw new NotImplementedException();
	}
}
