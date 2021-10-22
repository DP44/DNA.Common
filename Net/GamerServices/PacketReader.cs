using System;
using System.IO;
using Microsoft.Xna.Framework;

namespace DNA.Net.GamerServices
{
	public class PacketReader : BinaryReader
	{
		internal PacketReader(Stream stream) 
			: base(stream) {}

		public int Length => 
			throw new NotImplementedException();

		public int Position
		{
			get => 
				throw new NotImplementedException();

			set => 
				throw new NotImplementedException();
		}

		public Color ReadColor() => 
			throw new NotImplementedException();

		public override double ReadDouble() => 
			throw new NotImplementedException();

		public Matrix ReadMatrix() => 
			throw new NotImplementedException();

		public Quaternion ReadQuaternion() => 
			throw new NotImplementedException();

		public override float ReadSingle() => 
			throw new NotImplementedException();

		public Vector2 ReadVector2() => 
			throw new NotImplementedException();

		public Vector3 ReadVector3() => 
			throw new NotImplementedException();

		public Vector4 ReadVector4() => 
			throw new NotImplementedException();
	}
}
