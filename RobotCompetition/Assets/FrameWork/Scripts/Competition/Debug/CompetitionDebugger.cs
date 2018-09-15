using System;
using UnityEngine;

namespace Framework
{
	public class CompetitionDebugger : MonoBehaviour
	{
		public CompetitionManager manager;

		private void Start()
		{
			manager.Initialize();

			manager.OnGameFinish += (Type t) => { manager.Initialize(); };
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.S))
				manager.Next();

			if (Input.GetKeyDown(KeyCode.F))
			{
				Pool pool = manager.Pools[((TournamentManager)manager).round];
				manager.OnMatchFinish(pool.Competitors[UnityEngine.Random.Range(0, pool.Competitors.Count)]);
			}

		}
	}
}
