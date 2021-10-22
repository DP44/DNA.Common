using System;
using System.Diagnostics;

namespace DNA.Net.Lidgren
{
	[DebuggerDisplay("LengthBits={LengthBits}")]
	public sealed class NetOutgoingMessage : NetBuffer
	{
		internal NetMessageType m_messageType;
		internal bool m_isSent;
		internal int m_recyclingCount;
		internal int m_fragmentGroup;
		internal int m_fragmentGroupTotalBits;
		internal int m_fragmentChunkByteSize;
		internal int m_fragmentChunkNumber;

		internal NetOutgoingMessage() {}

		internal void Reset()
		{
			this.m_messageType = NetMessageType.LibraryError;
			this.m_bitLength = 0;
			this.m_isSent = false;
			this.m_recyclingCount = 0;
			this.m_fragmentGroup = 0;
		}

		internal int Encode(byte[] intoBuffer, int ptr, int sequenceNumber)
		{
			intoBuffer[ptr++] = (byte)this.m_messageType;
			byte b = (byte)(sequenceNumber << 1 | ((this.m_fragmentGroup == 0) ? 0 : 1));
			intoBuffer[ptr++] = b;
			intoBuffer[ptr++] = (byte)(sequenceNumber >> 7);
			
			if (this.m_fragmentGroup == 0)
			{
				intoBuffer[ptr++] = (byte)this.m_bitLength;
				intoBuffer[ptr++] = (byte)(this.m_bitLength >> 8);
			
				int num = NetUtility.BytesToHoldBits(this.m_bitLength);
			
				if (num > 0)
				{
					Buffer.BlockCopy(this.m_data, 0, intoBuffer, ptr, num);
					ptr += num;
				}
			}
			else
			{
				int num2 = ptr;
				intoBuffer[ptr++] = (byte)this.m_bitLength;
				intoBuffer[ptr++] = (byte)(this.m_bitLength >> 8);
			
				ptr = NetFragmentationHelper.WriteHeader(intoBuffer, ptr, 
					this.m_fragmentGroup, this.m_fragmentGroupTotalBits, 
					this.m_fragmentChunkByteSize, this.m_fragmentChunkNumber);
			
				int num3 = ptr - num2 - 2;
				int num4 = this.m_bitLength + num3 * 8;
				intoBuffer[num2] = (byte)num4;
				intoBuffer[num2 + 1] = (byte)(num4 >> 8);
				int num5 = NetUtility.BytesToHoldBits(this.m_bitLength);
				
				if (num5 > 0)
				{
					Buffer.BlockCopy(this.m_data, 
						this.m_fragmentChunkNumber * this.m_fragmentChunkByteSize, 
						intoBuffer, ptr, num5);
				
					ptr += num5;
				}
			}
			
			return ptr;
		}

		internal int GetEncodedSize()
		{
			int num = 5;
		
			if (this.m_fragmentGroup != 0)
			{
				num += NetFragmentationHelper.GetFragmentationHeaderSize(this.m_fragmentGroup, 
					this.m_fragmentGroupTotalBits / 8, this.m_fragmentChunkByteSize, 
					this.m_fragmentChunkNumber);
			}
		
			return num + base.LengthBytes;
		}

		public bool Encrypt(INetEncryption encryption) =>
			encryption.Encrypt(this);

		public override string ToString()
		{
			return string.Concat(
				"[NetOutgoingMessage ", this.m_messageType, " ", base.LengthBytes, " bytes]");
		}
	}
}
