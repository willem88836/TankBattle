using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleAI4 : RobotControl {

    //Variable that holds the current movePower
    private float movePower = 1f;
    //Variable that holds the current rotation direction
    private float currentDir = 90f;
    //Variable that holds wether or not a robot has been scanned
    private bool foundRobot = false;

    //Start function, has to call the base.Start to be able to reach the motor script
    protected override void Start () {
        //Required for referencing
        base.Start();
        //Change the color of seperate parts
        ChangeColor(Color.red, Color.white, Color.white);
    }
	
    //Called every frame
    void Update () {
        //Get all the data from the sensor
        TankData[] sensorData = FindTanks();

        //Check how many tanks there are in the sensor
        if (sensorData.Length <= 0)
        {
            //Zero robots are in the sensor
            if (foundRobot)
            {
                //Toggle foundRobot if not already done
                foundRobot = false;
                //Switch sensor rotation if a robot has left the sensor
                currentDir *= -1;
            }
        } else
        {
            //At least one robot is in the sensor
            if (!foundRobot)
            {
                //Toggle foundRobot if not already done
                foundRobot = true;
            }
            //Shoot in the sensor direction
            Shoot(0);
            //Rotate the robot so that it always is at a 90 degree angle compared to an enemies gunrotation
            RotateRobot(sensorData[0].GunAngle + 90f);
        }

        //Move the robot with the current movePower
        MoveRobot(movePower);
        //Rotate the sensor based on the currentDir
        RotateSensor(currentDir + MyData.SensorRotation);
        //Let the gun follow the sensor
        RotateGun(currentDir + MyData.SensorRotation);
    }

    protected override void OnWallCollision()
    {
        //If a wall is hit, reverse the robot's movement
        movePower *= -1;
    }

    protected override void OnVictory()
    {
        StartCoroutine(Victorious());
    }

    private IEnumerator<WaitForSeconds> Victorious()
    {
        while (true)
        {
            Color newclr = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
            ChangeColor(newclr);
            yield return new WaitForSeconds(1);
        }
    }

}
