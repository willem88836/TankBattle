using System;
using UnityEngine;

namespace Framework.Competition
{
	public class SelectionPanel : MonoBehaviour
	{
		public SelectionButton SelectionButtonPrefab;
		public Transform Parent;
		public Type Selection { get; private set; }

		private SelectionButton[] selectionButtons;


		/// <summary>
		///		Spawns all buttons for selection and sets default selection.
		/// </summary>
		public void InitializeSelectionProcess(Type[] behaviours)
		{
			selectionButtons = new SelectionButton[behaviours.Length];

			// default selection is set.
			Selection = behaviours[0] ?? null;

			// Buttons are spawned and initialized.
			for (int j = 0; j < behaviours.Length; j++)
			{
				Type t = behaviours[j];

				SelectionButton button = Instantiate(SelectionButtonPrefab, Parent);
				button.NameBox.text = t.ToString();
				button.Behaviour = t;

				button.OnClickAction += () =>
				{
					SelectBehaviour(button);
				};

				selectionButtons[j] = button;

				if (j == 0)
				{
					button.Select();
				}
			}
		}

		/// <summary>
		///		Updates all buttons according to selected button.
		///		Stores reference to the selected behaviour;
		/// </summary>
		private void SelectBehaviour(SelectionButton button)
		{
			int boxIndex = button.BoxIndex;
			Type behaviour = button.Behaviour;

			Selection = behaviour;

			for (int i = 0; i < selectionButtons.Length; i++)
			{
				SelectionButton current = selectionButtons[i];
				current.Deselect();
			}

			button.Select();

			Debug.LogFormat("Selection {0} is now of type {1}", boxIndex, behaviour.ToString());
		}
	}
}
