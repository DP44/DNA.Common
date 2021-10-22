using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DNA.Net.GamerServices
{
	public sealed class AvailableNetworkSessionCollection 
		: ReadOnlyCollection<AvailableNetworkSession>, IDisposable
	{
		public AvailableNetworkSessionCollection(IList<AvailableNetworkSession> list) 
			: base(list) {}

		public bool IsDisposed =>
			throw new NotImplementedException();

		public void Dispose() =>
			throw new NotImplementedException();
	}
}
