using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework
{
	public abstract class CompetitionManager : MonoBehaviour
	{
		[NonSerialized] public List<Pool> Pools;
		public Action<MonoBehaviour> OnGameFinish;


		[Header("CompetitionManager Settings")]
		public string TankBehaviourPath = "Assets/AIscripts/";
		public Spawner Spawner;


		public abstract void Initialize();

		public abstract void Next();

		public abstract void OnPoolFinish(MonoBehaviour winner);

		/// <summary>
		///		Loads all existing tankbehaviours at the selected path.
		/// </summary>
		protected MonoBehaviour[] LoadBehaviours()
		{
			MonoBehaviour[] competitors = (AssetDatabase.LoadAllAssetsAtPath(TankBehaviourPath) as MonoBehaviour[]);
			Debug.LogFormat("{0} competitor behaviours loaded!", competitors.Length);
			return competitors;
		}
	}
}
