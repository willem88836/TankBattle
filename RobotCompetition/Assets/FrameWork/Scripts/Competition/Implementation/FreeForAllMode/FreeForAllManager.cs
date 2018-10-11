using Framework.Core;
using System;

namespace Framework.Competition
{
	public class FreeForAllManager : CompetitionManager
	{
		private Type[] competitors;

		public override void Initialize()
		{
			competitors = LoadBehaviours();
			competitors = Utilities.Shuffle(competitors);
		}

		public override void OnMatchFinish(Type winner)
		{
			OnGameFinish.SafeInvoke(winner);
		}

		public override void OnNewMatchStart()
		{
			Spawner.Spawn(competitors);
		}
	}
}
