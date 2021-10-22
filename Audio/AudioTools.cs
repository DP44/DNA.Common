using System;
using DNA.Net.GamerServices;
using Microsoft.Xna.Framework.Audio;

namespace DNA.Audio
{
	public static class AudioTools
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static void ShortTimeFourierTransform(float[] fftBuffer, int fftFrameSize, int sign)
		{
			int num = fftFrameSize << 1;
			
			for (int i = 2; i < 2 * fftFrameSize - 2; i += 2)
			{
				int j = 2;
				int k = 0;
			
				while (j < num)
				{
					if ((i & j) != 0)
					{
						k++;
					}
					k <<= 1;
					j <<= 1;
				}
			
				if (i < k)
				{
					float num2 = fftBuffer[i];
					fftBuffer[i] = fftBuffer[k];
					fftBuffer[k] = num2;
					num2 = fftBuffer[i + 1];
					fftBuffer[i + 1] = fftBuffer[k + 1];
					fftBuffer[k + 1] = num2;
				}
			}
			
			int num3 = (int)(Math.Log((double)fftFrameSize) / Math.Log(2.0) + 0.5);
			int l = 0;
			int num4 = 2;
			
			while (l < num3)
			{
				num4 <<= 1;
				int num5 = num4 >> 1;
				float num6 = 1f;
				float num7 = 0f;
				float num8 = (float)(3.1415926535897931 / (double)(num5 >> 1));
				float num9 = (float)Math.Cos((double)num8);
				float num10 = (float)sign * (float)Math.Sin((double)num8);
				
				for (int k = 0; k < num5; k += 2)
				{
					float num12;
				
					for (int i = k; i < num; i += num4)
					{
						int num11 = i + num5;
						num12 = fftBuffer[num11] * num6 - fftBuffer[num11 + 1] * num7;
						float num13 = fftBuffer[num11] * num7 + fftBuffer[num11 + 1] * num6;
						fftBuffer[num11] = fftBuffer[i] - num12;
						fftBuffer[num11 + 1] = fftBuffer[i + 1] - num13;
						fftBuffer[i] += num12;
						fftBuffer[i + 1] += num13;
					}
					
					num12 = num6 * num9 - num7 * num10;
					num7 = num6 * num10 + num7 * num9;
					num6 = num12;
				}

				l++;
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static Microphone GetMic(SignedInGamer gamer)
		{
			foreach (Microphone microphone in Microphone.All)
			{
				if (gamer.IsHeadset(microphone))
				{
					try
					{
						MicrophoneState state = microphone.State;
					}
					catch (NoMicrophoneConnectedException)
					{
						return null;
					}
					
					return microphone;
				}
			}

			return null;
		}
	}
}
