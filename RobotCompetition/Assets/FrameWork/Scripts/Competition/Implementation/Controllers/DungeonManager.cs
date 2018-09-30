using System;
using UnityEngine;
using Framework.ScriptableObjects.Variables;

namespace Framework.Competition
{
	public sealed class DungeonManager : CompetitionManager
	{
		[Header("Dungeon Settings")]
		public Type ChallengerBehaviour;
		public Type[] Bosses;

		private Type challenger; 
		private int round;


		public override void Initialize()
		{
			round = 0;
			challenger = ChallengerBehaviour.GetType();

			Debug.Log("DungeonManager successfully Initialized");
		}

		public override void OnMatchFinish(Type winner)
		{
			Spawner.Clear();

			if (winner != challenger)
			{
				Type currentBoss = Bosses[round];
				OnGameFinish.SafeInvoke(currentBoss);
			}
			else
			{
				round++;

				if (round >= Bosses.Length)
				{
					OnGameFinish.SafeInvoke(challenger);
				}
				else
				{
					OnIntermission.SafeInvoke();
				}
			}
		}

		/// <inheritdoc />
		public override void OnNewMatchStart()
		{
			Spawner.Clear();

			Type currentBoss = Bosses[round].GetType();
			Spawner.Spawn(challenger, currentBoss);

			Debug.LogFormat("Boss Battle started with {0} and {1}", challenger.ToString(), currentBoss.ToString());
		}
	}
}
