using System;
using UnityEngine;

namespace Framework.Spawning
{
	public sealed class StaticSpawner : TankSpawner
	{
		public Transform[] SpawnLocations;

		private int currentSpawn = 0;


		public override void Spawn(Type behaviour)
		{
			Transform spawnPoint = SpawnLocations[currentSpawn];

			SpawnAt(behaviour, spawnPoint.position, spawnPoint.rotation);

			currentSpawn++;
			currentSpawn %= SpawnLocations.Length;
		}
	}
}
