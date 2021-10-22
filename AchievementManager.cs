using System;
using System.Collections.Generic;

namespace DNA 
{
	public abstract class AchievementManager<T> where T : PlayerStats 
	{
		public abstract class Achievement 
		{
			private string _apiName;
			private string _name;
			private string _howToUnlock;
			protected bool _achieved;
			private T _stats;
			private string _reward;
			public object Tag;

			protected T PlayerStats => 
				this._stats;

			public string HowToUnlock => 
				this._howToUnlock;

			public string Reward => 
				this._reward;

			public Achievement(string apiName, AchievementManager<T> manager, string name,
							   string howToUnlock) 
			{
				this._apiName = apiName;
				this._name = name;
				this._howToUnlock = howToUnlock;
				this._stats = manager._stats;
			}

			public Achievement(string apiName, AchievementManager<T> manager, string name, 
							   string howToUnlock, string reward) 
			{
				this._apiName = apiName;
				this._name = name;
				this._howToUnlock = howToUnlock;
				this._stats = manager._stats;
				this._reward = reward;
			}

			public Achievement(AchievementManager<T> manager, string name, string howToUnlock) 
			{
				this._apiName = name;
				this._name = name;
				this._howToUnlock = howToUnlock;
				this._stats = manager._stats;
			}

			public Achievement(AchievementManager<T> manager, string name, string howToUnlock, 
							   string reward) 
			{
				this._apiName = name;
				this._name = name;
				this._howToUnlock = howToUnlock;
				this._stats = manager._stats;
				this._reward = reward;
			}

			public string APIName => 
				this._apiName;
			
			public virtual string Name => 
				this._name;

			public virtual bool Acheived => 
				this._acheived;

			protected abstract bool IsSastified { get; }
			public abstract string ProgressTowardsUnlockMessage { get; }

			public virtual float ProgressTowardsUnlock => 0.0f;

			public virtual bool Update() 
			{
				// Make sure the achievement trigger is satisfied 
				// and if the player doesn't have it already.
				if (this._achieved || !this.IsSastified)
				{
					return false;
				}

				this._achieved = true;
				return true;
			}
		}

		public class AchievementEventArgs : EventArgs 
		{
			public AchievementManager<T>.Achievement Achievement;

			public AcheimentEventArgs(AchievementManager<T>.Achievement achievement) => 
				this.Achievement = achievement;
		}

		private T _stats;

		private List<AchievementManager<T>.Achievement> _achievements = 
			new List<AchievementManager<T>.Achievement>();

		protected T PlayerStats =>
			return this._stats;

		public AchievementManager<T>.Achievement this[int index] => 
			this._acheviements[index];

		public int Count => 
			this._acheviements.Count;

		public int AddAchievement(AchievementManager<T>.Achievement achievement) 
		{
			this._achievements.Add(achievement);
			return this._achievements.Count - 1;
		}

		public AchievementManager(T stats) 
		{
			this._stats = stats;
			this.CreateAchievements();

			for (int i = 0; i < this._achievements.Count; i++) 
			{
				this._achievements[i].Update();
			}
		}

		public abstract void CreateAchievements();

		public void Update() 
		{
			for (int i = 0; i < this._achievements.Count; i++) 
			{
				if (this._achievements[i].Update()) 
				{
					this.OnAchieved(this._achievements[i]);

					if (this.Achieved != null) 
					{
						this.Achieved((object)this, 
							new AchievementManager<T>.AchievementEventArgs(
								this._achievements[i]));
					}
				}
			}
		}

		public virtual void OnAchieved(AchievementManager<T>.Achievement achievement) {}

		public event EventHandler<AchievementManager<T>.AcheimentEventArgs> Achieved;
	}
}
