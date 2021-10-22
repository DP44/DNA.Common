using System;
using DNA.Net.MatchMaking;

namespace DNA 
{
	public delegate void GetPasswordForInvitedGameCallback(ClientSessionInfo sessionInfo, 
		object context, SetPasswordForInvitedGameCallback callback);
}
