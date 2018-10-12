using Framework.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Competition
{
	public sealed class VersusManager : CompetitionManager
	{
		[Header("VersusManager Settings")]
		public SelectionPanel[] selectionPanels;

		private List<Type> competitors = new List<Type>();


		/// <inheritdoc />
		public override void Initialize()
		{
			foreach (SelectionPanel panel in selectionPanels)
			{
				panel.InitializeSelectionProcess(_behaviours);
			}
		}

		/// <inheritdoc />
		public override void OnMatchFinish(Type winner)
		{
			Spawner.Clear();
			OnGameFinish.SafeInvoke(winner);
		}

		/// <inheritdoc />
		public override void OnNewMatchStart()
		{
			Spawner.Clear();

			competitors = FetchSelection();
			Spawner.Spawn(competitors);
		}

		/// <inheritdoc />
		protected override void OnTankDestroyed(Type destroyed)
		{
			competitors.Remove(destroyed);

			if (competitors.Count > 1)
				return;

			OnMatchFinish(competitors[0]);
		}


		private List<Type> FetchSelection()
		{
			List<Type> selection = new List<Type>();

			for(int i = 0; i < selectionPanels.Length; i++)
			{
				SelectionPanel current = selectionPanels[i];
				selection.Add(current.Selection);
			}

			return selection;
		}
	}
}
