using System;
using System.Collections.Generic;
using System.IO;

namespace DNA
{
	public class HighScoreManager<T> where T : PlayerStats, new()
	{
		private int MaxScores = 100;
		private Comparison<T> CompareScores;
		private List<T> _scores = new List<T>();

		/// <summary>
		/// A list of all the current scores.
		/// </summary>
		public IList<T> Scores => (IList<T>)this._scores;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="maxScores">The max amount of scores accepted.</param>
		/// <param name="comparer">Scores to compare.</param>
		public HighScoreManager(int maxScores, Comparison<T> comparer) => 
			this.CompareScores = comparer;

		/// <summary>
		/// Removes a gamer's scores.
		/// </summary>
		/// <param name="gamertag">The gamer to remove.</param>
		public void DeleteGamer(string gamertag)
		{
			// Iterate through all of the scores.
			for (int scoreIndex = 0; scoreIndex < this._scores.Count; scoreIndex++)
			{
				// Check if the score is from the gamer.
				if (this._scores[scoreIndex].GamerTag == gamertag)
				{
					// Gamertag matched, remove the score and decrement the list.
					this._scores.RemoveAt(scoreIndex);
					scoreIndex--;
				}
			}
		}

		/// <summary>
		/// Updates the list of scores.
		/// </summary>
		/// <param name="newScores">A list of new scores to add.</param>
		/// <param name="currentStats">The current scores in the list.</param>
		public void UpdateScores(IList<T> newScores, T currentStats)
		{
			List<T> scores = new List<T>();
			scores.AddRange(newScores);
			currentStats.DateRecorded = DateTime.UtcNow;
			scores.Add(currentStats);
			
			Dictionary<string, int> scoreHolders = new Dictionary<string, int>();
			
			for (int i = 0; i < this._scores.Count; i++)
			{
				scoreHolders[this._scores[i].GamerTag] = i;
			}
			
			for (int scoreIndex = 0; scoreIndex < scores.Count; scoreIndex++)
			{
				T score = scores[scoreIndex];
				int index;
			
				if (scoreHolders.TryGetValue(score.GamerTag, out index))
				{
					// Make sure the previous score was achieved sooner than the date given.
					if (this._scores[index].DateRecorded < score.DateRecorded)
					{
						// Update the score at the index with the new value.
						this._scores[index] = score;
					}
				}
				else
				{
					this._scores.Add(score);
					scoreHolders[score.GamerTag] = this._scores.Count - 1;
				}
			}
			
			this._scores.Sort(this.CompareScores);
			
			// Ensure that the number of scores isn't over the maximum amount set.
			if (this._scores.Count <= this.MaxScores)
			{
				return;
			}

			this._scores.RemoveRange(this.MaxScores, this._scores.Count - this.MaxScores);
		}

		/// <summary>
		/// Saves the highscores to a file.
		/// </summary>
		/// <param name="writer">The BinaryWriter handle for the file.</param>
		public void Save(BinaryWriter writer)
		{
			writer.Write(this._scores.Count);
			
			// Save all of the existing scores in the list.
			for (int i = 0; i < this.Scores.Count; i++)
			{
				this._scores[i].Save(writer);
			}
		}

		/// <summary>
		/// Loads the highscores from a file.
		/// </summary>
		/// <param name="reader">The BinaryReader handle for the file.</param>
		public void Load(BinaryReader reader)
		{
			// Get the amount of high scores from the file.
			int highscoreCount = reader.ReadInt32();

			// Clear the existing list of scores.
			this._scores.Clear();
			
			// Add all of the scores from the file to the list of scores.
			for (int i = 0; i < highscoreCount; i++)
			{
				T score = Activator.CreateInstance<T>();
				score.Load(reader);
				this._scores.Add(score);
			}
		}
	}
}
