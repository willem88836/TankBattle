using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debug_01 : RobotControl {

    //Only change color to white
    //Mainly for testing purposes of your AI against this AI

	// Cannot call Awake

	void Start()
    {
		// Called once when the tank is spawned
    }

	void Update()
	{
		// Called every frame after the tank is spawned
		TankData ownData = GetOwnData();

		float newAngle = ownData.GunAngle + 90.0f;

		SetGunAngle(newAngle);
		SetSensorAngle(newAngle);

		TankData[] sensorData = ReadSensor();

		if (sensorData.Length > 0)
			Shoot();
	}

	protected override void OnWallCollision()
	{
		// Called when this tank collides with a wall
	}

	protected override void OnTankCollision()
	{
		// Called when this tank collides with another tank
	}
}
