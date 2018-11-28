using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Behaviour:
 * This tank's sensor and gun will constantly rotate in the same direction scanning for other tanks, when other tanks are found he will try to shoot.
 * This tank will start with rotating to a 90 degree angle (horizontal) and will start with moving forward.
 * This will react on certain events like hitting another tank or wall by then moving in the opposite direction.
 */

public class ExampleAI1 : TankController {

    // Variable that holds the current movePower
    int _movePower = 1;

	// Called once when the tank is spawned
	void Start ()
    {
		// Change the tank's color
		SetTankColor(Color.blue);

		// Make the tank rotate towards a 90 degree angle
		SetTankAngle(90);
	}

	// Called every frame after the tank is spawned
	void Update ()
	{
		// Sets the movement movepower to the current movePower variable
		SetMovePower(_movePower);

		// Make the tank's sensor constantly rotating by always setting the targetAngle to its current angle + a constant value
		SetSensorAngle(-90 + GetOwnData().SensorAngle);

        // Make the tank's gun constantly follow the sensor (giving the same angle values)
        SetGunAngle(-90 + GetOwnData().SensorAngle);

        // If a tank is within the sensor, start shooting
        if (ReadSensor().Length > 0)
        {
            // Shoot a bullet, is called every frame but the internal cooldown prevents is from shooting too fast
            Shoot();
        }
	}

	// Called when this tank collides with a wall
	protected override void OnWallCollision()
    {
        // Move in the opposite direction when colliding with a wall
        _movePower *= -1;
    }

	// Called when this tank collides with another tank
	protected override void OnTankCollision()
    {
        // Move in the opposite direction when colliding with another tank
        _movePower *= -1;
    }

	// Called when this tank is hit by a bullet
	protected override void OnBulletHit()
	{
		// No functionality
	}
}
