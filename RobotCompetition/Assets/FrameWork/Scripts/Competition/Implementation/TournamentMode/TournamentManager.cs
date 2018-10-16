using Framework.Core;
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
		[Range(1, 20)] public int PoolSize;
		[Range(1, 30)] public int MatchCount;

		[NonSerialized] public List<Pool> Pools = new List<Pool>();

		public int Round { get; private set; }
		public int Match { get; private set;}

		// Marks what pools are within this round. 
		private Int2 roundRange;


		/// <inheritdoc />
		public override void Initialize()
		{
			if (Pools == null)
				Pools = new List<Pool>();
			else
				Pools.Clear();


			Round = 0;
			Match = 0;


			Type[] competitors = _behaviours;
			competitors = Utilities.Shuffle(competitors);


			EnrollNewCompetitors(competitors);
		}

		/// <inheritdoc />
		public override void OnMatchFinish(Type winner)
		{
			Pool current = Pools[Round];
			current.CompetitorAs(winner).Score++;
			Spawner.Clear();
			OnIntermission.SafeInvoke();
		}

		/// <inheritdoc />
		public override void OnNewMatchStart()
		{
			Pool current = Pools[Round];

			// When round is finished.
			if (Match >= MatchCount && !current.IsTied())
			{
				Round++;
				Match = 0;

				if (Round >= roundRange.Y)
				{
					OnStageFinished();
				}
			}

			StartNewRound();
			Match++;
		}

		/// <inheritdoc />
		protected override void OnTankDestroyed(Type destroyed)
		{
			Pool current = Pools[Round];
			current.CompetitorAs(destroyed).IsDefeated = true;

			int destroyedCount = 0;
			for (int i = 0; i < current.Count; i++)
			{
				if (current.CompetitorAt(i).IsDefeated)
					destroyedCount++;
			}

			if (destroyedCount >= current.Count - 1)
			{
				current.SortToScore();
				Type winner = current.CompetitorAt(0).Type;
				OnMatchFinish(winner);
			}
		}


		private void StartNewRound()
		{
			Spawner.Clear();

			Pool pool = Pools[Round];

			for (int i = 0; i < pool.Count; i++)
			{
				Spawner.Spawn(pool.CompetitorAt(i).Type);
			}
		}

		/// <summary>
		///		Makes sure all winners of the current stage
		///		are added to the next stage.
		/// </summary>
		private void OnStageFinished()
		{
			Type[] winners = new Type[roundRange.Y - roundRange.X];
			for (int i = roundRange.X; i < roundRange.Y; i++)
			{
				Pool current = Pools[i];
				current.SortToScore();
				winners[i] = current.CompetitorAt(0).Type;
			}

			// If this is the last match.
			if (winners.Length == 1)
			{
				OnGameFinish.SafeInvoke(winners[0]);
			}
			else
			{
				EnrollNewCompetitors(winners);
				roundRange.X = roundRange.Y;
				roundRange.Y = Pools.Count;
			}
		}

		/// <summary>
		///		Enrolls the provided Types to one or more
		///		new Pools.
		/// </summary>
		private void EnrollNewCompetitors(Type[] newCompetitors)
		{
			int newPoolCount = Mathf.CeilToInt(newCompetitors.Length / PoolSize);
			int oldPoolCount = Pools.Count;

			for (int i = 0; i < newCompetitors.Length; i++)
			{
				Pool currentPool;
				Type currentCompetitor = newCompetitors[i];

				int index = i % newPoolCount + oldPoolCount;
				if (Pools.Count <= index)
				{
					Pools.Add(new Pool());
				}
				currentPool = Pools[index];

				currentPool.Add(currentCompetitor);
			}
		}
	}
}
