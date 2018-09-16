using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
	/// <summary>
	///		Spawns and despawns objects with a desired behaviour.
	/// </summary>
	[Serializable]
	public class Spawner : MonoBehaviour
	{
		public GameObject BaseObject;
		public Transform Parent;
		public Transform[] SpawnLocations;

		private int currentSpawn = 0;

		private List<GameObject> spawnedObjects = new List<GameObject>();

		/// <summary>
		///		Destroys all spawned objects.
		/// </summary>
		public void Clear()
		{
			foreach (GameObject obj in spawnedObjects)
			{
				if (obj != null)
					Destroy(obj);
			}

			spawnedObjects.Clear();
		}

		/// <summary>
		///		Spawns multiple objects with the provided behaviour
		///		at multiple spawn locations.
		/// </summary>
		public void Spawn(Type behaviour, int count)
		{
			for (int i = 0; i < count; i++)
			{
				Spawn(behaviour);
			}
		}

		/// <summary>
		///		Spawns one object with the provided behaviour
		///		at one of the set spawnlocations.
		/// </summary>
		public void Spawn(Type behaviour)
		{
			Transform spawnPoint = SpawnLocations[currentSpawn];
			GameObject spawnedObject = Instantiate(
				BaseObject,
				spawnPoint.position,
				spawnPoint.rotation,
				Parent);

			// TODO: Update this line when Justin is finished with the new Tank Behaviours.
			spawnedObject.AddComponent(behaviour);

			spawnedObject.name = BaseObject.name + "_" + behaviour.ToString();

			spawnedObjects.Add(spawnedObject);

			currentSpawn++;
			currentSpawn %= SpawnLocations.Length;
		}
	}
}
