using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleAI1 : TankController {

    //Variable that holds the current movePower
    private int movePower = 1;

	void Start ()
    {
		//Changes the robot color
		SetBodyColor(Color.blue);
		SetTurretColor(Color.blue);
	}
	
    //Called every frame
	void Update ()
	{
		//Sets the movement movepower to the current movePower variable
		SetMovePower(movePower);

		//Make the robot rotate towards a 90 degree angle
		SetTankAngle(90);

		//Make the robot's sensor constantly rotating;
		SetSensorAngle(-90 + GetOwnData().SensorAngle);

        //Make the robot's gun constantly follow the sensor (giving the same angle values)
        SetGunAngle(-90 + GetOwnData().SensorAngle);

        //If a robot is within the sensor, start shooting
        if (ReadSensor().Length > 0)
        {
            //Shoot bullet
            Shoot();
        }
	}

    protected override void OnWallCollision()
    {
        //move in the opposite direction when colliding with a wall
        movePower *= -1;
    }

    protected override void OnTankCollision()
    {
        //move in the opposite direction when colliding with another robot
        movePower *= -1;
    }
}
