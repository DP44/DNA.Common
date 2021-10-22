using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DNA.Drawing.Animation
{
	public class AnimationPlayer : BaseAnimationPlayer
	{
		private AnimationClip _currentAnimationClip;

		private bool[] _influencedBones;

		private bool _onPong;
		private bool _reversed;
		private bool _looping = true;
		private bool _pingPong;

		private TimeSpan _currentTime = TimeSpan.Zero;

		private Vector3[] _translations;
		private Quaternion[] _rotations;
		private Vector3[] _scales;

		public bool GetBoneInfluence(int boneNumber) =>
			this._influencedBones == null || this._influencedBones[boneNumber];

		public override string Name
		{
			get =>
				base.Name != null ? base.Name : this._currentAnimationClip.Name;

			set =>
				base.Name = value;
		}

		public bool Finished
		{
			get
			{
				if (this.Looping)
				{
					return false;
				}

				if (this.PingPong && this._onPong)
				{
					return this.Progress <= 0f;
				}

				return this.Progress >= 1f;
			}
		}

		public bool Reversed
		{
			get =>
				this._reversed;

			set =>
				this._reversed = value;
		}

		public bool Looping
		{
			get =>
				this._looping;

			set =>
				this._looping = value;
		}

		public bool PingPong
		{
			get =>
				this._pingPong;

			set =>
				this._pingPong = value;
		}

		public TimeSpan CurrentTime
		{
			get =>
				this._currentTime;
			
			set =>
				this._currentTime = value;
		}

		public float Progress
		{
			get =>
				(float)(this._currentTime.TotalSeconds / this.Duration.TotalSeconds);

			set
			{
				this._onPong = false;
				
				this._currentTime = 
					TimeSpan.FromSeconds((double)value * this.Duration.TotalSeconds);
			}
		}

		public TimeSpan Duration =>
			this._currentAnimationClip.Duration;

		protected override void Reset()
		{
			this._onPong = false;
			this._currentTime = TimeSpan.Zero;
			base.Reset();
		}

		public void SetInfluncedBones(bool[] boneArray)
		{
			if (boneArray != null && 
				boneArray.Length != this._currentAnimationClip.BoneCount)
			{
				throw new Exception("bone array size mismatch");
			}
			if (boneArray == null)
			{
				if (this._influencedBones != null)
				{
					for (int i = 0; i < this._influencedBones.Length; i++)
					{
						this._influencedBones[i] = true;
					}

					return;
				}
			}
			else
			{
				if (this._influencedBones == null || 
					this._influencedBones.Length != this._currentAnimationClip.BoneCount)
				{
					this._influencedBones = new bool[this._currentAnimationClip.BoneCount];
				}

				boneArray.CopyTo(this._influencedBones, 0);
			}
		}

		public void ResetInfluncedBones() =>
			this._influencedBones = null;

		public Vector3[] Translations =>
			this._translations;

		public Quaternion[] Rotations =>
			this._rotations;

		public Vector3[] Scales =>
			this._scales;

		public void SetClip(AnimationClip clip)
		{
			this._onPong = false;
			this._reversed = false;
			this._looping = true;
			this._pingPong = false;
			this._currentTime = TimeSpan.Zero;
			base.Speed = 1f;
			this._currentAnimationClip = clip;
			
			if (this._influencedBones == null || 
				this._influencedBones.Length != this._currentAnimationClip.BoneCount)
			{
				this._influencedBones = new bool[this._currentAnimationClip.BoneCount];
			}
			
			for (int i = 0; i < this._influencedBones.Length; i++)
			{
				this._influencedBones[i] = true;
			}
			
			if (this._translations.Length != this._currentAnimationClip.BoneCount)
			{
				this._translations = new Vector3[this._currentAnimationClip.BoneCount];
			}
			
			if (this._rotations.Length != this._currentAnimationClip.BoneCount)
			{
				this._rotations = new Quaternion[this._currentAnimationClip.BoneCount];
			}
			
			if (this._scales.Length != this._currentAnimationClip.BoneCount)
			{
				this._scales = new Vector3[this._currentAnimationClip.BoneCount];
			}
		}

		public void SetClip(AnimationClip clip, IList<Bone> influenceBones)
		{
			this._onPong = false;
			this._reversed = false;
			this._looping = true;
			this._pingPong = false;
			this._currentTime = TimeSpan.Zero;
			base.Speed = 1f;
			this._currentAnimationClip = clip;
			
			if (this._influencedBones == null || 
				this._influencedBones.Length != this._currentAnimationClip.BoneCount)
			{
				this._influencedBones = new bool[this._currentAnimationClip.BoneCount];
			}
			
			for (int i = 0; i < influenceBones.Count; i++)
			{
				this._influencedBones[influenceBones[i].Index] = true;
			}
			
			if (this._translations.Length != this._currentAnimationClip.BoneCount)
			{
				this._translations = new Vector3[this._currentAnimationClip.BoneCount];
			}
			
			if (this._rotations.Length != this._currentAnimationClip.BoneCount)
			{
				this._rotations = new Quaternion[this._currentAnimationClip.BoneCount];
			}
			
			if (this._scales.Length != this._currentAnimationClip.BoneCount)
			{
				this._scales = new Vector3[this._currentAnimationClip.BoneCount];
			}
		}

		public AnimationPlayer(AnimationClip clip, IList<Bone> influenceBones)
		{
			this._currentAnimationClip = clip;
			this._influencedBones = new bool[this._currentAnimationClip.BoneCount];
			
			for (int i = 0; i < influenceBones.Count; i++)
			{
				this._influencedBones[influenceBones[i].Index] = true;
			}

			this._translations = new Vector3[this._currentAnimationClip.BoneCount];
			this._rotations = new Quaternion[this._currentAnimationClip.BoneCount];
			this._scales = new Vector3[this._currentAnimationClip.BoneCount];
		}

		public AnimationPlayer(AnimationClip clip)
		{
			this._currentAnimationClip = clip;
			this._scales = new Vector3[this._currentAnimationClip.BoneCount];
			this._translations = new Vector3[this._currentAnimationClip.BoneCount];
			this._rotations = new Quaternion[this._currentAnimationClip.BoneCount];
		}

		public void Update(TimeSpan timeSpan)
		{
			TimeSpan t = 
				TimeSpan.FromSeconds(timeSpan.TotalSeconds * (double)base.Speed);
			
			if (base.Playing)
			{
				if (this._onPong)
				{
					this._currentTime -= t;
				}
				else
				{
					this._currentTime += t;
				}
			}
			
			if (this._currentTime > this.Duration)
			{
				if (this.PingPong)
				{
					this._currentTime = 
						this.Duration - (this._currentTime - this.Duration);
					
					this._onPong = true;
				}
				else if (this.Looping)
				{
					while (this._currentTime > this.Duration)
					{
						this._currentTime -= this.Duration;
					}
				}
				else
				{
					this._currentTime = this.Duration;
				}
			}
			else if (this._currentTime < TimeSpan.Zero)
			{
				if (this.PingPong)
				{
					if (this.Looping)
					{
						this._onPong = false;
						this._currentTime = -this._currentTime;
					}
					else
					{
						this._currentTime = TimeSpan.Zero;
					}
				}
				else if (this.Looping)
				{
					while (this._currentTime < TimeSpan.Zero)
					{
						this._currentTime += this.Duration;
					}
				}
				else
				{
					this._currentTime = TimeSpan.Zero;
				}
			}
			
			TimeSpan position = this._currentTime;
			
			if (this._reversed)
			{
				position = this.Duration - this._currentTime;
			}

			this._currentAnimationClip.CopyTransforms(
				this._translations, this._rotations, 
				this._scales, position, this._influencedBones);
		}

		public override void Update(TimeSpan timeSpan, IList<Bone> boneTransforms)
		{
			this.Update(timeSpan);
			
			for (int i = 0; i < boneTransforms.Count; i++)
			{
				if (this._influencedBones == null || this._influencedBones[i])
				{
					boneTransforms[i].SetTransform(this._translations[i], 
												   this._rotations[i], 
												   this._scales[i]);
				}
			}
		}
	}
}
