using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Competition
{
	/// <summary>
	///		Base class for various competition controllers. 
	/// </summary>
	public abstract class CompetitionManager : BattleManager
	{
		public int CompetitorCount { get { return _behaviours != null ? _behaviours.Length : 0; } }

		public Action<Type> OnGameFinish;
		public Action OnIntermission;


		[Header("CompetitionManager Settings")]
		public Spawner Spawner;

		protected override void Awake()
		{
			base.Awake();

			Spawner.BaseObject = _tankPrefab;
			Spawner.Parent = _tankContainer;

			Initialize();
			ApplyOnDestroy();
		}

		public abstract void Initialize();

		/// <summary>
		///		Adds delegate OnTankDestroyed(Type) to each TankMotor.
		/// </summary>
		protected void ApplyOnDestroy()
		{
			List<GameObject> tanks = Spawner.SpawnedObjects;

			if (tanks == null)
				return;

			foreach (GameObject tank in tanks)
			{
				TankMotor motor = tank.GetComponent<TankMotor>();
				Action<Type> onDestroyed = (Type t) => { OnTankDestroyed(t); };
				motor.OnTankDestroyed -= onDestroyed;
				motor.OnTankDestroyed += onDestroyed;
			}
		}


		/// <summary>
		///		Is called when a new match is supposed to start. 
		/// </summary>
		public abstract void OnNewMatchStart();

		/// <summary>
		///		Is called after a single match has been finished.
		///		The winner (Type) is passed through.
		/// </summary>
		public abstract void OnMatchFinish(Type winner);

		/// <summary>
		///		Is called every time a tank is destroyed.
		/// </summary>
		protected abstract void OnTankDestroyed(Type destoyed);

#if UNITY_EDITOR
		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.F))
			{
				Debug.LogWarning("Force-Finishing Match");
				OnMatchFinish(null);
			}
		}
#endif
	}
}
