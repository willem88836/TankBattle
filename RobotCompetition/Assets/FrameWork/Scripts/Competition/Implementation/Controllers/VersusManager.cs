using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Framework.Competition
{
	public sealed class VersusManager : BattleManager
	{
		[Header("BehaviourSelect")]
		[SerializeField] GameObject _behaviourButton;
		[SerializeField] Transform _leftBehaviourContainer;
		[SerializeField] Transform _rightBehaviourContainer;

		[Header("Spawn")]
		[SerializeField] Spawnpoints _spawnpoints;

		BehaviourButton _selectedLeft;
		BehaviourButton _selectedRight;

		Dictionary<Type, List<BehaviourButton>> _buttonsPerBehaviour;

		protected override void Awake()
		{
			base.Awake();

			SpawnBehaviourButtons(_behaviours);
		}

		void SpawnBehaviourButtons(Type[] behaviours)
		{
			_buttonsPerBehaviour = new Dictionary<Type, List<BehaviourButton>>();

			for (int i = 0; i < _behaviours.Length; i++)
			{
				Type current = _behaviours[i];

				// Left
				BehaviourButton leftButton = Instantiate(_behaviourButton, _leftBehaviourContainer).GetComponent<BehaviourButton>();
				leftButton.SetBehaviour(current);
				leftButton.GetOnClick().AddListener(delegate { SelectedLeft(leftButton); });

				// Right
				BehaviourButton rightButton = Instantiate(_behaviourButton, _rightBehaviourContainer).GetComponent<BehaviourButton>();
				rightButton.SetBehaviour(current);
				rightButton.GetOnClick().AddListener(delegate { SelectedRight(rightButton); });

				List<BehaviourButton> buttons = new List<BehaviourButton> { leftButton, rightButton };

				_buttonsPerBehaviour.Add(current, buttons);
			}

			// Automatically select the first buttons
			if (_buttonsPerBehaviour.Count <= 0)
				return;

			List<Type> keyList = new List<Type>(_buttonsPerBehaviour.Keys);
			List<BehaviourButton> firstButtons = _buttonsPerBehaviour[keyList[0]];
			for (int i = 0; i < firstButtons.Count; i++)
			{
				Button.ButtonClickedEvent onClick = firstButtons[i].GetOnClick();

				if (onClick != null)
					onClick.Invoke();
			}
		}

		void SelectedLeft(BehaviourButton button)
		{
			if (_selectedLeft != null)
				_selectedLeft.OnDeselected();

			_selectedLeft = button;
			button.OnSelected();
		}

		void SelectedRight(BehaviourButton button)
		{
			if (_selectedRight != null)
				_selectedRight.OnDeselected();

			_selectedRight = button;
			button.OnSelected();
		}

		public void StartMatch()
		{
			if (_selectedLeft == null || _selectedRight == null)
				return;

			// Left
			Transform leftSpawn = _spawnpoints.GetNextSpawn();

			GameObject leftTank = Instantiate(_tankPrefab, _tankContainer);
			leftTank.transform.position = leftSpawn.position;
			leftTank.transform.rotation = leftSpawn.rotation;

			// Right
			Transform rightSpawn = _spawnpoints.GetNextSpawn();

			GameObject rightTank = Instantiate(_tankPrefab, _tankContainer);
			rightTank.transform.position = rightSpawn.position;
			rightTank.transform.rotation = rightSpawn.rotation;

			// Add behaviours
			leftTank.AddComponent(_selectedLeft.GetBehaviour());
			rightTank.AddComponent(_selectedRight.GetBehaviour());

			// TODO: Disable UI
		}

		// TODO: Action on tank destruction to end the match

		void ClearBullets()
		{
			foreach (Transform bullet in _bulletContainer)
			{
				Destroy(bullet.gameObject);
			}
		}

		void StripTankBehaviours()
		{
			foreach (Transform tank in _tankContainer)
			{
				Destroy(tank.GetComponent<RobotControl>());
			}
		}

		void ClearTanks()
		{
			foreach (Transform tank in _tankContainer)
			{
				Destroy(tank.gameObject);
			}
		}
	}
}
