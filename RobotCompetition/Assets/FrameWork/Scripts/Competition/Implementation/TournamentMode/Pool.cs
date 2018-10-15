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
		private class Competitor
		{
			public Type Type;
			public int Score;
			public bool IsDefeated;
		}

		private List<Competitor> Competitors = new List<Competitor>();

		public Type TypeAt(int index)
		{
			return Competitors[index].Type;
		}
		public int ScoreAt(int index)
		{
			return Competitors[index].Score;
		}
		public bool IsDefeatedAt(int index)
		{
			return Competitors[index].IsDefeated;
		}


		public Pool()
		{
			Competitors = new List<Competitor>();
		}

		public Pool(List<Type> competitors)
		{
			Competitors.Clear();
			foreach(Type t in competitors)
			{
				this.Competitors.Add(new Competitor() { Type = t });
			}
		}

		/// <summary>
		///		Adds a competitor to the pool.
		/// </summary>
		public void Add(Type competitor)
		{
			Competitors.Add(new Competitor() { Type = competitor });
		}

		/// <summary>
		///		Removes a competitor from the pool.
		/// </summary>
		public void Remove(Type competitor)
		{
			Competitors.Remove(
				Competitors.Find(
					(Competitor c) => { return c.Type == competitor; }));
		}

		/// <summary>
		///		Returns true if there are two or more
		///		competitors with the same highest score.
		/// </summary>
		public bool IsTied()
		{
			// TODO: Continue here with Pool Update.
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
