﻿using System;
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
		public int round { get; private set; }
		public int match { get; private set;}


		public override void Initialize()
		{
			Pools = new List<Pool>();
			Type[] competitors = LoadBehaviours();

			EnrollCompetitors(competitors);

			roundRange = new Int2(0, Pools.Count);
		}

		public override void Next()
		{
			Pool pool = Pools[round];

			Debug.Log((match >= MatchCount) + " " + (!pool.IsTied()));
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

			StartNewRound();
			match++;
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
			Pool pool = Pools[round];
			int i = pool.Competitors.LastIndexOf(winner);
			pool.Score[i]++;
			Spawner.Clear();
			Debug.LogFormat("Matches finished: {0} won!", winner.ToString());
		}

		private void OnStageFinish()
		{
			Debug.Log("stage finished");
			// Adds the winners of the previous pools to new ones.
			Type[] winners = new Type[roundRange.Y - roundRange.X];
			for (int i = roundRange.X; i < roundRange.Y; i++)
			{
				Type winner = Pools[i].FetchWinner();
				winners[roundRange.Y - i] = winner;
			}
			Debug.Log(winners.Length);	
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


		private void EnrollCompetitors(Type[] competitors)
		{
			int poolCount = Mathf.CeilToInt(competitors.Length / PoolSize);

			for (int i = 0; i < poolCount; i++)
			{
				Pool pool = new Pool();
				Debug.Log(Pools.Count + " pool contains:");
				for (int j = 0; j < PoolSize; j++)
				{
					int k = j + i * poolCount;

					if (k >= competitors.Length)
						break;

					Type current = competitors[k];
					pool.Add(current);
					Debug.Log("Tank: " + current.ToString());
				}
				Pools.Add(pool);
			}
		}
	}
}
