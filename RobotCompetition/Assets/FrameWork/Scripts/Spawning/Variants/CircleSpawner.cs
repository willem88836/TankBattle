using System;
using UnityEngine;

namespace Framework
{
	public sealed class CircleSpawner : Spawner
	{
		public float Radius;
		private int count;

		private int behaviourIndex = 0;


		public override void Spawn(params Type[] behaviour)
		{
			behaviourIndex = 0;
			count = behaviour.Length;

			for (int i = 0; i < count; i++)
			{
				Spawn(behaviour[i]);
				behaviourIndex++;
			}
		}

		public override void Spawn(Type behaviour, int count)
		{
			behaviourIndex = 0;
			this.count = count;

			for (int i = 0; i < count; i++)
			{
				Spawn(behaviour);
				behaviourIndex++;
			}
		}

		public override void Spawn(Type behaviour)
		{
			float angle = Mathf.Deg2Rad * (behaviourIndex / (float)count) * 360f;
			Vector3 position = new Vector3(Mathf.Cos(angle), 0, -Mathf.Sin(angle));

			position *= Radius;

			SpawnAt(behaviour, position, Quaternion.Euler(0, (Mathf.Rad2Deg * angle) + 270, 0));
		}
	}
}
