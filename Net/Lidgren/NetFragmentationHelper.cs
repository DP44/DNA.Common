using System;

namespace DNA.Net.Lidgren
{
	internal static class NetFragmentationHelper
	{
		internal static int WriteHeader(byte[] destination, int ptr, 
										int group, int totalBits, 
										int chunkByteSize, int chunkNumber)
		{
			uint num;
			
			for (num = (uint)group; num >= 128U; num >>= 7)
			{
				destination[ptr++] = (byte)(num | 128U);
			}
			
			destination[ptr++] = (byte)num;
			uint num2;
			
			for (num2 = (uint)totalBits; num2 >= 128U; num2 >>= 7)
			{
				destination[ptr++] = (byte)(num2 | 128U);
			}
			
			destination[ptr++] = (byte)num2;
			uint num3;
			
			for (num3 = (uint)chunkByteSize; num3 >= 128U; num3 >>= 7)
			{
				destination[ptr++] = (byte)(num3 | 128U);
			}
			
			destination[ptr++] = (byte)num3;
			uint num4;
			
			for (num4 = (uint)chunkNumber; num4 >= 128U; num4 >>= 7)
			{
				destination[ptr++] = (byte)(num4 | 128U);
			}
			
			destination[ptr++] = (byte)num4;
			return ptr;
		}

		internal static int ReadHeader(byte[] buffer, int ptr, out int group, 
									   out int totalBits, out int chunkByteSize, 
									   out int chunkNumber)
		{
			int num = 0;
			int num2 = 0;
			byte b;
			
			do
			{
				b = buffer[ptr++];
				num |= (int)(b & 127) << num2;
				num2 += 7;
			}
			while ((b & 128) != 0);
			
			group = num;
			num = 0;
			num2 = 0;
			byte b2;
			
			do
			{
				b2 = buffer[ptr++];
				num |= (int)(b2 & 127) << num2;
				num2 += 7;
			}
			while ((b2 & 128) != 0);
			
			totalBits = num;
			num = 0;
			num2 = 0;
			byte b3;
			
			do
			{
				b3 = buffer[ptr++];
				num |= (int)(b3 & 127) << num2;
				num2 += 7;
			}
			while ((b3 & 128) != 0);
			
			chunkByteSize = num;
			num = 0;
			num2 = 0;
			byte b4;
			
			do
			{
				b4 = buffer[ptr++];
				num |= (int)(b4 & 127) << num2;
				num2 += 7;
			}
			while ((b4 & 128) != 0);

			chunkNumber = num;
			return ptr;
		}

		internal static int GetFragmentationHeaderSize(int groupId, int totalBytes, 
													   int chunkByteSize, int numChunks)
		{
			int headerSize = 4;
			
			for (uint num2 = (uint)groupId; num2 >= 128U; num2 >>= 7)
			{
				headerSize++;
			}

			for (uint num3 = (uint)(totalBytes * 8); num3 >= 128U; num3 >>= 7)
			{
				headerSize++;
			}
			
			for (uint num4 = (uint)chunkByteSize; num4 >= 128U; num4 >>= 7)
			{
				headerSize++;
			}
			
			for (uint num5 = (uint)numChunks; num5 >= 128U; num5 >>= 7)
			{
				headerSize++;
			}
			
			return headerSize;
		}

		internal static int GetBestChunkSize(int group, int totalBytes, int mtu)
		{
			int num = mtu - 5 - 4;
			
			int fragmentationHeaderSize = 
				NetFragmentationHelper.GetFragmentationHeaderSize(
					group, totalBytes, num, totalBytes / num);
			
			num = mtu - 5 - fragmentationHeaderSize;
			
			int fragmentationHeaderSize2;
			
			do
			{
				num--;
				int num2 = totalBytes / num;
			
				if (num2 * num < totalBytes)
				{
					num2++;
				}
			
				fragmentationHeaderSize2 = 
					NetFragmentationHelper.GetFragmentationHeaderSize(
						group, totalBytes, num, num2);
			}
			while (num + fragmentationHeaderSize2 + 5 + 1 >= mtu);
			
			return num;
		}
	}
}
