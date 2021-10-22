using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace DNA.Drawing.Animation
{
	public class SkinedAnimationData : AnimationData
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public SkinedAnimationData(Dictionary<string, AnimationClip> animationClips, 
								   List<Matrix> inverseBindPose, Skeleton skeleton) 
			: base(animationClips)
		{
			this.InverseBindPose = inverseBindPose.ToArray();
			this.Skeleton = skeleton;
		}

		/// <summary>
		/// 
		/// </summary>
		private SkinedAnimationData() {}

		/// <summary>
		/// 
		/// </summary>
		[ContentSerializer]
		public Matrix[] InverseBindPose { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		[ContentSerializer]
		public Skeleton Skeleton { get; private set; }
	}
}
