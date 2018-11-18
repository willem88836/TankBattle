using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class ExampleAI3 : TankController {

    // This tank will periodically move forwards or backwards, switching every 4 seconds
    // The scanner will turn clockwise, along with the turret. As long as another tank is in the scanner, it will keep shooting
    // when a tank leaves the scanner area, the scanner will rotate in the oposite direction

    private float moveTimer= 4f;    // The timer used to move forward and backwards. Also used to set the speed for the tank
    private bool moveForward;       // Checks if the tank is moving forward or backward

    private int scanDirection = 90; // This is used to rotate the scanner in a direction. Will be set to negative to turn it counterclockwise

    private int otherTanksStored = 0; // the amount of other tanks found. This is used to check if a tank leaves the scanner, to make the scanner turn in another direction 

    // Use this for initialization
    void Start()
    {
        
    }
	
	// Update is called once per frame
	void Update ()
    {
        Movement(); // controls the movement
        Scan(); // controls the scanner and shooting
	}

    void Movement()
    {
        // move forward and backward. Switch every 4 seconds
        moveTimer -= Time.deltaTime;
        if (moveTimer <= 0)
        {
            moveTimer = 4;
            moveForward = !moveForward;
        }

        // the movePower is equal to the moveTimer. Whenever this is > 1, the script will automatically reduce the movePower to the maximum of 1 (or -1)
        // this also means that the movespeed will be reduced whenver the movePower will reduce when moveTimer goes below 1 
        if (moveForward) 
			SetMovePower(moveTimer);
        else
            SetMovePower(-moveTimer);
    }

    void Scan()
    {
        // scan in circles. If a tank leaves the scanner, turn the scanner in the other direction
        TankData[] otherTanks = ReadSensor();

        // if a tank is seen, shoot
        if (otherTanks.Length > 0)
        {
            Shoot();
        }

        // this part checks if a tank has left the scanner area. If thats true, it will scan in the oposite direction
        if (otherTanksStored > otherTanks.Length)
        {
            scanDirection = -scanDirection;
        }
        otherTanksStored = otherTanks.Length;

        SetSensorAngle(scanDirection + GetOwnData().SensorAngle);
        SetGunAngle(scanDirection + GetOwnData().SensorAngle);
    }
}
