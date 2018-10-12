using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
	/// <summary>
	///		Spawns and despawns objects with a desired behaviour.
	/// </summary>
	public abstract class Spawner : MonoBehaviour
	{
		public GameObject BaseObject;
		public Transform Parent;

		public List<GameObject> SpawnedObjects = new List<GameObject>();


		/// <summary>
		///		Destroys all spawned objects.
		/// </summary>
		public virtual void Clear()
		{
			foreach (GameObject obj in SpawnedObjects)
			{
				if (obj != null)
					Destroy(obj);
			}

			SpawnedObjects.Clear();
		}


		/// <summary>
		///		Spawns all the provided behaviours at multiple spawn locations.
		/// </summary>
		public virtual void Spawn(params Type[] behaviour)
		{
			for (int i = 0; i < behaviour.Length; i++)
			{
				Type current = behaviour[i];
				Spawn(current);
			}
		}

		/// <summary>
		///		Spawns all behaviours provided inside the list.
		/// </summary>
		public virtual void Spawn(List<Type> behaviour)
		{
			for (int i = 0; i < behaviour.Count; i++)
			{
				Type current = behaviour[i];
				Spawn(current);
			}
		}

		/// <summary>
		///		Spawns multiple objects with the provided behaviour
		///		at multiple spawn locations.
		/// </summary>
		public virtual void Spawn(Type behaviour, int count)
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
		public abstract void Spawn(Type behaviour);


		protected void SpawnAt(Type behaviour, Vector3 position, Quaternion rotation)
		{
			GameObject spawnedObject = Instantiate(BaseObject, position, rotation, Parent);
			spawnedObject.AddComponent(behaviour);
			spawnedObject.name = BaseObject.name + "_" + behaviour.ToString();

			SpawnedObjects.Add(spawnedObject);
		}
	}
}
