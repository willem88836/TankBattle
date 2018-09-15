using System;
using UnityEngine;

namespace Framework
{
	[Serializable]
	public class Spawner : MonoBehaviour
	{
		public GameObject BaseObject;
		public Transform Parent;
		public Transform[] SpawnLocations;

		private int currentSpawn = 0;


		public void Spawn(MonoBehaviour behaviour, int count)
		{
			for (int i = 0; i < count; i++)
			{
				Spawn(behaviour);
			}
		}

		public void Spawn(MonoBehaviour behaviour)
		{
			Transform spawnPoint = SpawnLocations[currentSpawn];
			GameObject spawnedObject = Instantiate(
				BaseObject,
				spawnPoint.position,
				spawnPoint.rotation,
				Parent);

			spawnedObject.AddComponent(behaviour.GetType());

			currentSpawn++;
			currentSpawn %= SpawnLocations.Length;
		}
	}
}
