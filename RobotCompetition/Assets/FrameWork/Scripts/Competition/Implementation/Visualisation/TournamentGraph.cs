using UnityEngine;

namespace Framework
{
	public class TournamentGraph : MonoBehaviour
	{
		public TournamentManager Manager;

		[Space]
		public GraphTile[] Tiles;

		public void Generate()
		{
			for(int i = 0; i < Manager.Pools.Count; i++)
			{
				Pool pool = Manager.Pools[i];
				GraphTile tile = Tiles[i];
				for (int j = 0; j < pool.Competitors.Count; j++)
				{
					tile.Add(pool.Competitors[j].ToString(), pool.Score[j]);
				}
			}
		}
	}
}
