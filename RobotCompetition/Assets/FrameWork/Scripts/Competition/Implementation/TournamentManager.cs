using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
	public sealed class TournamentManager : CompetitionManager
	{
		[Header("Tournament Settings")]
		public int PoolSize;
		public int MatchCount;


		private Int2 roundRange;
		private int round = 0;


		public override void Initialize()
		{
			Pools = new List<Pool>();
			MonoBehaviour[] competitors = LoadBehaviours();

			EnrollCompetitors(competitors);

			roundRange = new Int2(0, Pools.Count);
		}

		public override void OnPoolFinish(MonoBehaviour winner)
		{
			Pool pool = Pools[round - 1];
			// todo: continue here and determine whether a next round should be played .
			int i = pool.Competitors.LastIndexOf(winner);
			pool.Score[i]++;
		}

		public override void Next()
		{
			Pool currentPool = Pools[round];

			for (int i = 0; i < currentPool.Competitors.Count; i++)
			{
				MonoBehaviour tankBehaviour = currentPool.Competitors[i];
				Spawner.Spawn(tankBehaviour);
			}

			round++;

			if (round >= roundRange.Y)
			{
				OnRoundFinish();
			}
		}
		

		private void OnRoundFinish()
		{
			// Adds the winners of the previous pools to new ones.
			MonoBehaviour[] winners = new MonoBehaviour[roundRange.Y - roundRange.X];
			for (int i = roundRange.X; i < roundRange.Y; i++)
			{
				MonoBehaviour winner = Pools[i].FetchWinner();
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
		}

		private void EnrollCompetitors(MonoBehaviour[] competitors)
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

					MonoBehaviour current = competitors[k];
					pool.Competitors.Add(current);
				}
				Pools.Add(pool);
			}
		}
	}
}
