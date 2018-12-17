using UnityEngine;
using Framework.Core;

namespace Framework
{
	/// <summary>
	///		This tank zigzags.
	/// </summary>
	public class MenuTank_003 : TankController, IDoNotLoad
	{
		const float forwardTime = 30;
		const float rotateTime = 5;
		const float sensorSpeed = 60;

		float rotationUpdate = 180;

		float rot;
		float time = 0;

		private void Start()
		{
			// NO! DO NOT DO THIS IN YOUR ACTUAL ROBOT BEHAVIOUR!
			// IT'S CHEATING AND THAT SUCKS!
			rot = transform.rotation.eulerAngles.y;
			SetTankAngle(rot);

			SetTankColor(new Color(0.552f, 0.286f, 0));
		}

		private void Update()
		{
			if (time >= forwardTime)
			{
				SetMovePower(0);
				float angle = rot + rotationUpdate;
				SetTankAngle(angle);
				SetGunAngle(angle);
				SetSensorAngle(angle);
				if (time >= forwardTime + rotateTime)
				{
					rot += 180;
					time = 0;
					return;
				}
			}
			else
			{
				SetMovePower(1);
			}

			time += Time.deltaTime;




			float sensorRot = Mathf.Sin(time) * sensorSpeed + rot;
			SetSensorAngle(sensorRot);
		}
	}
}
