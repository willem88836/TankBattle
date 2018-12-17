using UnityEngine;

public class Debug_02 : TankController
{
	//Only change color to white
	//Mainly for testing purposes of your AI against this AI

	// Cannot call Awake

	const float GUN_OFFSET = 90;

	float _movePower = 1;
	float _gunDirection = 1;

	bool _hadTankInSensor;

	void Start()
    {
		// Called once when the tank is spawned

		//SetTurretColor(Color.cyan);
		//SetBodyColor(Color.cyan);

		SetTankAngle(90);
		SetMovePower(_movePower);
    }

	void Update()
	{
		// Called every frame after the tank is spawned
		TankData[] sensorData = ReadSensor();

		bool tankInSensor = sensorData.Length > 0;

		if (!tankInSensor && _hadTankInSensor)
			SwitchGunDirection();

		_hadTankInSensor = tankInSensor;

		TankData ownData = GetOwnData();

		float targetAngle = ownData.GunAngle + _gunDirection * GUN_OFFSET;
		SetGunAngle(targetAngle);
		SetSensorAngle(targetAngle);

		Shoot();
	}

	void SwitchTankDirection()
	{
		_movePower *= -1;
		SetMovePower(_movePower);
	}

	void SwitchGunDirection()
	{
		_gunDirection *= -1;
	}

	protected override void OnWallCollision()
	{
		// Called when this tank collides with a wall
		SwitchTankDirection();
	}

	protected override void OnTankCollision()
	{
		// Called when this tank collides with another tank
		SwitchTankDirection();
	}

	protected override void OnBulletHit()
	{
		// Called when this tank is hit by a bullet
		SwitchTankDirection();
	}
}
