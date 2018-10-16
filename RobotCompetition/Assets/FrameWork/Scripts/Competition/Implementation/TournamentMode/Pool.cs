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
		/// <summary>
		///		Contains one Competitor's info.
		/// </summary>
		private class Competitor
		{
			public Type Type = null;
			public int Score = 0;
			public bool IsDefeated = false;
		}

		private List<Competitor> Competitors = new List<Competitor>();


		public Pool()
		{
			if (Competitors == null)
				Competitors = new List<Competitor>();
			else
				Competitors.Clear();
		}
		public Pool(List<Type> competitors)
		{
			if (Competitors == null)
				Competitors = new List<Competitor>();
			else
				Competitors.Clear();

			foreach (Type t in competitors)
			{
				this.Competitors.Add(new Competitor() { Type = t });
			}
		}


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
			SortToScore();
			return Competitors[0].Score == Competitors[1].Score;
		}
		/// <summary>
		///		Sort all competitors according to their score.
		/// </summary>
		public void SortToScore()
		{
			Competitors.Sort((c1, c2) => c1.Score.CompareTo(c2.Score));
		}
	}
}
