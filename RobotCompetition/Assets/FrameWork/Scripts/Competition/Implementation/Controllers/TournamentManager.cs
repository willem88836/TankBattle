using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Competition
{
	/// <summary>
	///		Contains all behaviour to set up and update a 
	///		tournament setting.
	/// 
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
		public int Round { get; private set; }
		public int Match { get; private set;}


		/// <summary>
		///		Creates a new Tournament.
		/// </summary>
		public override void Initialize()
		{
			Round = 0;
			Match = 0;

			Pools = new List<Pool>();
			Type[] competitors = LoadBehaviours();

			EnrollCompetitors(competitors);

			roundRange = new Int2(0, Pools.Count);
		}

		/// <inheritdoc />
		public override void OnNewMatchStart()
		{
			Pool pool = Pools[Round];

			if (Match >= MatchCount && !pool.IsTied())
			{
				Round++;
				Match = 0;

				if (Round >= roundRange.Y)
				{
					OnStageFinish();
				}
			}

			StartNewRound();
			Match++;
		}

		/// <summary>
		///		Is called once a new round is started. 
		/// </summary>
		private void StartNewRound()
		{
			Pool pool = Pools[Round];
				
			Spawner.Clear();

			for (int i = 0; i < pool.Competitors.Count; i++)
			{
				Type tankBehaviour = pool.Competitors[i];
				Spawner.Spawn(tankBehaviour);
			}
		}

		/// <inheritdoc />
		public override void OnMatchFinish(Type winner)
		{
			Pool pool = Pools[Round];
			int i = pool.Competitors.LastIndexOf(winner);
			pool.Score[i]++;
			Spawner.Clear();
			Debug.LogFormat("Matches finished: {0} won!", winner.ToString());
		}

		/// <summary>
		///		Is called once a stage is finished.
		/// </summary>
		private void OnStageFinish()
		{
			// Adds the winners of the previous pools to new ones.
			Type[] winners = new Type[roundRange.Y - roundRange.X];
			for (int i = roundRange.X; i < roundRange.Y; i++)
			{
				Type winner = Pools[i].FetchWinner();
				winners[roundRange.Y - i - 1] = winner;
			}
			if (winners.Length == 1)
			{
				OnGameFinish.SafeInvoke(winners[0]);
			}
			else
			{
				EnrollCompetitors(winners);
				roundRange.X = roundRange.Y;
				roundRange.Y = Pools.Count;
			}
		}


		/// <summary>
		///		Enrolls the provided Types to one or more
		///		new Pools.
		/// </summary>
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
					pool.Add(current);
				}
				Pools.Add(pool);
			}
		}
	}
}
