using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Behaviour:
 * This tank's sensor and gun will constantly rotate untill it finds a tank.
 * If a tank leaves the sensor the turning direction of the sensor and gun will reverse, making the sensor move over the tank again.
 * This tank will not rotate itself.
 * This tank will periodically move forwards or backwards, switching every 4 second.
 */

public class ExampleAI3 : TankController
{
	// The timer used to move forward and backwards. Also used to set the speed for the tank
	private float moveTimer= 4f;

	// Checks if the tank is moving forward or backward
	private bool moveForward;

	// This is used to rotate the scanner in a direction. Will be set to negative to turn it counterclockwise
	private int scanDirection = 90;

	// The amount of other tanks found. This is used to check if a tank leaves the scanner, to make the scanner turn in another direction
	private int otherTanksStored = 0;

	// Called once when the tank is spawned
	void Start()
    {
        
    }

	// Called every frame after the tank is spawned
	void Update ()
    {
		// Controls the movement
		Movement();

		// Controls the scanner and shooting
		Scan();
	}

    void Movement()
    {
        // Move forward and backward, switching every 4 seconds
        moveTimer -= Time.deltaTime;
        if (moveTimer <= 0)
        {
            moveTimer = 4;
            moveForward = !moveForward;
        }

        // The movePower is equal to the moveTimer. Whenever this is > 1, the script will automatically reduce the movePower to the maximum of 1 (or -1)
        // This also means that the movespeed will be reduced whenver the movePower will reduce when moveTimer goes below 1 
        if (moveForward) 
			SetMovePower(moveTimer);
        else
            SetMovePower(-moveTimer);
    }

    void Scan()
    {
        // Scan in circles, if a tank leaves the scanner, turn the scanner in the other direction
        TankData[] otherTanks = ReadSensor();

        // If a tank is seen, shoot!
        if (otherTanks.Length > 0)
        {
            Shoot();
        }

        // This part checks if a tank has left the scanner area. If thats true, it will scan in the oposite direction
        if (otherTanksStored > otherTanks.Length)
        {
            scanDirection = -scanDirection;
        }
        otherTanksStored = otherTanks.Length;

        SetSensorAngle(scanDirection + GetOwnData().SensorAngle);
        SetGunAngle(scanDirection + GetOwnData().SensorAngle);
    }
}
