using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Framework.Competition
{
	/// <summary>
	///		Base class for various competition controllers. 
	/// </summary>
	public abstract class CompetitionManager : BattleManager
	{
		public int CompetitorCount { get; protected set; }

		[NonSerialized] public List<Pool> Pools;
		public Action<Type> OnGameFinish;
		public Action OnIntermission;


		[Header("CompetitionManager Settings")]
		public string TankBehaviourPath = "AIscripts/";
		public Spawner Spawner;

		protected override void Awake()
		{
			base.Awake();
			Initialize();
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

		/// <summary>
		///		Loads all existing tankbehaviours at the selected path.
		/// </summary>
		protected Type[] LoadBehaviours()
		{
			Type baseType = typeof(RobotControl);
			Assembly assembly = Assembly.GetAssembly(baseType);
			Type[] competitors = (assembly.GetTypes().Where(t => t != baseType && baseType.IsAssignableFrom(t))).ToArray();

			CompetitorCount = competitors.Length;
			Debug.LogFormat("{0} competitor behaviours loaded!", competitors.Length);
			return competitors;
		}
	}
}
