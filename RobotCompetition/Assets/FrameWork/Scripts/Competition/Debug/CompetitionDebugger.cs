using System;
using UnityEngine;

namespace Framework.Competition
{
	/// <summary>
	///		Debugging stuff.
	/// </summary>
	public class CompetitionDebugger : MonoBehaviour
	{
		public TournamentManager tournamentManager;
		public DungeonManager dungeonManager;
		public TournamentGraph graph;

		private void Start()
		{
			tournamentManager.Initialize();


			tournamentManager.OnGameFinish += (Type t) =>
			{
				Debug.Log("Game Finished! Competition won by: " + t.ToString());
				tournamentManager.Initialize();
			};


			dungeonManager.OnGameFinish += (Type t) =>
			{
				Debug.Log("Game Finished! Competition won by: " + t.ToString());
				dungeonManager.Initialize();
			};
		}



		public void ForceRandomTournamentWinner()
		{
			Pool pool = tournamentManager.Pools[tournamentManager.Round];
			tournamentManager.OnMatchFinish(pool.Competitors[UnityEngine.Random.Range(0, pool.Competitors.Count)]);
		}

		public void ForceRandomDungeonWinner()
		{
			dungeonManager.OnMatchFinish(dungeonManager.ChallengerBehaviour.GetClass());
		}
	}

}
