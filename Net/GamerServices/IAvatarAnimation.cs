using System;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;

namespace DNA.Net.GamerServices
{
	public interface IAvatarAnimation
	{
		ReadOnlyCollection<Matrix> BoneTransforms { get; }
		TimeSpan CurrentPosition { get; set; }
		AvatarExpression Expression { get; }
		TimeSpan Length { get; }

		void Update(TimeSpan elapsedAnimationTime, bool loop);
	}
}
