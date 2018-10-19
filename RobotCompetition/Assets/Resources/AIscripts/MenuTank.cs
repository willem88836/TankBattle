using UnityEngine;

namespace Framework
{
	public class MenuTank : TankController, IDoNotLoad
	{
		const float gunSpeed = 25;

		float rot;
		float time = 0;

		private void Start()
		{
			// NO! DO NOT DO THIS IN YOUR ACTUAL ROBOT BEHAVIOUR!
			// IT'S CHEATING AND THAT SUCKS!
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
