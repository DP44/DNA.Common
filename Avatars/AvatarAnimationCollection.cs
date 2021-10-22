using System;
using DNA.Drawing.Animation;

namespace DNA.Avatars
{
	public class AvatarAnimationCollection : LayeredAnimationPlayer
	{
		private Avatar _avatar;
		private AnimationPlayer[,] players;
		private int[] currentPlayers;
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="avatar">The avatar for the animations.</param>
		public AvatarAnimationCollection(Avatar avatar) : base(16)
		{
			this._avatar = avatar;
			this.players = new AnimationPlayer[16, 3];
			this.currentPlayers = new int[16];
		}
		
		/// <summary>
		/// Play an animation.
		/// </summary>
		/// <param name="id">The animation ID.</param>
		/// <param name="channel">The animation channel.</param>
		/// <param name="blendTime">How long it takes for animation blending 
		/// 						to transition into the animation.</param>
		public AnimationPlayer Play(string id, int channel, TimeSpan blendTime)
		{
			int currentPlayer = this.currentPlayers[channel];
			AnimationPlayer player = this.players[channel, currentPlayer];
			
			if (player == null)
			{
				player = (this.players[channel, currentPlayer] = 
					AvatarAnimationManager.Instance.GetAnimation(id, this._avatar.IsMale));
			}
			else
			{
				AvatarAnimationManager.Instance.GetAnimation(player, id, this._avatar.IsMale);
			}
			
			this.currentPlayers[channel] = (this.currentPlayers[channel] + 1) % 3;
			
			base.PlayAnimation(channel, player, blendTime);
			
			return player;
		}
	}
}
