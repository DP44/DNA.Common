using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DNA.Net.GamerServices
{
	public sealed class SignedInGamerCollection 
		: GamerCollection<SignedInGamer>
	{
		internal SignedInGamerCollection(IList<SignedInGamer> list) 
			: base(list) {}

		public SignedInGamer this[PlayerIndex index] =>
			index == PlayerIndex.One ? base[0] : null;
		/*
			{
				get
				{
					if (index == PlayerIndex.One)
					{
						return base[0];
					}

					return null;
				}
			}
		*/
	}
}
