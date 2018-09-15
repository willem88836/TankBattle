using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
	[Serializable]
	public struct Pool
	{
		public List<Type> Competitors;
		public int[] Score;


		public Pool(List<Type> competitors)
		{
			Competitors = competitors;
			Score = new int[competitors.Count];
		}


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
