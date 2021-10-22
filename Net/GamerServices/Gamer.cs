using System;
using System.Threading;

namespace DNA.Net.GamerServices
{
	public abstract class Gamer
	{
		private class BaseAsyncResult : IAsyncResult
		{
			private object _state;

			public ManualResetEvent Event = new ManualResetEvent(false);
			public AsyncCallback Callback;

			public BaseAsyncResult(AsyncCallback callback, object state)
			{
				this.Callback = callback;
				this._state = state;
			}

			public object AsyncState => 
				this._state;

			public WaitHandle AsyncWaitHandle => 
				(WaitHandle)this.Event;

			public bool CompletedSynchronously => 
				throw new NotImplementedException();

			public bool IsCompleted => true;
		}

		private string _gamerTag = "User";
		private object _tag;
		private bool _isDisposed;

		public PlayerID PlayerID = PlayerID.Null;

		public string Gamertag
		{
			get => 
				this._gamerTag;
		
			set => 
				this._gamerTag = value;
		}

		public bool IsDisposed => this._isDisposed;

		public static SignedInGamerCollection SignedInGamers => 
			GamerServicesComponent.Instance.SignedInGamers;

		public object Tag
		{
			get => 
				this._tag;
			
			set => 
				this._tag = value;
		}

		public static IAsyncResult BeginGetFromGamertag(string gamertag, 
														AsyncCallback callback, 
														object asyncState) => 
			throw new NotImplementedException();

		public IAsyncResult BeginGetProfile(AsyncCallback callback, object asyncState)
		{
			IAsyncResult result = new Gamer.BaseAsyncResult(callback, asyncState);
			callback(result);
			return result;
		}

		public static Gamer EndGetFromGamertag(IAsyncResult result) => 
			throw new NotImplementedException();

		public GamerProfile EndGetProfile(IAsyncResult result) => 
			(GamerProfile)null;

		public static Gamer GetFromGamertag(string gamertag) => 
			throw new NotImplementedException();

		public GamerProfile GetProfile() =>
			throw new NotImplementedException();

		public override string ToString() => 
			this._gamerTag;
	}
}
