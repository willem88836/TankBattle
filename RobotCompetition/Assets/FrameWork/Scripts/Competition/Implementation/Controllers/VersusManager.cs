using System;
using UnityEngine;

namespace Framework.Competition
{
	public sealed class VersusManager : CompetitionManager
	{
		[Header("VersusManager Settings")]
		[SerializeField] private Transform[] selectionBoxes;
		[SerializeField] private SelectionButton selectionButtonPrefab;

		private SelectionButton[,] selectionButtons;

		private Type[] selection;


		/// <inheritdoc />
		public override void Initialize()
		{
			Type[] competitors = LoadBehaviours();
			InitializeSelectionProcess(competitors);

			Debug.Log("VersusManager successfully Initialized");
		}

		/// <summary>
		///		Spawns all buttons for selection and sets default selection.
		/// </summary>
		private void InitializeSelectionProcess(Type[] behaviours)
		{
			selectionButtons = new SelectionButton[selectionBoxes.Length, behaviours.Length];
			selection = new Type[selectionBoxes.Length];

			for (int i = 0; i < selectionBoxes.Length; i++)
			{
				// default selection is set.
				selection[i] = behaviours[0] ?? null;

				// Buttons are spawned and initialized.
				Transform parent = selectionBoxes[i];
				for(int j = 0; j < behaviours.Length; j++)
				{
					Type t = behaviours[j];

					SelectionButton button = Instantiate(selectionButtonPrefab, parent);
					button.NameBox.text = t.ToString();
					button.BoxIndex = i;
					button.Behaviour = t;

					button.OnClickAction += () => 
					{
						SelectBehaviour(button.BoxIndex, button.Behaviour, button);
					};

					selectionButtons[i, j] = button;
				}
			}
		}

		/// <summary>
		///		Updates all buttons according to selected button.
		///		Stores reference to the selected behaviour;
		/// </summary>
		private void SelectBehaviour(int boxIndex, Type behaviour, SelectionButton button)
		{
			selection[boxIndex] = behaviour;

			for (int i = 0; i < selectionButtons.GetLength(1); i++)
			{
				SelectionButton current = selectionButtons[boxIndex, i];
				current.Deselect();
			}

			button.Select();

			Debug.LogFormat("Selection {0} is now of type {1}", boxIndex, behaviour.ToString());
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
			Spawner.Spawn(selection);
		}
	}
}
