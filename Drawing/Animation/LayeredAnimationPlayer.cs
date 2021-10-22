using System;
using System.Collections.Generic;

namespace DNA.Drawing.Animation
{
	public class LayeredAnimationPlayer : BaseAnimationPlayer
	{
		private AnimBlender[] _blenders;

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public AnimationPlayer this[int index] => 
			this._blenders[index].ActiveAnimation;

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public AnimationPlayer GetAnimation(int channel) => 
			this._blenders[channel].ActiveAnimation;

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void PlayAnimation(int channel, AnimationPlayer player, TimeSpan blendTime) => 
			this._blenders[channel].Play(player, blendTime);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void ClearAnimation(int channel, TimeSpan blendTime) => 
			this.PlayAnimation(channel, (AnimationPlayer)null, blendTime);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public LayeredAnimationPlayer(int channels)
		{
			this._blenders = new AnimBlender[channels];
			
			for (int i = 0; i < this._blenders.Length; i++)
			{
				this._blenders[i] = new AnimBlender();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public override void Update(TimeSpan timeSpan, IList<Bone> boneTransforms)
		{
			for (int i = 0; i < this._blenders.Length; i++)
			{
				this._blenders[i].Update(timeSpan, boneTransforms);
			}
		}
	}
}
