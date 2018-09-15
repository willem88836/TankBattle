using System;
using System.Collections.Generic;
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

		private List<GameObject> spawnedObjects = new List<GameObject>();

		public void Clear()
		{
			foreach (GameObject obj in spawnedObjects)
			{
				if (obj != null)
					Destroy(obj);
			}

			spawnedObjects.Clear();
		}

		public void Spawn(Type behaviour, int count)
		{
			for (int i = 0; i < count; i++)
			{
				Spawn(behaviour);
			}
		}

		public void Spawn(Type behaviour)
		{
			Transform spawnPoint = SpawnLocations[currentSpawn];
			GameObject spawnedObject = Instantiate(
				BaseObject,
				spawnPoint.position,
				spawnPoint.rotation,
				Parent);

			spawnedObject.AddComponent(behaviour);

			spawnedObjects.Add(spawnedObject);

			currentSpawn++;
			currentSpawn %= SpawnLocations.Length;
		}
	}
}
