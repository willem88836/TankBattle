using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework.Competition
{
	/// <summary>
	///		Base class for various competition controllers. 
	/// </summary>
	public abstract class CompetitionManager : MonoBehaviour
	{
		public int CompetitorCount { get; private set; }

		[NonSerialized] public List<Pool> Pools;
		public Action<Type> OnGameFinish;


		[Header("CompetitionManager Settings")]
		public string TankBehaviourPath = "AIscripts/";
		public Spawner Spawner;


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
			MonoScript[] assets = Resources.LoadAll<MonoScript>(TankBehaviourPath);

			Type[] competitors = new Type[assets.Length];
			for(int i = 0; i < assets.Length; i++)
			{
				competitors[i] = assets[i].GetClass();
			}

			CompetitorCount = competitors.Length;
			Debug.LogFormat("{0} competitor behaviours loaded!", competitors.Length);
			return competitors;
		}
	}
}
