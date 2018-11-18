using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleAI2 : TankController
{
    // this AI has a simple mechanic. It will keep moving in circles, and shoots whenever another tank is spotted. 
    // It will switch movement when it is hit by a bullet

    int moveSpeed = 1; // the movementspeed. Switches between 1 to -1 if this tank is hit

    void Start()
    {
		// Change the color of this tank
		SetBodyColor(Color.grey);
		SetTurretColor(Color.grey);
    }

    // Update is called once per frame
    void Update ()
    {
        Movement();
        ShootAtEnemy();
	}

    // function that controls the movement of the tank. It will constantly move in circles
    void Movement()
    {
        // move forward with the max forward speed of 1
        SetMovePower(moveSpeed);

        // rotate the robot, gun and scanner clockwise
        SetTankAngle(GetOwnData().SensorAngle + 90);
        SetGunAngle(GetOwnData().SensorAngle + 90);
        SetSensorAngle(GetOwnData().SensorAngle + 90);
    }

    // shoot once an enemy has been spotted
    void ShootAtEnemy()
    {
        // get an array with data from tanks that are scanned
        TankData[] otherTanks = ReadSensor();

        // if at least one tank has been scanned, shoot
        if (otherTanks.Length > 0)
        {
            Shoot();
        }
    }
}
