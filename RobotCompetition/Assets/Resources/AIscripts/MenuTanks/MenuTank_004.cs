using UnityEngine;
using Framework.Core;

namespace Framework
{
	/// <summary>
	///		This tank zigzags.
	/// </summary>
	public class MenuTank_004 : TankController, IDoNotLoad
	{
		const float rotationUpdate = 1;
		float rot;

		private void Start()
		{
			// NO! DO NOT DO THIS IN YOUR ACTUAL ROBOT BEHAVIOUR!
			// IT'S CHEATING AND THAT SUCKS!
			rot = transform.rotation.eulerAngles.y;
			SetTankAngle(rot);

			SetBodyColor(new Color(1, 0, 1));
			SetGunColor(new Color(1, 0, 1));
		}

		private void Update()
		{
			TankData data = GetOwnData();

			rot += rotationUpdate;

			SetTankAngle(rot);
			SetGunAngle(rot);
			SetSensorAngle(rot);

			SetMovePower(1);
		}
	}
}
