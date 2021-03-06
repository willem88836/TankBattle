﻿using Framework.Core;
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

			ChallengerPanel.InitializeSelectionProcess(_behaviours);

			List<Type> bosses = new List<Type>(_behaviours);
			bosses = Utilities.Shuffle(bosses);
			Bosses = bosses.ToArray();

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
					OnIntermission.SafeInvoke(winner);
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

			Type currentBoss = Bosses[round];
			Spawner.Spawn(challenger, currentBoss);

			ApplyOnDestroy();

			Debug.LogFormat("Boss Battle started with {0} and {1}", challenger.ToString(), currentBoss.ToString());
		}

		protected override void OnTankDestroyed(Type destroyed)
		{
			Type winner = destroyed == challenger ? challenger : Bosses[round];
			OnMatchFinish(winner);
		}
	}
}
