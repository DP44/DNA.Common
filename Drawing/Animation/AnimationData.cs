using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace DNA.Drawing.Animation
{
	public class AnimationData
	{
		/// <summary>
		/// 
		/// </summary>
		[ContentSerializer]
		public Dictionary<string, AnimationClip> AnimationClips { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public AnimationData(Dictionary<string, AnimationClip> animationClips) =>
			this.AnimationClips = animationClips;

		/// <summary>
		/// 
		/// </summary>
		protected AnimationData() {}
	}
}
