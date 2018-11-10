using UnityEngine;
using System;
using Framework.ScriptableObjects.Variables;

namespace Framework.Core
{
	[RequireComponent(typeof(AudioSource))]
	public class BattleManager : MonoBehaviour
	{
		static BattleManager _instance;
	
		[Header("General")]
		[SerializeField] int _targetFramerate = 60;
		[SerializeField, Range(1, 20)] int _behaviourEntries = 1;
		[SerializeField] bool _infiniteHealth = false;
		[SerializeField] StringReference _behaviourPath;

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

		/// <summary>
		///		Loads all existing tankbehaviours at the selected path.
		/// </summary>
		protected Type[] LoadBehaviours()
		{
			return TankBehaviourLoader.LoadAllBehaviours(_behaviourPath.Value, _behaviourEntries);
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

				Destroy(tank.GetComponent<TankController>());
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

		public bool InfiniteHealth()
		{
			return _infiniteHealth;
		}

		public static BattleManager Singleton()
		{
			if (_instance == null)
				Debug.LogWarning("BattleManager singleton returned null");
	
			return _instance;
		}
	}
}
