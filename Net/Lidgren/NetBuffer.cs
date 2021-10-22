using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;

namespace DNA.Net.Lidgren
{
	public class NetBuffer
	{
		private const string c_readOverflowError = "Trying to read past the buffer size - likely caused by mismatching Write/Reads, different size or order.";

		protected const int c_overAllocateAmount = 4;

		private static readonly Dictionary<Type, MethodInfo> s_readMethods;

		private static readonly Dictionary<Type, MethodInfo> s_writeMethods;

		internal byte[] m_data;

		internal int m_bitLength;

		internal int m_readPosition;

		public void EnsureBufferSize(int numberOfBits)
		{
			int num = numberOfBits + 7 >> 3;
			if (this.m_data == null)
			{
				this.m_data = new byte[num + 4];
				return;
			}
			if (this.m_data.Length < num)
			{
				Array.Resize<byte>(ref this.m_data, num + 4);
			}
		}

		internal void InternalEnsureBufferSize(int numberOfBits)
		{
			int num = numberOfBits + 7 >> 3;
			if (this.m_data == null)
			{
				this.m_data = new byte[num];
				return;
			}
			if (this.m_data.Length < num)
			{
				Array.Resize<byte>(ref this.m_data, num);
			}
		}

		public void Write(bool value)
		{
			this.EnsureBufferSize(this.m_bitLength + 1);
			NetBitWriter.WriteByte(value ? 1 : 0, 1, this.m_data, this.m_bitLength);
			this.m_bitLength++;
		}

		public void Write(byte source)
		{
			this.EnsureBufferSize(this.m_bitLength + 8);
			NetBitWriter.WriteByte(source, 8, this.m_data, this.m_bitLength);
			this.m_bitLength += 8;
		}

		[CLSCompliant(false)]
		public void Write(sbyte source)
		{
			this.EnsureBufferSize(this.m_bitLength + 8);
			NetBitWriter.WriteByte((byte)source, 8, this.m_data, this.m_bitLength);
			this.m_bitLength += 8;
		}

		public void Write(byte source, int numberOfBits)
		{
			this.EnsureBufferSize(this.m_bitLength + numberOfBits);
			NetBitWriter.WriteByte(source, numberOfBits, this.m_data, this.m_bitLength);
			this.m_bitLength += numberOfBits;
		}

		public void Write(byte[] source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			int num = source.Length * 8;
			this.EnsureBufferSize(this.m_bitLength + num);
			NetBitWriter.WriteBytes(source, 0, source.Length, this.m_data, this.m_bitLength);
			this.m_bitLength += num;
		}

		public void Write(byte[] source, int offsetInBytes, int numberOfBytes)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			int num = numberOfBytes * 8;
			this.EnsureBufferSize(this.m_bitLength + num);
			NetBitWriter.WriteBytes(source, offsetInBytes, numberOfBytes, this.m_data, this.m_bitLength);
			this.m_bitLength += num;
		}

		public void CopyBytesFrom(NetBuffer src, int numberOfBytes)
		{
			if (src == null)
			{
				throw new ArgumentNullException("source");
			}
			int num = numberOfBytes * 8;
			this.EnsureBufferSize(this.m_bitLength + num);
			if (src.m_bitLength - src.m_readPosition + 7 < numberOfBytes * 8)
			{
				return;
			}
			if ((this.m_bitLength & 7) == 0)
			{
				NetBitWriter.ReadBytes(src.m_data, numberOfBytes, src.m_readPosition, this.m_data, this.m_bitLength >> 3);
				src.m_readPosition += numberOfBytes * 8;
				this.m_bitLength += numberOfBytes * 8;
				return;
			}
			byte[] array = src.ReadBytes(numberOfBytes);
			if (array != null)
			{
				NetBitWriter.WriteBytes(array, 0, numberOfBytes, this.m_data, this.m_bitLength);
				this.m_bitLength += numberOfBytes * 8;
			}
		}

		[CLSCompliant(false)]
		public void Write(ushort source)
		{
			this.EnsureBufferSize(this.m_bitLength + 16);
			NetBitWriter.WriteUInt16(source, 16, this.m_data, this.m_bitLength);
			this.m_bitLength += 16;
		}

		[CLSCompliant(false)]
		public void Write(ushort source, int numberOfBits)
		{
			this.EnsureBufferSize(this.m_bitLength + numberOfBits);
			NetBitWriter.WriteUInt16(source, numberOfBits, this.m_data, this.m_bitLength);
			this.m_bitLength += numberOfBits;
		}

		public void Write(short source)
		{
			this.EnsureBufferSize(this.m_bitLength + 16);
			NetBitWriter.WriteUInt16((ushort)source, 16, this.m_data, this.m_bitLength);
			this.m_bitLength += 16;
		}

		public void Write(int source)
		{
			this.EnsureBufferSize(this.m_bitLength + 32);
			NetBitWriter.WriteUInt32((uint)source, 32, this.m_data, this.m_bitLength);
			this.m_bitLength += 32;
		}

		[CLSCompliant(false)]
		public void Write(uint source)
		{
			this.EnsureBufferSize(this.m_bitLength + 32);
			NetBitWriter.WriteUInt32(source, 32, this.m_data, this.m_bitLength);
			this.m_bitLength += 32;
		}

		[CLSCompliant(false)]
		public void Write(uint source, int numberOfBits)
		{
			this.EnsureBufferSize(this.m_bitLength + numberOfBits);
			NetBitWriter.WriteUInt32(source, numberOfBits, this.m_data, this.m_bitLength);
			this.m_bitLength += numberOfBits;
		}

		public void Write(int source, int numberOfBits)
		{
			this.EnsureBufferSize(this.m_bitLength + numberOfBits);
			if (numberOfBits != 32)
			{
				int num = 1 << numberOfBits - 1;
				if (source < 0)
				{
					source = (-source - 1 | num);
				}
				else
				{
					source &= ~num;
				}
			}
			NetBitWriter.WriteUInt32((uint)source, numberOfBits, this.m_data, this.m_bitLength);
			this.m_bitLength += numberOfBits;
		}

		[CLSCompliant(false)]
		public void Write(ulong source)
		{
			this.EnsureBufferSize(this.m_bitLength + 64);
			NetBitWriter.WriteUInt64(source, 64, this.m_data, this.m_bitLength);
			this.m_bitLength += 64;
		}

		[CLSCompliant(false)]
		public void Write(ulong source, int numberOfBits)
		{
			this.EnsureBufferSize(this.m_bitLength + numberOfBits);
			NetBitWriter.WriteUInt64(source, numberOfBits, this.m_data, this.m_bitLength);
			this.m_bitLength += numberOfBits;
		}

		public void Write(long source)
		{
			this.EnsureBufferSize(this.m_bitLength + 64);
			NetBitWriter.WriteUInt64((ulong)source, 64, this.m_data, this.m_bitLength);
			this.m_bitLength += 64;
		}

		public void Write(long source, int numberOfBits)
		{
			this.EnsureBufferSize(this.m_bitLength + numberOfBits);
			NetBitWriter.WriteUInt64((ulong)source, numberOfBits, this.m_data, this.m_bitLength);
			this.m_bitLength += numberOfBits;
		}

		public void Write(float source)
		{
			SingleUIntUnion singleUIntUnion;
			singleUIntUnion.UIntValue = 0U;
			singleUIntUnion.SingleValue = source;
			this.Write(singleUIntUnion.UIntValue);
		}

		public void Write(double source)
		{
			byte[] bytes = BitConverter.GetBytes(source);
			this.Write(bytes);
		}

		[CLSCompliant(false)]
		public int WriteVariableUInt32(uint value)
		{
			int num = 1;
			uint num2 = value;
			while (num2 >= 128U)
			{
				this.Write((byte)(num2 | 128U));
				num2 >>= 7;
				num++;
			}
			this.Write((byte)num2);
			return num;
		}

		public int WriteVariableInt32(int value)
		{
			uint value2 = (uint)(value << 1 ^ value >> 31);
			return this.WriteVariableUInt32(value2);
		}

		public int WriteVariableInt64(long value)
		{
			ulong value2 = (ulong)(value << 1 ^ value >> 63);
			return this.WriteVariableUInt64(value2);
		}

		[CLSCompliant(false)]
		public int WriteVariableUInt64(ulong value)
		{
			int num = 1;
			ulong num2 = value;
			while (num2 >= 128UL)
			{
				this.Write((byte)(num2 | 128UL));
				num2 >>= 7;
				num++;
			}
			this.Write((byte)num2);
			return num;
		}

		public void WriteSignedSingle(float value, int numberOfBits)
		{
			float num = (value + 1f) * 0.5f;
			int num2 = (1 << numberOfBits) - 1;
			uint source = (uint)(num * (float)num2);
			this.Write(source, numberOfBits);
		}

		public void WriteUnitSingle(float value, int numberOfBits)
		{
			int num = (1 << numberOfBits) - 1;
			uint source = (uint)(value * (float)num);
			this.Write(source, numberOfBits);
		}

		public void WriteRangedSingle(float value, float min, float max, int numberOfBits)
		{
			float num = max - min;
			float num2 = (value - min) / num;
			int num3 = (1 << numberOfBits) - 1;
			this.Write((uint)((float)num3 * num2), numberOfBits);
		}

		public int WriteRangedInteger(int min, int max, int value)
		{
			uint value2 = (uint)(max - min);
			int num = NetUtility.BitsToHoldUInt(value2);
			uint source = (uint)(value - min);
			this.Write(source, num);
			return num;
		}

		public void Write(string source)
		{
			if (string.IsNullOrEmpty(source))
			{
				this.EnsureBufferSize(this.m_bitLength + 8);
				this.WriteVariableUInt32(0U);
				return;
			}
			byte[] bytes = Encoding.UTF8.GetBytes(source);
			this.EnsureBufferSize(this.m_bitLength + 8 + bytes.Length * 8);
			this.WriteVariableUInt32((uint)bytes.Length);
			this.Write(bytes);
		}

		public void Write(IPEndPoint endPoint)
		{
			byte[] addressBytes = endPoint.Address.GetAddressBytes();
			this.Write((byte)addressBytes.Length);
			this.Write(addressBytes);
			this.Write((ushort)endPoint.Port);
		}

		public void WriteTime(bool highPrecision)
		{
			double now = NetTime.Now;
			if (highPrecision)
			{
				this.Write(now);
				return;
			}
			this.Write((float)now);
		}

		public void WriteTime(double localTime, bool highPrecision)
		{
			if (highPrecision)
			{
				this.Write(localTime);
				return;
			}
			this.Write((float)localTime);
		}

		public void WritePadBits()
		{
			this.m_bitLength = (this.m_bitLength + 7 >> 3) * 8;
			this.EnsureBufferSize(this.m_bitLength);
		}

		public void WritePadBits(int numberOfBits)
		{
			this.m_bitLength += numberOfBits;
			this.EnsureBufferSize(this.m_bitLength);
		}

		public void Write(NetOutgoingMessage message)
		{
			this.EnsureBufferSize(this.m_bitLength + message.LengthBytes * 8);
			this.Write(message.m_data, 0, message.LengthBytes);
			int num = message.m_bitLength % 8;
			if (num != 0)
			{
				int num2 = 8 - num;
				this.m_bitLength -= num2;
			}
		}

		public void Write(NetIncomingMessage message)
		{
			this.EnsureBufferSize(this.m_bitLength + message.LengthBytes * 8);
			this.Write(message.m_data, 0, message.LengthBytes);
			int num = message.m_bitLength % 8;
			if (num != 0)
			{
				int num2 = 8 - num;
				this.m_bitLength -= num2;
			}
		}

		public byte[] PeekDataBuffer()
		{
			return this.m_data;
		}

		public bool PeekBoolean()
		{
			byte b = NetBitWriter.ReadByte(this.m_data, 1, this.m_readPosition);
			return b > 0;
		}

		public byte PeekByte()
		{
			return NetBitWriter.ReadByte(this.m_data, 8, this.m_readPosition);
		}

		[CLSCompliant(false)]
		public sbyte PeekSByte()
		{
			byte b = NetBitWriter.ReadByte(this.m_data, 8, this.m_readPosition);
			return (sbyte)b;
		}

		public byte PeekByte(int numberOfBits)
		{
			return NetBitWriter.ReadByte(this.m_data, numberOfBits, this.m_readPosition);
		}

		public byte[] PeekBytes(int numberOfBytes)
		{
			byte[] array = new byte[numberOfBytes];
			NetBitWriter.ReadBytes(this.m_data, numberOfBytes, this.m_readPosition, array, 0);
			return array;
		}

		public void PeekBytes(byte[] into, int offset, int numberOfBytes)
		{
			NetBitWriter.ReadBytes(this.m_data, numberOfBytes, this.m_readPosition, into, offset);
		}

		public short PeekInt16()
		{
			uint num = (uint)NetBitWriter.ReadUInt16(this.m_data, 16, this.m_readPosition);
			return (short)num;
		}

		[CLSCompliant(false)]
		public ushort PeekUInt16()
		{
			uint num = (uint)NetBitWriter.ReadUInt16(this.m_data, 16, this.m_readPosition);
			return (ushort)num;
		}

		public int PeekInt32()
		{
			return (int)NetBitWriter.ReadUInt32(this.m_data, 32, this.m_readPosition);
		}

		public int PeekInt32(int numberOfBits)
		{
			uint num = NetBitWriter.ReadUInt32(this.m_data, numberOfBits, this.m_readPosition);
			if (numberOfBits == 32)
			{
				return (int)num;
			}
			int num2 = 1 << numberOfBits - 1;
			if (((ulong)num & (ulong)((long)num2)) == 0UL)
			{
				return (int)num;
			}
			uint num3 = uint.MaxValue >> 33 - numberOfBits;
			uint num4 = (num & num3) + 1U;
			return (int)(-(int)num4);
		}

		[CLSCompliant(false)]
		public uint PeekUInt32()
		{
			return NetBitWriter.ReadUInt32(this.m_data, 32, this.m_readPosition);
		}

		[CLSCompliant(false)]
		public uint PeekUInt32(int numberOfBits)
		{
			return NetBitWriter.ReadUInt32(this.m_data, numberOfBits, this.m_readPosition);
		}

		[CLSCompliant(false)]
		public ulong PeekUInt64()
		{
			ulong num = (ulong)NetBitWriter.ReadUInt32(this.m_data, 32, this.m_readPosition);
			ulong num2 = (ulong)NetBitWriter.ReadUInt32(this.m_data, 32, this.m_readPosition + 32);
			return num + (num2 << 32);
		}

		public long PeekInt64()
		{
			return (long)this.PeekUInt64();
		}

		[CLSCompliant(false)]
		public ulong PeekUInt64(int numberOfBits)
		{
			ulong num;
			if (numberOfBits <= 32)
			{
				num = (ulong)NetBitWriter.ReadUInt32(this.m_data, numberOfBits, this.m_readPosition);
			}
			else
			{
				num = (ulong)NetBitWriter.ReadUInt32(this.m_data, 32, this.m_readPosition);
				num |= (ulong)NetBitWriter.ReadUInt32(this.m_data, numberOfBits - 32, this.m_readPosition);
			}
			return num;
		}

		public long PeekInt64(int numberOfBits)
		{
			return (long)this.PeekUInt64(numberOfBits);
		}

		public float PeekFloat()
		{
			return this.PeekSingle();
		}

		public float PeekSingle()
		{
			if ((this.m_readPosition & 7) == 0)
			{
				return BitConverter.ToSingle(this.m_data, this.m_readPosition >> 3);
			}
			byte[] value = this.PeekBytes(4);
			return BitConverter.ToSingle(value, 0);
		}

		public double PeekDouble()
		{
			if ((this.m_readPosition & 7) == 0)
			{
				return BitConverter.ToDouble(this.m_data, this.m_readPosition >> 3);
			}
			byte[] value = this.PeekBytes(8);
			return BitConverter.ToDouble(value, 0);
		}

		public string PeekString()
		{
			int readPosition = this.m_readPosition;
			string result = this.ReadString();
			this.m_readPosition = readPosition;
			return result;
		}

		public void ReadAllFields(object target)
		{
			this.ReadAllFields(target, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		}

		public void ReadAllFields(object target, BindingFlags flags)
		{
			if (target == null)
			{
				return;
			}
			Type type = target.GetType();
			FieldInfo[] fields = type.GetFields(flags);
			NetUtility.SortMembersList(fields);
			foreach (FieldInfo fieldInfo in fields)
			{
				MethodInfo methodInfo;
				if (NetBuffer.s_readMethods.TryGetValue(fieldInfo.FieldType, out methodInfo))
				{
					object value = methodInfo.Invoke(this, null);
					fieldInfo.SetValue(target, value);
				}
			}
		}

		public void ReadAllProperties(object target)
		{
			this.ReadAllProperties(target, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		}

		public void ReadAllProperties(object target, BindingFlags flags)
		{
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}
			if (target == null)
			{
				return;
			}
			Type type = target.GetType();
			PropertyInfo[] properties = type.GetProperties(flags);
			NetUtility.SortMembersList(properties);
			foreach (PropertyInfo propertyInfo in properties)
			{
				MethodInfo methodInfo;
				if (NetBuffer.s_readMethods.TryGetValue(propertyInfo.PropertyType, out methodInfo))
				{
					object obj = methodInfo.Invoke(this, null);
					MethodInfo setMethod = propertyInfo.GetSetMethod((flags & BindingFlags.NonPublic) == BindingFlags.NonPublic);
					setMethod.Invoke(target, new object[]
					{
						obj
					});
				}
			}
		}

		public bool ReadBoolean()
		{
			byte b = NetBitWriter.ReadByte(this.m_data, 1, this.m_readPosition);
			this.m_readPosition++;
			return b > 0;
		}

		public byte ReadByte()
		{
			byte result = NetBitWriter.ReadByte(this.m_data, 8, this.m_readPosition);
			this.m_readPosition += 8;
			return result;
		}

		public bool ReadByte(out byte result)
		{
			if (this.m_bitLength - this.m_readPosition < 8)
			{
				result = 0;
				return false;
			}
			result = NetBitWriter.ReadByte(this.m_data, 8, this.m_readPosition);
			this.m_readPosition += 8;
			return true;
		}

		[CLSCompliant(false)]
		public sbyte ReadSByte()
		{
			byte b = NetBitWriter.ReadByte(this.m_data, 8, this.m_readPosition);
			this.m_readPosition += 8;
			return (sbyte)b;
		}

		public byte ReadByte(int numberOfBits)
		{
			byte result = NetBitWriter.ReadByte(this.m_data, numberOfBits, this.m_readPosition);
			this.m_readPosition += numberOfBits;
			return result;
		}

		public byte[] ReadBytes(int numberOfBytes)
		{
			byte[] array = new byte[numberOfBytes];
			NetBitWriter.ReadBytes(this.m_data, numberOfBytes, this.m_readPosition, array, 0);
			this.m_readPosition += 8 * numberOfBytes;
			return array;
		}

		public bool ReadBytes(int numberOfBytes, out byte[] result)
		{
			if (this.m_bitLength - this.m_readPosition + 7 < numberOfBytes * 8)
			{
				result = null;
				return false;
			}
			result = new byte[numberOfBytes];
			NetBitWriter.ReadBytes(this.m_data, numberOfBytes, this.m_readPosition, result, 0);
			this.m_readPosition += 8 * numberOfBytes;
			return true;
		}

		public bool GetAlignedData(out byte[] data, out int offset)
		{
			if ((this.m_readPosition & 7) == 0)
			{
				data = this.m_data;
				offset = this.m_readPosition >> 3;
				return true;
			}
			data = null;
			offset = 0;
			return false;
		}

		public void ReadBytes(byte[] into, int offset, int numberOfBytes)
		{
			NetBitWriter.ReadBytes(this.m_data, numberOfBytes, this.m_readPosition, into, offset);
			this.m_readPosition += 8 * numberOfBytes;
		}

		public void ReadBits(byte[] into, int offset, int numberOfBits)
		{
			int num = numberOfBits / 8;
			int num2 = numberOfBits - num * 8;
			NetBitWriter.ReadBytes(this.m_data, num, this.m_readPosition, into, offset);
			this.m_readPosition += 8 * num;
			if (num2 > 0)
			{
				into[offset + num] = this.ReadByte(num2);
			}
		}

		public short ReadInt16()
		{
			uint num = (uint)NetBitWriter.ReadUInt16(this.m_data, 16, this.m_readPosition);
			this.m_readPosition += 16;
			return (short)num;
		}

		[CLSCompliant(false)]
		public ushort ReadUInt16()
		{
			uint num = (uint)NetBitWriter.ReadUInt16(this.m_data, 16, this.m_readPosition);
			this.m_readPosition += 16;
			return (ushort)num;
		}

		public int ReadInt32()
		{
			uint result = NetBitWriter.ReadUInt32(this.m_data, 32, this.m_readPosition);
			this.m_readPosition += 32;
			return (int)result;
		}

		[CLSCompliant(false)]
		public bool ReadInt32(out int result)
		{
			if (this.m_bitLength - this.m_readPosition < 32)
			{
				result = 0;
				return false;
			}
			result = (int)NetBitWriter.ReadUInt32(this.m_data, 32, this.m_readPosition);
			this.m_readPosition += 32;
			return true;
		}

		public int ReadInt32(int numberOfBits)
		{
			uint num = NetBitWriter.ReadUInt32(this.m_data, numberOfBits, this.m_readPosition);
			this.m_readPosition += numberOfBits;
			if (numberOfBits == 32)
			{
				return (int)num;
			}
			int num2 = 1 << numberOfBits - 1;
			if (((ulong)num & (ulong)((long)num2)) == 0UL)
			{
				return (int)num;
			}
			uint num3 = uint.MaxValue >> 33 - numberOfBits;
			uint num4 = (num & num3) + 1U;
			return (int)(-(int)num4);
		}

		[CLSCompliant(false)]
		public uint ReadUInt32()
		{
			uint result = NetBitWriter.ReadUInt32(this.m_data, 32, this.m_readPosition);
			this.m_readPosition += 32;
			return result;
		}

		[CLSCompliant(false)]
		public bool ReadUInt32(out uint result)
		{
			if (this.m_bitLength - this.m_readPosition < 32)
			{
				result = 0U;
				return false;
			}
			result = NetBitWriter.ReadUInt32(this.m_data, 32, this.m_readPosition);
			this.m_readPosition += 32;
			return true;
		}

		[CLSCompliant(false)]
		public uint ReadUInt32(int numberOfBits)
		{
			uint result = NetBitWriter.ReadUInt32(this.m_data, numberOfBits, this.m_readPosition);
			this.m_readPosition += numberOfBits;
			return result;
		}

		[CLSCompliant(false)]
		public ulong ReadUInt64()
		{
			ulong num = (ulong)NetBitWriter.ReadUInt32(this.m_data, 32, this.m_readPosition);
			this.m_readPosition += 32;
			ulong num2 = (ulong)NetBitWriter.ReadUInt32(this.m_data, 32, this.m_readPosition);
			ulong result = num + (num2 << 32);
			this.m_readPosition += 32;
			return result;
		}

		public long ReadInt64()
		{
			return (long)this.ReadUInt64();
		}

		[CLSCompliant(false)]
		public ulong ReadUInt64(int numberOfBits)
		{
			ulong num;
			if (numberOfBits <= 32)
			{
				num = (ulong)NetBitWriter.ReadUInt32(this.m_data, numberOfBits, this.m_readPosition);
			}
			else
			{
				num = (ulong)NetBitWriter.ReadUInt32(this.m_data, 32, this.m_readPosition);
				num |= (ulong)NetBitWriter.ReadUInt32(this.m_data, numberOfBits - 32, this.m_readPosition);
			}
			this.m_readPosition += numberOfBits;
			return num;
		}

		public long ReadInt64(int numberOfBits)
		{
			return (long)this.ReadUInt64(numberOfBits);
		}

		public float ReadFloat()
		{
			return this.ReadSingle();
		}

		public float ReadSingle()
		{
			if ((this.m_readPosition & 7) == 0)
			{
				float result = BitConverter.ToSingle(this.m_data, this.m_readPosition >> 3);
				this.m_readPosition += 32;
				return result;
			}
			byte[] value = this.ReadBytes(4);
			return BitConverter.ToSingle(value, 0);
		}

		public bool ReadSingle(out float result)
		{
			if (this.m_bitLength - this.m_readPosition < 32)
			{
				result = 0f;
				return false;
			}
			if ((this.m_readPosition & 7) == 0)
			{
				result = BitConverter.ToSingle(this.m_data, this.m_readPosition >> 3);
				this.m_readPosition += 32;
				return true;
			}
			byte[] value = this.ReadBytes(4);
			result = BitConverter.ToSingle(value, 0);
			return true;
		}

		public double ReadDouble()
		{
			if ((this.m_readPosition & 7) == 0)
			{
				double result = BitConverter.ToDouble(this.m_data, this.m_readPosition >> 3);
				this.m_readPosition += 64;
				return result;
			}
			byte[] value = this.ReadBytes(8);
			return BitConverter.ToDouble(value, 0);
		}

		[CLSCompliant(false)]
		public uint ReadVariableUInt32()
		{
			int num = 0;
			int num2 = 0;
			byte b;
			do
			{
				b = this.ReadByte();
				num |= (int)(b & 127) << num2;
				num2 += 7;
			}
			while ((b & 128) != 0);
			return (uint)num;
		}

		[CLSCompliant(false)]
		public bool ReadVariableUInt32(out uint result)
		{
			int num = 0;
			int num2 = 0;
			byte b;
			while (this.ReadByte(out b))
			{
				num |= (int)(b & 127) << num2;
				num2 += 7;
				if ((b & 128) == 0)
				{
					result = (uint)num;
					return true;
				}
			}
			result = 0U;
			return false;
		}

		public int ReadVariableInt32()
		{
			uint num = this.ReadVariableUInt32();
			return (int)(num >> 1 ^ -(int)(num & 1U));
		}

		public long ReadVariableInt64()
		{
			ulong num = this.ReadVariableUInt64();
			return (long)(num >> 1 ^ -(long)(num & 1UL));
		}

		[CLSCompliant(false)]
		public ulong ReadVariableUInt64()
		{
			ulong num = 0UL;
			int num2 = 0;
			byte b;
			do
			{
				b = this.ReadByte();
				num |= ((ulong)b & 127UL) << num2;
				num2 += 7;
			}
			while ((b & 128) != 0);
			return num;
		}

		public float ReadSignedSingle(int numberOfBits)
		{
			uint num = this.ReadUInt32(numberOfBits);
			int num2 = (1 << numberOfBits) - 1;
			return ((num + 1U) / (float)(num2 + 1) - 0.5f) * 2f;
		}

		public float ReadUnitSingle(int numberOfBits)
		{
			uint num = this.ReadUInt32(numberOfBits);
			int num2 = (1 << numberOfBits) - 1;
			return (num + 1U) / (float)(num2 + 1);
		}

		public float ReadRangedSingle(float min, float max, int numberOfBits)
		{
			float num = max - min;
			int num2 = (1 << numberOfBits) - 1;
			float num3 = this.ReadUInt32(numberOfBits);
			float num4 = num3 / (float)num2;
			return min + num4 * num;
		}

		public int ReadRangedInteger(int min, int max)
		{
			uint value = (uint)(max - min);
			int numberOfBits = NetUtility.BitsToHoldUInt(value);
			uint num = this.ReadUInt32(numberOfBits);
			return (int)((long)min + (long)((ulong)num));
		}

		public string ReadString()
		{
			int num = (int)this.ReadVariableUInt32();
			if (num == 0)
			{
				return string.Empty;
			}
			if ((this.m_readPosition & 7) == 0)
			{
				string @string = Encoding.UTF8.GetString(this.m_data, this.m_readPosition >> 3, num);
				this.m_readPosition += 8 * num;
				return @string;
			}
			byte[] array = this.ReadBytes(num);
			return Encoding.UTF8.GetString(array, 0, array.Length);
		}

		public bool ReadString(out string result)
		{
			uint num;
			if (!this.ReadVariableUInt32(out num))
			{
				result = string.Empty;
				return false;
			}
			if (num == 0U)
			{
				result = string.Empty;
				return true;
			}
			if ((long)(this.m_bitLength - this.m_readPosition) < (long)((ulong)(num * 8U)))
			{
				result = string.Empty;
				return false;
			}
			if ((this.m_readPosition & 7) == 0)
			{
				result = Encoding.UTF8.GetString(this.m_data, this.m_readPosition >> 3, (int)num);
				this.m_readPosition += (int)(8U * num);
				return true;
			}
			byte[] array;
			if (!this.ReadBytes((int)num, out array))
			{
				result = string.Empty;
				return false;
			}
			result = Encoding.UTF8.GetString(array, 0, array.Length);
			return true;
		}

		public double ReadTime(NetConnection connection, bool highPrecision)
		{
			double num = highPrecision ? this.ReadDouble() : ((double)this.ReadSingle());
			if (connection == null)
			{
				throw new NetException("Cannot call ReadTime() on message without a connected sender (ie. unconnected messages)");
			}
			return num - connection.m_remoteTimeOffset;
		}

		public IPEndPoint ReadIPEndPoint()
		{
			byte numberOfBytes = this.ReadByte();
			byte[] address = this.ReadBytes((int)numberOfBytes);
			int port = (int)this.ReadUInt16();
			IPAddress address2 = new IPAddress(address);
			return new IPEndPoint(address2, port);
		}

		public void SkipPadBits()
		{
			this.m_readPosition = (this.m_readPosition + 7 >> 3) * 8;
		}

		public void ReadPadBits()
		{
			this.m_readPosition = (this.m_readPosition + 7 >> 3) * 8;
		}

		public void SkipPadBits(int numberOfBits)
		{
			this.m_readPosition += numberOfBits;
		}

		public void WriteAllFields(object ob)
		{
			this.WriteAllFields(ob, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		}

		public void WriteAllFields(object ob, BindingFlags flags)
		{
			if (ob == null)
			{
				return;
			}
			Type type = ob.GetType();
			FieldInfo[] fields = type.GetFields(flags);
			NetUtility.SortMembersList(fields);
			foreach (FieldInfo fieldInfo in fields)
			{
				object value = fieldInfo.GetValue(ob);
				MethodInfo methodInfo;
				if (!NetBuffer.s_writeMethods.TryGetValue(fieldInfo.FieldType, out methodInfo))
				{
					throw new NetException("Failed to find write method for type " + fieldInfo.FieldType);
				}
				methodInfo.Invoke(this, new object[]
				{
					value
				});
			}
		}

		public void WriteAllProperties(object ob)
		{
			this.WriteAllProperties(ob, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		}

		public void WriteAllProperties(object ob, BindingFlags flags)
		{
			if (ob == null)
			{
				return;
			}
			Type type = ob.GetType();
			PropertyInfo[] properties = type.GetProperties(flags);
			NetUtility.SortMembersList(properties);
			foreach (PropertyInfo propertyInfo in properties)
			{
				MethodInfo getMethod = propertyInfo.GetGetMethod((flags & BindingFlags.NonPublic) == BindingFlags.NonPublic);
				object obj = getMethod.Invoke(ob, null);
				MethodInfo methodInfo;
				if (NetBuffer.s_writeMethods.TryGetValue(propertyInfo.PropertyType, out methodInfo))
				{
					methodInfo.Invoke(this, new object[]
					{
						obj
					});
				}
			}
		}

		public byte[] Data
		{
			get
			{
				return this.m_data;
			}
			set
			{
				this.m_data = value;
			}
		}

		public int LengthBytes
		{
			get
			{
				return this.m_bitLength + 7 >> 3;
			}
			set
			{
				this.m_bitLength = value * 8;
				this.InternalEnsureBufferSize(this.m_bitLength);
			}
		}

		public int LengthBits
		{
			get
			{
				return this.m_bitLength;
			}
			set
			{
				this.m_bitLength = value;
				this.InternalEnsureBufferSize(this.m_bitLength);
			}
		}

		public long Position
		{
			get
			{
				return (long)this.m_readPosition;
			}
			set
			{
				this.m_readPosition = (int)value;
			}
		}

		public int PositionInBytes
		{
			get
			{
				return this.m_readPosition / 8;
			}
		}

		static NetBuffer()
		{
			typeof(byte).Assembly.GetTypes();
			NetBuffer.s_readMethods = new Dictionary<Type, MethodInfo>();
			MethodInfo[] methods = typeof(NetIncomingMessage).GetMethods(BindingFlags.Instance | BindingFlags.Public);
			foreach (MethodInfo methodInfo in methods)
			{
				if (methodInfo.GetParameters().Length == 0 && methodInfo.Name.StartsWith("Read", StringComparison.InvariantCulture) && methodInfo.Name.Substring(4) == methodInfo.ReturnType.Name)
				{
					NetBuffer.s_readMethods[methodInfo.ReturnType] = methodInfo;
				}
			}
			NetBuffer.s_writeMethods = new Dictionary<Type, MethodInfo>();
			methods = typeof(NetOutgoingMessage).GetMethods(BindingFlags.Instance | BindingFlags.Public);
			foreach (MethodInfo methodInfo2 in methods)
			{
				if (methodInfo2.Name.Equals("Write", StringComparison.InvariantCulture))
				{
					ParameterInfo[] parameters = methodInfo2.GetParameters();
					if (parameters.Length == 1)
					{
						NetBuffer.s_writeMethods[parameters[0].ParameterType] = methodInfo2;
					}
				}
			}
		}
	}
}
