using System;
using UnityEngine;
using UnityEditor;
using Framework.ScriptableObjects.Variables;

namespace Framework.Competition
{
	public sealed class DungeonManager : CompetitionManager
	{
		[Header("Dungeon Settings")]
		public MonoScript ChallengerBehaviour;
		public SharedMonoScriptList Bosses;

		private Type challenger; 
		private int round;


		public override void Initialize()
		{
			round = 0;
			challenger = ChallengerBehaviour.GetClass();
		}

		public override void OnMatchFinish(Type winner)
		{
			if (winner != challenger)
			{
				Type currentBoss = Bosses[round].GetType();
				OnGameFinish.SafeInvoke(currentBoss);
			}
			else
			{
				round++;

				if (round >= Bosses.Value.Count)
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

			Type currentBoss = Bosses[round].GetClass();
			Spawner.Spawn(challenger, currentBoss);
		}
	}
}
