using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Behaviour:
 * This tank will constantly move in circles.
 * This tank will constantly move its sensor and gun around and attemt to shoot when it spots another tank.
 * This will react on being hit by a bullet by then moving in the opposite direction.
 */

public class ExampleAI2 : TankController
{
	// The movementspeed, switches between 1 to -1 if this tank is hit
	int _movePower = 1;

	// Called once when the tank is spawned
	void Start()
    {
		// Change the color of this tank
		SetTankColor(Color.grey);
    }

	// Called every frame after the tank is spawned
	void Update ()
    {
		// Call seperate functions that regulate movement and shooting for this tank's behaviour
        Movement();
        ShootAtEnemy();
	}

    // Function that controls the movement of the tank, it will constantly move in circles
    void Movement()
    {
        // Move forward with the current movePower
        SetMovePower(_movePower);

		// Calculate a new targetAngle for this tank
		float newTargetAngle = GetOwnData().SensorAngle + 90;

		// Apply the new targetAngle to all parts of this tank
		SetTankAngle(newTargetAngle);
        SetGunAngle(newTargetAngle);
        SetSensorAngle(newTargetAngle);
    }

    // Shoot once an enemy has been spotted
    void ShootAtEnemy()
    {
        // Get an array with data from tanks that are in the scanner
        TankData[] otherTanks = ReadSensor();

        // If at least one tank has been detected, shoot!
        if (otherTanks.Length > 0)
        {
            Shoot();
        }
    }

	// Called when this tank is hit by a bullet
	protected override void OnBulletHit()
	{
		_movePower *= -1;
	}
}
