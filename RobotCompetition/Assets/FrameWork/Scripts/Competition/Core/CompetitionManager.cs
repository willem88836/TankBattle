﻿using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework
{
	public abstract class CompetitionManager : MonoBehaviour
	{
		[NonSerialized] public List<Pool> Pools;
		public Action<Type> OnGameFinish;


		[Header("CompetitionManager Settings")]
		public string TankBehaviourPath = "AIscripts/";
		public Spawner Spawner;


		public abstract void Initialize();

		public abstract void Next();

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

			Debug.LogFormat("{0} competitor behaviours loaded!", competitors.Length);
			return competitors;
		}
	}
}
