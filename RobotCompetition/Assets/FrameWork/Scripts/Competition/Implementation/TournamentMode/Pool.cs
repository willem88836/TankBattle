using System;
using System.Collections.Generic;

namespace Framework.Competition
{
	/// <summary>
	///		Contains all information used inside one competitor pool.
	/// </summary>
	public class Pool
	{

		private List<Competitor> Competitors = new List<Competitor>();

		public int Count { get { return Competitors == null ? 0 : Competitors.Count; } }


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


		public Competitor CompetitorAt(int index)
		{
			return Competitors[index];
		}
		public Competitor CompetitorAs(Type type)
		{
			return Competitors.Find((Competitor c) => c.Type == type);
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
			Competitors.Sort((c1, c2) => c2.Score.CompareTo(c1.Score));
		}
		
		public Type GetFirstAlive()
		{
			foreach(Competitor c in Competitors)
			{
				if (!c.IsDefeated)
					return c.Type;
			}

			return null;
		}
	}
}
