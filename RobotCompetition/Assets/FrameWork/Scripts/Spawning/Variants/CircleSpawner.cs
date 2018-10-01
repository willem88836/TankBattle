using System;
using UnityEngine;

namespace Framework
{
	public sealed class CircleSpawner : Spawner
	{
		public float Radius;
		private int count;

		private float interval = 0;
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
			float angle = (behaviourIndex / (float)count) * 360f;

			Vector2 position = new Vector2(
				Mathf.Cos(angle * Mathf.Rad2Deg),
				-Mathf.Sin(angle * Mathf.Rad2Deg));

			position *= Radius;

			SpawnAt(behaviour, position, Quaternion.Euler(-position));
		}
	}
}
