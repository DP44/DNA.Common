using System;
using Microsoft.Xna.Framework.Storage;

namespace DNA.IO.Storage
{
	public class SharedSaveDevice : MUSaveDevice
	{
		public SharedSaveDevice(string containerName, byte[] key) : base(containerName, key) {}

		protected override void GetStorageDevice(AsyncCallback callback, SuccessCallback resultCallback)
		{
			StorageDevice.BeginShowSelector(callback, resultCallback);
		}
	}
}
