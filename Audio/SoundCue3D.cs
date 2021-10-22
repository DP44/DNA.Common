using System;
using Microsoft.Xna.Framework.Audio;

namespace DNA.Audio
{
	public class SoundCue3D
	{
		private AudioEmitter _emitter;
		private Cue _cue;
		
		/// <summary>
		/// 
		/// </summary>
		public AudioEmitter AudioEmitter
		{
			get
			{
				return this._emitter;
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		public Cue Cue
		{
			get
			{
				return this._cue;
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void Set(Cue cue, AudioEmitter emitter)
		{
			this._cue = cue;
			this._emitter = emitter;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public SoundCue3D(Cue cue, AudioEmitter emitter)
		{
			this._cue = cue;
			this._emitter = emitter;
		}
		
		/// <summary>
		/// 
		/// </summary>
		public bool IsCreated
		{
			get
			{
				return this.Cue.IsCreated;
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		public bool IsDisposed
		{
			get
			{
				return this.Cue.IsDisposed;
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		public bool IsPaused
		{
			get
			{
				return this.Cue.IsPaused;
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		public bool IsPlaying
		{
			get
			{
				return this.Cue.IsPlaying;
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		public bool IsPrepared
		{
			get
			{
				return this.Cue.IsPrepared;
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		public bool IsPreparing
		{
			get
			{
				return this.Cue.IsPreparing;
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		public bool IsStopped
		{
			get
			{
				return this.Cue.IsStopped;
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		public bool IsStopping
		{
			get
			{
				return this.Cue.IsStopping;
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		public string Name
		{
			get
			{
				return this.Cue.Name;
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		public void Dispose()
		{
			this.Cue.Dispose();
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public float GetVariable(string name)
		{
			return this.Cue.GetVariable(name);
		}
		
		/// <summary>
		/// 
		/// </summary>
		public void Pause()
		{
			this.Cue.Pause();
		}
		
		/// <summary>
		/// 
		/// </summary>
		public void Play()
		{
			this.Cue.Play();
		}
		
		/// <summary>
		/// 
		/// </summary>
		public void Resume()
		{
			this.Cue.Resume();
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void SetVariable(string name, float value)
		{
			this.Cue.SetVariable(name, value);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void Stop(AudioStopOptions options)
		{
			this.Cue.Stop(options);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void Apply3D(AudioListener listener)
		{
			this._cue.Apply3D(listener, this._emitter);
		}
	}
}
