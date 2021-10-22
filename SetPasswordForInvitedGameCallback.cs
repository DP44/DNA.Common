using System;

namespace DNA 
{
	public delegate void SetPasswordForInvitedGameCallback(bool cancelled, string password, 
														   string errorString, object context);
}
