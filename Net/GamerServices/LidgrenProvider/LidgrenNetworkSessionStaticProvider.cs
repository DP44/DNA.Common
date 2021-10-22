using System;
using System.Collections.Generic;
using System.Threading;
using DNA.Net.MatchMaking;

namespace DNA.Net.GamerServices.LidgrenProvider
{
	public class LidgrenNetworkSessionStaticProvider : NetworkSessionStaticProvider
	{
		protected override NetworkSession CreateSession() =>
			LidgrenNetworkSessionProvider.CreateNetworkSession(this);

		protected override void FinishBeginCreate(
			NetworkSessionStaticProvider.BeginCreateSessionState state
		) =>
			base.TaskScheduler.QueueUserWorkItem(
				new ParameterizedThreadStart(this.CreateSessionsThread), state);

		protected virtual void CreateSessionsThread(object state)
		{
			NetworkSessionStaticProvider.BeginCreateSessionState beginCreateSessionState = 
				(NetworkSessionStaticProvider.BeginCreateSessionState)state;
			
			beginCreateSessionState.Session.StartHost(beginCreateSessionState);
			beginCreateSessionState.Event.Set();
			
			if (beginCreateSessionState.Callback != null)
			{
				beginCreateSessionState.Callback(beginCreateSessionState);
			}
		}

		protected override void FinishBeginJoin(
			NetworkSessionStaticProvider.BeginJoinSessionState state
		) =>
			base.TaskScheduler.QueueUserWorkItem(
				new ParameterizedThreadStart(this.BeginJoinThread), state);

		protected virtual void BeginJoinThread(object state)
		{
			NetworkSessionStaticProvider.BeginJoinSessionState beginJoinSessionState = 
				(NetworkSessionStaticProvider.BeginJoinSessionState)state;
			
			beginJoinSessionState.Session.StartClient(beginJoinSessionState);
			
			while ((beginJoinSessionState.Session.HostConnectionResult != 
					NetworkSession.ResultCode.Succeeded || 
					beginJoinSessionState.Session.LocalGamers.Count <= 0) && 
				beginJoinSessionState.Session.HostConnectionResult <= 
				NetworkSession.ResultCode.Succeeded)
			{
				Thread.Sleep(100);
				beginJoinSessionState.Session.Update();
			}
			
			beginJoinSessionState.HostConnectionResult = 
				beginJoinSessionState.Session.HostConnectionResult;
			
			beginJoinSessionState.HostConnectionResultString = 
				beginJoinSessionState.Session.HostConnectionResultString;
			
			beginJoinSessionState.Event.Set();
			
			if (beginJoinSessionState.Callback != null)
			{
				beginJoinSessionState.Callback(beginJoinSessionState);
			}
		}

		public override IAsyncResult BeginJoinInvited(ulong lobbyId, int version, 
			string gameName, IEnumerable<SignedInGamer> localGamers, 
			AsyncCallback callback, object asyncState, 
			GetPasswordForInvitedGameCallback getPasswordCallback)
		{
			throw new NotImplementedException("Begin join not yet implemented");
		}

		protected override void FinishBeginFind(
			NetworkSessionStaticProvider.SessionQueryState state)
		{
			base.TaskScheduler.QueueUserWorkItem(
				new ParameterizedThreadStart(this.FindAvailableSessionsThread), state);
		}

		protected virtual void FindAvailableSessionsThread(object state)
		{
			NetworkSessionStaticProvider.SessionQueryState sessionQueryState = 
				(NetworkSessionStaticProvider.SessionQueryState)state;
			
			List<AvailableNetworkSession> list = new List<AvailableNetworkSession>();
			
			try
			{
				ClientSessionInfo[] array = this.NetworkSessionServices.QueryClientInfo(
					sessionQueryState.SearchProperties);
			
				foreach (ClientSessionInfo clientSessionInfo in array)
				{
					list.Add(new AvailableNetworkSession(clientSessionInfo));
				}
			}
			catch
			{

			}

			sessionQueryState.Sessions = new AvailableNetworkSessionCollection(list);
			sessionQueryState.Event.Set();
		
			if (sessionQueryState.Callback != null)
			{
				sessionQueryState.Callback(sessionQueryState);
			}
		}

		public override HostDiscovery GetHostDiscoveryObject(string gamename, int version, 
															 PlayerID playerID)
		{
			return new LidgrenHostDiscovery(gamename, version, playerID);
		}
	}
}
