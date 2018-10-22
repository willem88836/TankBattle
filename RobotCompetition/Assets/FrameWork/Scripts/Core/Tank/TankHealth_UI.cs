using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.Core
{
	public class TankHealth_UI : MonoBehaviour
	{
		[HideInInspector] public Transform tank;

		public Color[] _healthColors;

		Image _healthBar;

		// healthbarcolor calculations
		float _halfHealth = 0.5f;
		float _colorMultiplier = 2f;

		void Start()
		{
			_healthBar = GetComponent<Image>();
			_healthBar.color = _healthColors[2];
		}

		// update the position of the tank
		void Update()
		{
			if (tank == null)
				return;

			transform.position = tank.position + Vector3.up * 0.1f;
		}

		public void Damage(float amount)
		{
			// the image goes from 0 to 1, so devide by 100 first
			float fixedAmount = amount / 100;

			_healthBar.fillAmount -= fixedAmount;
			SetHealthColor(_healthBar.fillAmount);
		}

		void SetHealthColor(float amount)
		{
			if (amount >= _halfHealth)
			{
				float fixedAmount = (amount - _halfHealth) * _colorMultiplier;
				_healthBar.color = Color.Lerp(_healthColors[1], _healthColors[2], fixedAmount);
			}
			else
			{
				float fixedAmount = _halfHealth * _colorMultiplier;
				_healthBar.color = Color.Lerp(_healthColors[0], _healthColors[1], fixedAmount);
			}
		}
	}
}

