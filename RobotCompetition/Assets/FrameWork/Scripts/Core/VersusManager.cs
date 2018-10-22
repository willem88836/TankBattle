using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Framework.Core
{
	public sealed class VersusManager : BattleManager
	{
		[Header("BehaviourSelect")]
		[SerializeField] GameObject _behaviourButton;
		[SerializeField] Transform _leftBehaviourContainer;
		[SerializeField] Transform _rightBehaviourContainer;

		[Header("Spawn")]
		[SerializeField] Spawnpoints _spawnpoints;

		[Header("UI")]
		[SerializeField] CanvasGroup _behaviourSelectUI;
		[SerializeField] CanvasGroup _inMatchUI;
		[SerializeField] CanvasGroup _matchResultUI;
		[SerializeField] Transform _worldSpaceCanvas;

		[SerializeField] Text _defeatedTankText;
		[SerializeField] Toggle _sensorToggle;
		[SerializeField] Toggle _soundToggle;

		[SerializeField] Camera _sensorCamera;

		BehaviourButton _selectedLeft;
		BehaviourButton _selectedRight;

		Dictionary<Type, List<BehaviourButton>> _buttonsPerBehaviour;

		protected override void Awake()
		{
			base.Awake();

			SpawnBehaviourButtons(_behaviours);

			_sensorToggle.onValueChanged.AddListener(delegate { ToggleSensors(_sensorToggle); });
			ToggleSensors(_sensorToggle);

			_soundToggle.onValueChanged.AddListener(delegate { ToggleSound(_soundToggle); });
			ToggleSound(_soundToggle);

			SetInMatchUI(false);
			SetMatchResultUI(false);
			SetBehaviourSelectUI(true);
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

		void ToggleSensors(Toggle toggle)
		{
			_sensorCamera.enabled = toggle.isOn;
		}

		void ToggleSound(Toggle toggle)
		{
			_audioSource.mute = !toggle.isOn;
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

			TankMotor leftMotor = leftTank.GetComponent<TankMotor>();
			leftMotor.InitHealthUI(_worldSpaceCanvas);
			leftMotor.OnTankDestroyed += TankDestroyed;

			// Right
			Transform rightSpawn = _spawnpoints.GetNextSpawn();

			GameObject rightTank = Instantiate(_tankPrefab, _tankContainer);
			rightTank.transform.position = rightSpawn.position;
			rightTank.transform.rotation = rightSpawn.rotation;

			TankMotor rightMotor = rightTank.GetComponent<TankMotor>();
			rightMotor.InitHealthUI(_worldSpaceCanvas);
			rightMotor.OnTankDestroyed += TankDestroyed;

			// Add behaviours
			leftMotor.SetBehaviour(_selectedLeft.GetBehaviour());
			rightMotor.SetBehaviour(_selectedRight.GetBehaviour());

			// TODO: Disable UI
			SetBehaviourSelectUI(false);
			SetMatchResultUI(false);
			SetInMatchUI(true);
		}

		void TankDestroyed(Type behaviour)
		{
			_defeatedTankText.text = behaviour.ToString();

			EndMatch();
		}

		public void ManualStop()
		{
			_defeatedTankText.text = "Nobody";

			EndMatch();
		}

		void EndMatch()
		{
			ClearBullets();
			StripTankBehaviours();
			_spawnpoints.ResetSpawnIndex();

			StartCoroutine(EnableBehaviourSelectAfter(2.0f));
		}

		IEnumerator EnableBehaviourSelectAfter(float delay)
		{
			SetBehaviourSelectUI(false);
			SetInMatchUI(false);
			SetMatchResultUI(true);

			_matchResultUI.alpha = 0.0f;

			float timer = 0.0f;
			float fadeTime = 0.5f;

			while (timer < fadeTime)
			{
				yield return null;

				timer += Time.deltaTime;
				float percent = timer / fadeTime;
				_matchResultUI.alpha = percent;
			}

			yield return new WaitForSeconds(delay);

			timer = 0.0f;

			while (timer < fadeTime)
			{
				yield return null;

				timer += Time.deltaTime;
				float percent = timer / fadeTime;
				_matchResultUI.alpha = 1.0f - percent;
			}

			ClearTanks();

			SetInMatchUI(false);
			SetMatchResultUI(false);
			SetBehaviourSelectUI(true);
		}

		void SetBehaviourSelectUI(bool value)
		{
			if (value)
				_behaviourSelectUI.alpha = 1.0f;
			else
				_behaviourSelectUI.alpha = 0f;

			_behaviourSelectUI.blocksRaycasts = value;
		}

		void SetInMatchUI(bool value)
		{
			if (value)
				_inMatchUI.alpha = 1.0f;
			else
				_inMatchUI.alpha = 0f;

			_inMatchUI.interactable = value;
			_inMatchUI.blocksRaycasts = value;
		}

		void SetMatchResultUI(bool value)
		{
			if (value)
				_matchResultUI.alpha = 1.0f;
			else
				_matchResultUI.alpha = 0f;

			_matchResultUI.interactable = value;
			_matchResultUI.blocksRaycasts = value;
		}
	}
}
