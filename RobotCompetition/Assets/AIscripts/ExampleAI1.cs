using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleAI1 : RobotControl {

    //Variable that holds the current movePower
    private int movePower = 1;

    //Start function, has to call the base.Start to be able to reach the motor script
	protected override void Start ()
    {
        //Required for referencing
        base.Start();

        //Changes the robot color
        ChangeColor(Color.blue);
	}
	
    //Called every frame
	void Update () {
        //Sets the movement movepower to the current movePower variable
        MoveRobot(movePower);

        //Make the robot rotate towards a 90 degree angle
        RotateRobot(90);

        //Make the robot's sensor constantly rotating;
        RotateSensor(-90 + MyData.SensorRotation);

        //Make the robot's gun constantly follow the sensor (giving the same angle values)
        RotateGun(-90 + MyData.SensorRotation);

        //If a robot is within the sensor, start shooting
        if (FindTanks().Length > 0)
        {
            //Shoot bullet type 0, currently the only type of bullet available
            Shoot(0);
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
