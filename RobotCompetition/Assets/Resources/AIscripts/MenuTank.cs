﻿using UnityEngine;

namespace Framework
{
	public class MenuTank : RobotControl
	{
		const float gunSpeed = 25;

		TankData tankData;
		float rot;
		float time = 0;

		private void Start()
		{
			tankData = GetOwnData();

			rot = transform.rotation.eulerAngles.y;

			SetTankAngle(rot);
		}

		private void Update()
		{
			float sensorRotation = GetOwnData().SensorAngle;
			sensorRotation += 1;
			SetSensorAngle(sensorRotation);


			time += Time.deltaTime;

			float gunRot = Mathf.Sin(time) * gunSpeed + rot;

			SetGunAngle(gunRot);
		}
	}
}
