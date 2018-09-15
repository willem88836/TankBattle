using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
	/// <summary>
	///		Note to self: 
	///			Match = match
	///			Round = multiple matches
	///			Stage = multiple rounds
	/// </summary>
	public sealed class TournamentManager : CompetitionManager
	{
		[Header("Tournament Settings")]
		public int PoolSize;
		public int MatchCount;


		private Int2 roundRange;
		private int round = 0;
		private int match = 0;


		public override void Initialize()
		{
			Pools = new List<Pool>();
			Type[] competitors = LoadBehaviours();

			EnrollCompetitors(competitors);

			roundRange = new Int2(0, Pools.Count);
		}

		public override void Next()
		{
			Pool pool = Pools[round - 1];

			if (match >= MatchCount && !pool.IsTied())
			{
				round++;
				match = 0;

				if (round > roundRange.Y)
				{
					OnStageFinish();
					return;
				}
			}
			else
			{
				match++;
			}

			StartNewRound();
		}

		private void StartNewRound()
		{
			Pool pool = Pools[round];

			Spawner.Clear();

			for (int i = 0; i < pool.Competitors.Count; i++)
			{
				Type tankBehaviour = pool.Competitors[i];
				Spawner.Spawn(tankBehaviour);
			}
		}

		public override void OnMatchFinish(Type winner)
		{
			Pool pool = Pools[round - 1];
			int i = pool.Competitors.LastIndexOf(winner);
			pool.Score[i]++;
		}

		private void OnStageFinish()
		{
			// Adds the winners of the previous pools to new ones.
			Type[] winners = new Type[roundRange.Y - roundRange.X];
			for (int i = roundRange.X; i < roundRange.Y; i++)
			{
				Type winner = Pools[i].FetchWinner();
				winners[roundRange.Y - i] = winner;
			}

			if (winners.Length == 1)
			{
				OnGameFinish(winners[0]);
			}
			else
			{
				EnrollCompetitors(winners);
				roundRange.X = roundRange.Y;
				roundRange.Y = Pools.Count;
			}

			// todo: a new match should be initialized.
		}


		private void EnrollCompetitors(Type[] competitors)
		{
			int poolCount = Mathf.CeilToInt(competitors.Length / PoolSize);

			for (int i = 0; i < poolCount; i++)
			{
				Pool pool = new Pool();
				for (int j = 0; j < PoolSize; j++)
				{
					int k = j + i * poolCount;

					if (k >= competitors.Length)
						break;

					Type current = competitors[k];
					pool.Competitors.Add(current);
				}
				Pools.Add(pool);
			}
		}
	}
}
