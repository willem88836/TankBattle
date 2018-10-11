using System;
using UnityEngine;

namespace Framework.Competition
{
	/// <summary>
	///		Base class for various competition controllers. 
	/// </summary>
	public abstract class CompetitionManager : BattleManager
	{
		public int CompetitorCount { get; protected set; }

		public Action<Type> OnGameFinish;
		public Action OnIntermission;


		[Header("CompetitionManager Settings")]
		public Spawner Spawner;

		protected override void Awake()
		{
			base.Awake();
			Initialize();
			Spawner.BaseObject = _tankPrefab;
		}

		public abstract void Initialize();

		/// <summary>
		///		Is called when a new match is supposed to start. 
		/// </summary>
		public abstract void OnNewMatchStart();

		/// <summary>
		///		Is called after a single match has been finished.
		///		The winner (Type) is passed through.
		/// </summary>
		public abstract void OnMatchFinish(Type winner);

		protected override Type[] LoadBehaviours()
		{
			Type[] competitors = base.LoadBehaviours();
			CompetitorCount = competitors.Length;
			return competitors;
		}

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
