using Framework.Core;
using System;

namespace Framework.Competition
{
	public class FreeForAllManager : CompetitionManager
	{
		private Type[] competitors;

		public override void Initialize()
		{
			competitors = Utilities.Shuffle(_behaviours);
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
