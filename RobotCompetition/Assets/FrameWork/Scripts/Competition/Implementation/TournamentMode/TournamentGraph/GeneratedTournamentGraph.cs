using Framework.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Competition
{
	/// <summary>
	///		Generates a set of tiles, used to display 
	///		the current Tournament data.
	/// </summary>
	public class GeneratedTournamentGraph : TournamentGraph
	{
		public Transform Parent;
		public GameObject StageObject;
		public GameObject BaseTile;


		private List<GameObject> stages = new List<GameObject>();

		/// <summary>
		///		Destroys all current Graph objects. 
		/// </summary>
		public void Clear()
		{
			Parent.ReversedForeach((Transform child) => { Destroy(child.gameObject); });
		}

		/// <summary>
		///		Generates and fills a new Tournament Graph
		///		based on the current Tournament data.
		/// </summary>
		public override void Generate()
		{
			Tiles.Clear();
			Clear();

			int playerCount = Manager.CompetitorCount;
			int brackets = playerCount / Manager.PoolSize;

			for (int i = 0; i < brackets && playerCount > 1; i++)
			{
				GameObject stageObject = Instantiate(StageObject, Parent);
				stageObject.name = string.Format("Bracket_{0}", i);
				stages.Add(StageObject);

				int pools = playerCount / Manager.PoolSize;
				pools = Mathf.Clamp(pools, 1, int.MaxValue);

				for (int j = 0; j < pools; j++)
				{
					Transform parent = stageObject.transform;
					GameObject tile = Instantiate(BaseTile, parent);
					tile.name = string.Format("Pool_{0}_{1}", i, j);
					Tiles.Add(tile);
				}

				playerCount = pools;
			}

			base.Generate();
		}
	}
}
