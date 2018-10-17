using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
	public class Spawnpoints : MonoBehaviour
	{
		[SerializeField] bool _allowDoubleSpawn = false;

		Transform[] _spawnpoints;
		int _spawnIndex = 0;

		private void Awake()
		{
			_spawnpoints = ReferenceSpawnpoints(transform); // Could be seperate object later
		}

		Transform[] ReferenceSpawnpoints(Transform parent)
		{
			List<Transform> spawnpoints = new List<Transform>();

			foreach(Transform child in parent)
			{
				spawnpoints.Add(child);
			}

			return spawnpoints.ToArray();
		}

		public Transform GetNextSpawn()
		{
			if (_spawnIndex >= _spawnpoints.Length)
			{
				ResetSpawnIndex();

				if (!_allowDoubleSpawn)
					Debug.LogWarning("A spawnpoint has been used twice");
			}

			Transform currentSpawn = _spawnpoints[_spawnIndex];
			_spawnIndex++;

			return currentSpawn;
		}

		// TODO: GetRandomSpawn(), also keep track of double spawns

		public void ResetSpawnIndex()
		{
			_spawnIndex = 0;
		}
	}
}
