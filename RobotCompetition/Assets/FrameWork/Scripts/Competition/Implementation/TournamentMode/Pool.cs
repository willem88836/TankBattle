using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Competition
{
	/// <summary>
	///		Contains all information used inside one competitor pool.
	/// </summary>
	[Serializable]
	public class Pool
	{
		public List<Type> Competitors;
		public List<int> Score;

		public Pool()
		{
			Competitors = new List<Type>();
			Score = new List<int>();
		}

		public Pool(List<Type> competitors)
		{
			Competitors = competitors;
			Score = new List<int>(competitors.Count);
		}

		/// <summary>
		///		Adds a competitor to the pool.
		/// </summary>
		public void Add(Type competitor)
		{
			Competitors.Add(competitor);
			Score.Add(0);
		}

		/// <summary>
		///		Removes a competitor from the pool.
		/// </summary>
		public void Remove(Type competitor)
		{
			int index = Competitors.IndexOf(competitor);
			Competitors.RemoveAt(index);
			Score.RemoveAt(index);
		}

		/// <summary>
		///		Returns true if there are two or more
		///		competitors with the same highest score.
		/// </summary>
		public bool IsTied()
		{
			Type winner = FetchWinner();
			int index = Competitors.IndexOf(winner);

			for (int i = 0; i < Competitors.Count; i++)
			{
				if (Competitors[i] == winner)
					continue;

				if (Score[i] == Score[index])
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		///		Returns the winner's Type.
		/// </summary>
		public Type FetchWinner()
		{
			int score = 0;
			int index = 0;

			for (int i = 0; i < Competitors.Count; i++)
			{
				int s = Score[i];
				if (s > score)
				{
					score = s;
					index = i;
				}
			}

			return Competitors[index];
		}
	}
}
