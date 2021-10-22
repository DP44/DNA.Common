using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

namespace DNA.Audio
{
	public class SoundManager
	{
		public static AudioListener ActiveListener;
		public static SoundManager Instance = new SoundManager();
		private AudioEngine _engine;
		private SoundBank _soundBank;
		private WaveBank _waveBank;
		private WaveBank _streamBank;
		public ReverbSettings ReverbSettings = ReverbSettings.Mountains;
		private Dictionary<string, float> _globalVars = new Dictionary<string, float>();
		private LinkedList<SoundCue3D> _activeSounds = new LinkedList<SoundCue3D>();
		private bool DoReverb = true;
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void Load(string name)
		{
			this._engine = new AudioEngine("Content\\" + name + ".xgs");
			this._soundBank = new SoundBank(this._engine, "Content\\" + name + ".xsb");
			this._waveBank = new WaveBank(this._engine, "Content\\" + name + ".xwb");
			
			try
			{
				this._streamBank = new WaveBank(this._engine, "Content\\" + name + "Streaming.xwb", 0, 32);
			}
			catch
			{
				// Swallow exceptions.
			}

			do
			{
				this._engine.Update();
			}
			while ((this._streamBank != null && !this._streamBank.IsPrepared) || !this._waveBank.IsPrepared);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public float GetGlobalVarible(string varibleName)
		{
			return this._engine.GetGlobalVariable(varibleName);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void SetGlobalVarible(string varibleName, float value)
		{
			float num;
			
			if (!this._globalVars.TryGetValue(varibleName, out num) || num != value)
			{
				this._engine.SetGlobalVariable(varibleName, value);
				this._globalVars[varibleName] = value;
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public AudioCategory GetCatagory(string name)
		{
			return this._engine.GetCategory(name);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public Cue GetCue(string name)
		{
			return this._soundBank.GetCue(name);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public Cue PlayInstance(string name)
		{
			Cue cue = this.GetCue(name);
			cue.Play();
			return cue;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public SoundCue3D PlayInstance(string name, AudioEmitter emitter)
		{
			Cue cue = this.GetCue(name);
			cue.Apply3D(SoundManager.ActiveListener, emitter);
			cue.Play();
			SoundCue3D soundCue3D = new SoundCue3D(cue, emitter);
			this._activeSounds.AddLast(soundCue3D);
			return soundCue3D;
		}
		
		/// <summary>
		/// 
		/// </summary>
		public void Update()
		{
			if (this._engine != null)
			{
				this._engine.Update();
				
				if (this.DoReverb)
				{
					try
					{
						this.SetGlobalVarible("ReverbReflectionsGain", this.ReverbSettings.ReflectionsGain);
						this.SetGlobalVarible("ReverbReverbGain", this.ReverbSettings.ReverbGain);
						this.SetGlobalVarible("ReverbDecayTime", this.ReverbSettings.DecayTime);
						this.SetGlobalVarible("ReverbReflectionsDelay", this.ReverbSettings.ReflectionsDelay);
						this.SetGlobalVarible("ReverbReverbDelay", this.ReverbSettings.ReverbDelay);
						this.SetGlobalVarible("ReverbRearDelay", this.ReverbSettings.RearDelay);
						this.SetGlobalVarible("ReverbRoomSize", this.ReverbSettings.RoomSize);
						this.SetGlobalVarible("ReverbDensity", this.ReverbSettings.Density);
						this.SetGlobalVarible("ReverbLowEQGain", this.ReverbSettings.LowEQGain);
						this.SetGlobalVarible("ReverbLowEQCutoff", this.ReverbSettings.LowEQCutoff);
						this.SetGlobalVarible("ReverbHighEQGain", this.ReverbSettings.HighEQGain);
						this.SetGlobalVarible("ReverbHighEQCutoff", this.ReverbSettings.HighEQCutoff);
						this.SetGlobalVarible("ReverbPositionLeft", this.ReverbSettings.PositionLeft);
						this.SetGlobalVarible("ReverbPositionRight", this.ReverbSettings.PositionRight);
						this.SetGlobalVarible("ReverbPositionLeftMatrix", this.ReverbSettings.PositionLeftMatrix);
						this.SetGlobalVarible("ReverbPositionRightMatrix", this.ReverbSettings.PositionRightMatrix);
						this.SetGlobalVarible("ReverbEarlyDiffusion", this.ReverbSettings.EarlyDiffusion);
						this.SetGlobalVarible("ReverbLateDiffusion", this.ReverbSettings.LateDiffusion);
						this.SetGlobalVarible("ReverbRoomFilterMain", this.ReverbSettings.RoomFilterMain);
						this.SetGlobalVarible("ReverbRoomFilterFrequency", this.ReverbSettings.RoomFilterFrequency);
						this.SetGlobalVarible("ReverbRoomFilterHighFrequency", this.ReverbSettings.RoomFilterHighFrequency);
						this.SetGlobalVarible("ReverbWetDryMix", this.ReverbSettings.WetDryMix);
					}
					catch
					{
						this.DoReverb = false;
					}
				}
			}
		
			LinkedListNode<SoundCue3D> next;
			
			for (LinkedListNode<SoundCue3D> linkedListNode = this._activeSounds.First; linkedListNode != null; linkedListNode = next)
			{
				next = linkedListNode.Next;
			
				if (linkedListNode.Value.IsStopped)
				{
					this._activeSounds.Remove(linkedListNode);
				}
				else
				{
					linkedListNode.Value.Apply3D(SoundManager.ActiveListener);
				}
			}
		}
	}
}
