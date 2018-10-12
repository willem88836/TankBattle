using Framework.Core;
using System;
using System.Collections.Generic;

namespace Framework.Competition
{
	public class FreeForAllManager : CompetitionManager
	{
		private List<Type> competitors = new List<Type>();

		public override void Initialize()
		{
			competitors = Utilities.ShuffleToList(_behaviours);
		}

		public override void OnMatchFinish(Type winner)
		{
			OnGameFinish.SafeInvoke(winner);
		}

		public override void OnNewMatchStart()
		{
			Spawner.Spawn(competitors);
		}

		protected override void OnTankDestroyed(Type destroyed)
		{
			competitors.Remove(destroyed);

			if (competitors.Count > 1)
				return;

			OnMatchFinish(competitors[0]);
		}
	}
}
