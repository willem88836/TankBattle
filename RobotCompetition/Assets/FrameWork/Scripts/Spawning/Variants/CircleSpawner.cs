using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Spawning
{
	public sealed class CircleSpawner : TankSpawner
	{
		public float Radius;
		private int count;

		private int behaviourIndex = 0;


		public override void Spawn(params Type[] behaviour)
		{
			behaviourIndex = 0;
			this.count = behaviour.Length;

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

		public override void Spawn(List<Type> behaviour)
		{
			behaviourIndex = 0;
			this.count = behaviour.Count;

			for (int i = 0; i < count; i++)
			{
				Spawn(behaviour[i]);
				behaviourIndex++;
			}
		}


		public override void Spawn(Type behaviour)
		{
			float angle = Mathf.Deg2Rad * (behaviourIndex / (float)count) * 360f;
			Vector3 position = new Vector3(Mathf.Cos(angle), 0, -Mathf.Sin(angle));

			position *= Radius;

			float yRot = (Mathf.Rad2Deg * angle) + 270;
			Quaternion rotation = Quaternion.Euler(0, yRot, 0);
			SpawnAt(behaviour, position, rotation);
		}
	}
}
