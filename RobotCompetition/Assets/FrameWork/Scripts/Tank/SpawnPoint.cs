using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
	public class SpawnPoint
	{

		public Vector3 position;
		public float angle;

		public SpawnPoint(Vector3 newPos, float newAngle)
		{
			position = newPos;
			angle = newAngle;
		}
	}
}

