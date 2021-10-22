using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DNA.Drawing.Animation
{
	public class AnimBlender
	{
		private AnimationPlayer _currentAnimation;
		private AnimationPlayer _previousAnimation;

		private TimeSpan _blendTotalTime = TimeSpan.FromSeconds(0.25);
		private TimeSpan _blendCurrentTime;

		/// <summary>
		/// 
		/// </summary>
		public AnimationPlayer ActiveAnimation =>
			this._currentAnimation;

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void Play(AnimationPlayer nextAnimation, TimeSpan blendTime)
		{
			this._previousAnimation = this._currentAnimation;
			this._currentAnimation = nextAnimation;
			this._blendTotalTime = blendTime;
			this._blendCurrentTime = TimeSpan.Zero;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void Update(TimeSpan elapsedAnimationTime, IList<Bone> bones)
		{
			if (this._currentAnimation == null && this._previousAnimation == null)
			{
				return;
			}
			
			if (this._currentAnimation != null)
			{
				this._currentAnimation.Update(elapsedAnimationTime);
			}
			
			if (this._previousAnimation != null)
			{
				this._previousAnimation.Update(elapsedAnimationTime);
			}
			
			this._blendCurrentTime += elapsedAnimationTime;
			
			float num = 1f;
			
			if (this._blendTotalTime.TotalSeconds > 0.0)
			{
				num = (float)(
					this._blendCurrentTime.TotalSeconds / this._blendTotalTime.TotalSeconds);
			}

			if (num < 1f)
			{
				for (int i = 0; i < bones.Count; i++)
				{
					if (this._previousAnimation != null && 
						this._previousAnimation.GetBoneInfluence(i))
					{
						if (this._currentAnimation != null && 
							this._currentAnimation.GetBoneInfluence(i))
						{
							bones[i].Translation = 
								Vector3.Lerp(this._previousAnimation.Translations[i], 
											 this._currentAnimation.Translations[i], num);
							
							bones[i].Rotation =
								Quaternion.Slerp(this._previousAnimation.Rotations[i], 
												 this._currentAnimation.Rotations[i], num);
							
							bones[i].Scale = 
								Vector3.Lerp(this._previousAnimation.Scales[i], 
											 this._currentAnimation.Scales[i], num);
						}
						else
						{
							bones[i].Translation = 
								Vector3.Lerp(this._previousAnimation.Translations[i], 
											 bones[i].Translation, num);
						
							bones[i].Rotation = 
								Quaternion.Slerp(this._previousAnimation.Rotations[i], 
											     bones[i].Rotation, num);
							
							bones[i].Scale = 
								Vector3.Lerp(this._previousAnimation.Scales[i], 
											 bones[i].Scale, num);
						}
					}
					else if (this._currentAnimation != null && 
							 this._currentAnimation.GetBoneInfluence(i))
					{
						bones[i].Translation = 
							Vector3.Lerp(bones[i].Translation, 
										 this._currentAnimation.Translations[i], num);
						
						bones[i].Rotation = 
							Quaternion.Slerp(bones[i].Rotation, 
										 this._currentAnimation.Rotations[i], num);
						
						bones[i].Scale = 
							Vector3.Lerp(bones[i].Scale, 
										 this._currentAnimation.Scales[i], num);
					}
				}

				return;
			}
			
			this._previousAnimation = null;
			
			if (this._currentAnimation == null)
			{
				return;
			}
			
			int count = bones.Count;
			
			for (int j = 0; j < count; j++)
			{
				if (this._currentAnimation.GetBoneInfluence(j))
				{
					bones[j].SetTransform(ref this._currentAnimation.Translations[j], 
										  ref this._currentAnimation.Rotations[j], 
										  ref this._currentAnimation.Scales[j]);
				}
			}
		}
	}
}
