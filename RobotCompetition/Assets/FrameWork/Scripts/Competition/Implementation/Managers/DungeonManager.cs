using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Competition
{
	public sealed class DungeonManager : CompetitionManager
	{
		[Header("Dungeon Settings")]
		public SelectionPanel ChallengerPanel;

		private Type[] Bosses;
		private Type challenger; 
		private int round;


		public override void Initialize()
		{
			round = 0;

			Type[] competitors = LoadBehaviours();
			ChallengerPanel.InitializeSelectionProcess(competitors);


			List<Type> bosses = new List<Type>(competitors);
			bosses = Utilities.Shuffle(bosses);

			Debug.Log("DungeonManager successfully Initialized");
		}

		public override void OnMatchFinish(Type winner)
		{
			Spawner.Clear();

			if (winner != challenger)
			{
				Type currentBoss = Bosses[round];
				OnGameFinish.SafeInvoke(currentBoss);
				challenger = null;
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
			if (challenger == null)
			{
				challenger = ChallengerPanel.Selection;
			}

			Spawner.Clear();

			Type currentBoss = Bosses[round].GetType();
			Spawner.Spawn(challenger, currentBoss);

			Debug.LogFormat("Boss Battle started with {0} and {1}", challenger.ToString(), currentBoss.ToString());
		}
	}
}
