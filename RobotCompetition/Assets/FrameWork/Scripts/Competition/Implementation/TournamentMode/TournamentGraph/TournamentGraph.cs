using System.Collections.Generic;
using UnityEngine;

namespace Framework.Competition.Graph
{
	/// <summary>
	///		Displays the current tournament data using 
	///		the manually set GraphTiles.
	/// </summary>
	public class TournamentGraph : MonoBehaviour
	{
		public TournamentManager Manager;
		public List<GameObject> Tiles;


		public virtual void Generate()
		{
			int shownPools = Mathf.Min(Manager.Pools.Count, Tiles.Count);

			for(int i = 0; i < shownPools; i++)
			{
				Pool pool = Manager.Pools[i];
				GraphTile tile = Tiles[i].GetComponent<GraphTile>();
				for (int j = 0; j < pool.Count; j++)
				{
					Competitor competitor = pool.CompetitorAt(j);
					tile.Add(competitor.Type.ToString(), competitor.Score);
				}
			}

			for (int i = shownPools; i < Tiles.Count; i++)
			{
				GraphTile tile = Tiles[i].GetComponent<GraphTile>();
				for (int j = 0; j < Manager.PoolSize; j++)
				{
					tile.Add("???", 0);
				}
			}
		}
	}
}
