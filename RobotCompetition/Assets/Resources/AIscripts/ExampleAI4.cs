using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Behaviour:
 * This tank's sensor and gun will constantly rotate untill it finds a tank.
 * If a tank leaves the sensor the turning direction of the sensor and gun will reverse, making the sensor move over the tank again.
 * This tank will rotate itself based on the gun rotation of a scanned enemy tank.
 * This tank will switch movement directions when colliding with a wall.
 */

public class ExampleAI4 : TankController
{
    // Variable that holds the current movePower
    private float movePower = 1f;

    // Variable that holds the current rotation direction
    private float currentDir = 90f;

    // Variable that holds wether or not a robot has been scanned
    private bool foundRobot = false;

	// Called once when the tank is spawned
	void Start()
	{
		// Change the color of seperate tank parts
		SetBodyColor(Color.red);
		SetGunColor(Color.white);
    }

	// Called every frame after the tank is spawned
	void Update () {
        // Get all the data from the sensor
        TankData[] sensorData = ReadSensor();

        // Check how many tanks there are in the sensor
        if (sensorData.Length <= 0)
        {
            // Zero robots are in the sensor
            if (foundRobot)
            {
                // Toggle foundRobot if not already done
                foundRobot = false;

                // Switch sensor rotation if a robot has left the sensor
                currentDir *= -1;
            }
        } else
        {
            // At least one robot is in the sensor
            if (!foundRobot)
            {
                // Toggle foundRobot if not already done
                foundRobot = true;
            }

            // Shoot in the sensor direction
            Shoot();

            // Rotate the robot so that it always is at a 90 degree angle compared to an enemies gunrotation
            SetTankAngle(sensorData[0].GunAngle + 90f);
        }

        // Move the robot with the current movePower
        SetMovePower(movePower);

        // Rotate the sensor based on the currentDir
        SetSensorAngle(currentDir + GetOwnData().SensorAngle);

        // Let the gun follow the sensor
        SetGunAngle(currentDir + GetOwnData().SensorAngle);
    }

	// Called when this tank collides with a wall
	protected override void OnWallCollision()
    {
        // If a wall is hit, reverse the robot's movement
        movePower *= -1;
    }
}
