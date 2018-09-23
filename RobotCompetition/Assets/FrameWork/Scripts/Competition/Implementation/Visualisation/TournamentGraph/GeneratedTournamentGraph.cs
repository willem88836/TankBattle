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

			int stageCount = 0;
			int competitorCount = Manager.CompetitorCount;

			while (competitorCount > 1)
			{
				stageCount++;
				competitorCount /= Manager.PoolSize;
				if (competitorCount < 1)
					competitorCount = 1;

				Debug.Log(stageCount + "  " +	competitorCount);
				
				GameObject stageObject = Instantiate(StageObject, Parent);
				stageObject.name = "StageObject_" + stageCount;
				stages.Add(StageObject);

				for (int i = 0; i < competitorCount; i++)
				{
					GameObject tile = Instantiate(BaseTile, stageObject.transform);
				 	Tiles.Add(tile);
				}
			}

			base.Generate();
		}
	}
}
