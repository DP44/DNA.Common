using System;
using System.Collections.Generic;

namespace DNA.Drawing.Animation
{
	public abstract class BaseAnimationPlayer
	{
		private string _name;
		private float _speed = 1f;
		private bool _playing = true;

		/// <summary>
		/// 
		/// </summary>
		public virtual string Name
		{
			get =>
				return this._name;
			
			set =>
				this._name = value;
		}

		/// <summary>
		/// 
		/// </summary>
		public float Speed
		{
			get =>
				return this._speed;

			set =>
				this._speed = value;
		}

		/// <summary>
		/// 
		/// </summary>
		public bool Playing =>
			this._playing;

		/// <summary>
		/// 
		/// </summary>
		public void Play() =>
			this._playing = true;

		/// <summary>
		/// 
		/// </summary>
		public void Pause() => 
			this._playing = false;

		/// <summary>
		/// 
		/// </summary>
		public void Stop()
		{
			this._playing = false;
			this.Reset();
		}

		/// <summary>
		/// 
		/// </summary>
		protected virtual void Reset() => 
			this._speed = 1f;

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public abstract void Update(TimeSpan timeSpan, IList<Bone> boneTransforms);
	}
}
