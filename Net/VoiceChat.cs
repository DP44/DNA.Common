using System;
using DNA.Audio;
using DNA.Net.GamerServices;
using Microsoft.Xna.Framework.Audio;

namespace DNA.Net
{
	public class VoiceChat
	{
		private Microphone _microphone;
		private EventHandler<EventArgs> handler;
		private DynamicSoundEffectInstance _playbackEffect;
		private LocalNetworkGamer _gamer;

		private byte[] _micBuffer;
		private byte[] _sendBuffer;
		private byte[] _playBuffer;
		private static byte[] _pcmToALawMap;
		private static short[] _aLawToPcmMap;

		static VoiceChat()
		{
			VoiceChat.BuildALawMap();

			// Why is this here?
			Microphone default = Microphone.Default;
		}

		public VoiceChat(LocalNetworkGamer gamer)
		{
			this._gamer = gamer;
			this._microphone = AudioTools.GetMic(gamer.SignedInGamer);
			this._microphone.BufferDuration = TimeSpan.FromMilliseconds(100.0);
			this.handler = new EventHandler<EventArgs>(this._microphone_BufferReady);
			this._microphone.BufferReady += this.handler;
			
			this._micBuffer = new byte[this._microphone.GetSampleSizeInBytes(
				this._microphone.BufferDuration)];
			
			this._playBuffer = new byte[this._micBuffer.Length];
			this._sendBuffer = new byte[this._micBuffer.Length / 2];
			
			this._playbackEffect = new DynamicSoundEffectInstance(
				this._microphone.SampleRate, AudioChannels.Mono);
			
			this._playbackEffect.SubmitBuffer(this._micBuffer);
			this._playbackEffect.Play();
		}

		private void _microphone_BufferReady(object sender, EventArgs e)
		{
			this._microphone.GetData(this._micBuffer);
			int num = this._micBuffer.Length / 2;
			int num2 = 0;
			
			for (int i = 0; i < num; i++)
			{
				short num3 = (short)this._micBuffer[num2 + 1];
				short num4 = (short)(this._micBuffer[num2] << 8);
				short num5 = num3 | num4;
				this._sendBuffer[i] = VoiceChat._pcmToALawMap[(int)num5];
				num2 += 2;
			}
			
			VoiceChatMessage.Send(this._gamer, this._sendBuffer);
		}

		public void ProcessMessage(VoiceChatMessage message)
		{
			int num = 0;
			
			for (int i = 0; i < message.AudioBuffer.Length; i++)
			{
				short num2 = VoiceChat._aLawToPcmMap[(int)message.AudioBuffer[i]];
				this._playBuffer[num++] = (byte)(num2 & 255);
				this._playBuffer[num++] = (byte)(num2 >> 8);
			}
			
			this._playbackEffect.SubmitBuffer(this._playBuffer);
		}

		private static void BuildALawMap()
		{
			VoiceChat._pcmToALawMap = new byte[65536];
			
			for (int i = -32768; i <= 32767; i++)
			{
				VoiceChat._pcmToALawMap[i & 65535] = VoiceChat.EncodeALawSample(i);
			}
			
			VoiceChat._aLawToPcmMap = new short[256];
			
			for (byte b = 0; b < 255; b += 1)
			{
				VoiceChat._aLawToPcmMap[(int)b] = VoiceChat.DecodeALawSample(b);
			}
		}

		private static byte EncodeALawSample(int pcm)
		{
			int num = (pcm & 32768) >> 8;
			
			if (num != 0)
			{
				pcm = -pcm;
			}
			
			if (pcm > 32767)
			{
				pcm = 32767;
			}
			
			int num2 = 7;
			int num3 = 16384;
			
			while ((pcm & num3) == 0 && num2 > 0)
			{
				num2--;
				num3 >>= 1;
			}
			
			int num4 = pcm >> (((num2 == 0) ? 4 : (num2 + 3)) & 31) & 15;
			byte b = (byte)(num | num2 << 4 | num4);
			return b ^ 213;
		}

		private static short DecodeALawSample(byte alaw)
		{
			alaw ^= 0xD5;
			int num = (int)(alaw & 0x80);
			int num2 = ((int)alaw & 0x70) >> 4;
			int num3 = (((int)alaw & 0x0F) << 4) + 8;
			
			if (num2 != 0)
			{
				num3 += 256;
			}
			
			if (num2 > 1)
			{
				num3 <<= num2 - 1;
			}
			
			return (short)((num == 0) ? num3 : (-(short)num3));
		}
	}
}
