using System.Linq;
using System.Reflection;
using UnityEngine;
using System;

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
			Type baseType = typeof(TankController);
			Assembly assembly = Assembly.GetAssembly(baseType);
			Type[] competitors = (assembly.GetTypes().Where(t =>
			t != baseType
			&& baseType.IsAssignableFrom(t)
			&& !typeof(IDoNotLoad).IsAssignableFrom(t))).ToArray();

			// This is for debugging purposes.
			Type[] multipliedCompetitors = new Type[competitors.Length * _behaviourEntries];
			for (int i = 0; i < multipliedCompetitors.Length; i++)
			{
				multipliedCompetitors[i] = competitors[i % competitors.Length];
			}

			Debug.LogFormat("{0} competitor behaviours loaded!", multipliedCompetitors.Length);
			return multipliedCompetitors;
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
