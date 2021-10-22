using System;
using System.Collections.Generic;
using DNA.Drawing.Animation;
using DNA.Net.GamerServices;

namespace DNA.Avatars
{
	public class AvatarAnimationManager
	{
		private class AvatarAnimationSet
		{
			public AnimationClip Male;
			public AnimationClip Female;

			public bool[] InfluencedBones;
			public bool Looping;
		
			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			public AnimationClip GetClip(bool male) => 
				male ? (this.Male == null ? this.Female : this.Male) 
					 : (this.Female == null ? this.Male : this.Female);
		
			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			public AvatarAnimationSet(AnimationClip male, AnimationClip female, 
									  bool[] influencedBones, bool looping)
			{
				this.Male = male;
				this.Female = female;
				this.InfluencedBones = influencedBones;
				this.Looping = looping;
			}
		}

		public static AvatarAnimationManager Instance = new AvatarAnimationManager();
	
		private Dictionary<string, AvatarAnimationManager.AvatarAnimationSet> _clips = 
			new Dictionary<string, AvatarAnimationManager.AvatarAnimationSet>();
	
		public static readonly AnimationClip DefaultAnimationClip;
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void RegisterAnimation(string name, AnimationClip clip, bool looping, 
									  IList<AvatarBone> bones, IList<AvatarBone> maskedBones)
		{
			this._clips[name] = new AvatarAnimationManager.AvatarAnimationSet(clip, 
				(AnimationClip)null, Avatar.GetInfluncedBoneList(bones, maskedBones), looping);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void RegisterAnimation(string name, AnimationClip maleClip, 
									  AnimationClip femaleClip, bool looping, 
									  IList<AvatarBone> bones, IList<AvatarBone> maskedBones)
		{
			this._clips[name] = new AvatarAnimationManager.AvatarAnimationSet(maleClip, 
				femaleClip, Avatar.GetInfluncedBoneList(bones, maskedBones), looping);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void RegisterAnimation(string name, AnimationClip clip, bool looping, 
									  IList<AvatarBone> bones)
		{
			this._clips[name] = new AvatarAnimationManager.AvatarAnimationSet(clip,
				(AnimationClip)null, Avatar.GetInfluncedBoneList(bones), looping);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void RegisterAnimation(string name, AnimationClip maleClip, 
									  AnimationClip femaleClip, bool looping, 
									  IList<AvatarBone> bones)
		{
			this._clips[name] = new AvatarAnimationManager.AvatarAnimationSet(maleClip, 
				femaleClip, Avatar.GetInfluncedBoneList(bones), looping);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void RegisterAnimation(string name, AnimationClip clip, bool looping)
		{
			this._clips[name] = new AvatarAnimationManager.AvatarAnimationSet(clip, 
				(AnimationClip)null, (bool[])null, looping);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void RegisterAnimation(string name, AnimationClip maleClip, 
									  AnimationClip femaleClip, bool looping)
		{
			this._clips[name] = new AvatarAnimationManager.AvatarAnimationSet(maleClip, 
				femaleClip, (bool[])null, looping);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public AnimationPlayer GetAnimation(string name, bool male)
		{
			AvatarAnimationManager.AvatarAnimationSet clip = this._clips[name];
	
			AnimationPlayer animPlayer = new AnimationPlayer(clip.GetClip(male));
		
			animPlayer.Name = name;
			animPlayer.Looping = clip.Looping;
			animPlayer.SetInfluncedBones(clip.InfluencedBones);
		
			return animPlayer;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void GetAnimation(AnimationPlayer player, string name, bool male)
		{
			AvatarAnimationManager.AvatarAnimationSet clip = this._clips[name];
			
			player.SetClip(clip.GetClip(male));
			player.Name = name;
			player.Looping = clip.Looping;
			player.SetInfluncedBones(clip.InfluencedBones);
		}
		
		/// <summary>
		/// Constructor
		/// </summary>
		private AvatarAnimationManager() {}
	}
}
