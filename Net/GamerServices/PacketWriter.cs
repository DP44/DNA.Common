using System;
using System.IO;
using Microsoft.Xna.Framework;

namespace DNA.Net.GamerServices
{
	public class PacketWriter : BinaryWriter
	{
		public PacketWriter() {}

		public PacketWriter(int capacity) => 
			throw new NotImplementedException();

		public int Length => 
			throw new NotImplementedException();

		public int Position
		{
			get => 
				throw new NotImplementedException();

			set => 
				throw new NotImplementedException();
		}

		public void Write(Color value) => 
			throw new NotImplementedException();

		public override void Write(double value) => 
			throw new NotImplementedException();

		public override void Write(float value) => 
			throw new NotImplementedException();

		public void Write(Matrix value) => 
			throw new NotImplementedException();

		public void Write(Quaternion value) => 
			throw new NotImplementedException();

		public void Write(Vector2 value) => 
			throw new NotImplementedException();

		public void Write(Vector3 value) => 
			throw new NotImplementedException();

		public void Write(Vector4 value) => 
			throw new NotImplementedException();
	}
}
