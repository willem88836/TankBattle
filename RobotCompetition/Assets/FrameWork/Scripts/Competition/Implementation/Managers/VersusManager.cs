using System;
using UnityEngine;

namespace Framework.Competition
{
	public sealed class VersusManager : CompetitionManager
	{
		[Header("VersusManager Settings")]
		[SerializeField] private SelectionPanel[] selectionPanels;

		/// <inheritdoc />
		public override void Initialize()
		{
			Type[] competitors = LoadBehaviours();

			foreach (SelectionPanel panel in selectionPanels)
			{
				panel.InitializeSelectionProcess(competitors);
			}

			Debug.Log("VersusManager successfully Initialized");
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

			Type[] selection = FetchSelection();
			Spawner.Spawn(selection);
		}

		private Type[] FetchSelection()
		{
			Type[] selection = new Type[selectionPanels.Length];

			for(int i = 0; i < selectionPanels.Length; i++)
			{
				SelectionPanel current = selectionPanels[i];
				selection[i] = current.Selection;
			}

			return selection;
		}
	}
}
