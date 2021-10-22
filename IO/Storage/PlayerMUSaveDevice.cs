using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;

namespace DNA.IO.Storage
{
	public class PlayerMUSaveDevice : MUSaveDevice
	{
		private const string playerException = "Player {0} must be signed in to get a player specific storage device.";

		public PlayerIndex Player { get; private set; }

		public PlayerMUSaveDevice(PlayerIndex player, string containerName, byte[] key) : base(containerName, key)
		{
			this.Player = player;
		}

		protected override void GetStorageDevice(AsyncCallback callback, SuccessCallback resultCallback)
		{
			StorageDevice.BeginShowSelector(this.Player, callback, resultCallback);
		}

		protected override void PrepareEventArgs(SaveDeviceEventArgs args)
		{
			base.PrepareEventArgs(args);
			args.PlayerToPrompt = new PlayerIndex?(this.Player);
		}
	}
}
