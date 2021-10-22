using System;

namespace DNA.Net.GamerServices
{
	public class AvatarDescription
	{
		private byte[] _data;

		private AvatarBodyType _bodyType = AvatarBodyType.Male;

		private float _height = 1.6f;

		public AvatarDescription(byte[] data) =>
			this._data = data;

		public AvatarBodyType BodyType =>
			this._bodyType;

		public byte[] Description =>
			this._data;

		public float Height =>
			this._height;

		public bool IsValid =>
			this._data != null;

		public event EventHandler<EventArgs> Changed;

		public static IAsyncResult BeginGetFromGamer(Gamer gamer, AsyncCallback callback, 
													 object state) =>
			throw new NotImplementedException();

		public static AvatarDescription CreateRandom() =>
			throw new NotImplementedException();

		public static AvatarDescription CreateRandom(AvatarBodyType bodyType) =>
			throw new NotImplementedException();

		public static AvatarDescription EndGetFromGamer(IAsyncResult result) =>
			throw new NotImplementedException();
	}
}
