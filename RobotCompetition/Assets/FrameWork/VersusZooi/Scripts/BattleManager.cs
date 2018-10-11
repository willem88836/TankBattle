using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Framework
{
	[RequireComponent(typeof(AudioSource))]
	public class BattleManager : MonoBehaviour
	{
		static BattleManager _instance;
	
		[Header("General")]
		[SerializeField] int _targetFramerate = 60;
		[SerializeField] string _behaviourPath;

		[Header("Prefabs")]
		[SerializeField] protected GameObject _tankPrefab;

		[Header("Containers")]
		[SerializeField] protected Transform _bulletContainer;
		[SerializeField] protected Transform _tankContainer;

		[Header("Behaviours")]
		protected Type[] _behaviours;

		protected AudioSource _audioSource;
		
		protected virtual void Awake()
		{
			if (_instance != null && _instance != this)
				Destroy(_instance.gameObject);

			_instance = this;

			Application.targetFrameRate = _targetFramerate;

			_audioSource = GetComponent<AudioSource>();

			_behaviours = LoadBehaviours();
		}

		// TODO: Only works in editor
		protected Type[] LoadBehaviours()
		{
			MonoScript[] assets = Resources.LoadAll<MonoScript>(_behaviourPath);

			Type[] behaviours = new Type[assets.Length];
			for (int i = 0; i < assets.Length; i++)
			{
				behaviours[i] = assets[i].GetClass();
			}

			Debug.LogFormat("{0} Behaviours found", behaviours.Length);
			return behaviours;
		}

		protected void ClearBullets()
		{
			foreach (Transform bullet in _bulletContainer)
			{
				Destroy(bullet.gameObject);
			}
		}

		protected void StripTankBehaviours()
		{
			foreach (Transform tank in _tankContainer)
			{
				TankMotor motor = tank.GetComponent<TankMotor>();

				if (motor != null)
					motor.StopTank();

				Destroy(tank.GetComponent<RobotControl>());
			}
		}

		protected void ClearTanks()
		{
			foreach (Transform tank in _tankContainer)
			{
				Destroy(tank.gameObject);
			}
		}

		public Transform GetBulletContainer()
		{
			return _bulletContainer;
		}

		public Transform GetTankContainer()
		{
			return _tankContainer;
		}

		public void PlaySound(AudioClip clip)
		{
			_audioSource.PlayOneShot(clip);
		}

		public static BattleManager Singleton()
		{
			if (_instance == null)
				Debug.LogWarning("BattleManager singleton returned null");
	
			return _instance;
		}
	}
}
