using UnityEngine;
using Framework.Core;

namespace Framework
{
	/// <summary>
	///		This tank zigzags.
	/// </summary>
	public class MenuTank_002 : TankController, IDoNotLoad
	{
		const float forwardTime = 17;
		const float rotateTime = 5;
		const float sensorSpeed = 60;
		const float rotationUpdate = 180;
		const int maxTurns = 5;

		float rot;
		float time = 0;

		Quaternion originalRot;
		Vector3 originalPos;

		int turns = 0;


		private void Start()
		{
			// NO! DO NOT DO THIS IN YOUR ACTUAL ROBOT BEHAVIOUR!
			// IT'S CHEATING AND THAT SUCKS!
			rot = transform.rotation.eulerAngles.y;
			SetTankAngle(rot);

			originalRot = transform.rotation;
			originalPos = transform.position;
		}

		private void Update()
		{
			if (time >= forwardTime)
			{
				rot += rotationUpdate;

				SetTankAngle(rot);
				SetGunAngle(rot);

				time = 0;


				turns++;

				if(turns >= maxTurns)
				{
					turns = 0;
					float origRot = originalRot.eulerAngles.y;
					rot = origRot;
					SetTankAngle(origRot);
					SetGunAngle(origRot);

					transform.position = originalPos;
					transform.rotation = originalRot;

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
