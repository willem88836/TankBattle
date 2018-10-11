using System;
using System.Collections.Generic;
using UnityEngine;

public class WillemMeijer : RobotControl
{
	private float movePower = 1;


	private void Start()
	{
		SetBodyColor(Color.blue);
		SetTurretColor(Color.blue);
	}

	private void Update()
	{
		TankData ownData = GetOwnData();
		List<TankData> sensorData = new List<TankData>(ReadSensor());

		SetSensorAngle(ownData.SensorAngle + 1);
		SetGunAngle(ownData.SensorAngle + 1);

		if (sensorData.Count == 0)
		{
			Debug.Log(" nope");
		}
		else
		{
			Debug.Log(" FOUND AND AENENEMT");
			float minDistance = float.MaxValue;
			TankData target = null;
			
			foreach (TankData tank in sensorData)
			{
				float distance = (tank.Position - ownData.Position).sqrMagnitude;
				if (distance < minDistance)
				{
					minDistance = distance;
					target = tank;
				}
			}

			Vector2 normal = target.Position - ownData.Position;
			float targetAngle = Vector3.Angle(target.Position, ownData.Position);

			//SetGunAngle(targetAngle);
			//SetSensorAngle(targetAngle); // TODO: Change this to somehting that makes sense. 
			Shoot();
		}
		SetTankAngle(ownData.TankAngle + Mathf.Sin(Time.deltaTime) * 20);
		SetMovePower(movePower);
		
	}

	protected override void OnWallCollision()
	{
		Debug.Log(" colliding!");
		movePower *= -1;
	}
	protected override void OnTankCollision()
	{
		Debug.Log(" shooting");
		Shoot();
		movePower *= -1;
	}
}
