using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Behaviour:
 * This tank's sensor and gun will constantly rotate untill it finds a tank.
 * If a tank leaves the sensor the turning direction of the sensor and gun will reverse, making the sensor move over the tank again.
 * This Tank will move forwards untill it collides with a wall, then it will continue moving along the wall.
 */

public class ExampleAI5 : TankController
{
	// The speed at which the tank moves
	float _movePower = 1;

	// The direction the tank is moving in
	float _moveDirection;

	// Variable that holds information wether the tank is moving back or not
	bool _isMovingBack;

	// Variable that holds the duration of the back movement
	float _moveBackDuration = 0.5f;

	// Variable that stores the data from the sensor
	TankData[] _sensorData;

	// Variable that stores the sensor rotation offset
	float _sensorRotation = 90;

	// Variable that holds if enemies have been found in the scanner
	bool _enemyFound = false;

	// Called once when the tank is spawned
	void Start ()
	{
		//Changes the tank part's colors to a random color
		SetBodyColor(Random.ColorHSV());
		SetGunColor(Random.ColorHSV());
	}

	// Called every frame after the tank is spawned
	void Update ()
	{
		// Function that controls movement
		Movement();

		// Function that controls scanning and shooting
		Scan();
	}

	// Decide what kind of movement is used
	void Movement()
	{
		// Give back movement priority if it is enabled
		if (_isMovingBack)
			MoveBack();
		else
			Move();
	}

	// Control the movement of the tank
	void Move()
	{
		if (_moveDirection == GetOwnData().TankAngle)
		{
			// Move forward if the tank is not rotating
			SetMovePower(_movePower);
		}
		else
		{
			// Or rotate the tank if _moveDirection is different from the current direct
			SetTankAngle(_moveDirection);
			SetMovePower(0);
		}
	}

	// Briefly back away from the wall after it has been hit
	void MoveBack()
	{
		// Move backwards
		SetMovePower(-_movePower);

		// Move back for 0.5 seconds
		_moveBackDuration -= Time.deltaTime;

		if (_moveBackDuration <= 0)
			_isMovingBack = false;
	}

	// Called when this tank collides with a wall
	protected override void OnWallCollision()
	{
		// Increase the movedirection by 90 degrees when a wall is hit
		_moveDirection += 90;
		_moveBackDuration = 0.5f;
		_isMovingBack = true;
	}

	void Scan()
	{
		// Get the other tankdata from the sensor, if they are found
		_sensorData = ReadSensor();

		// Rotate the sensor and the gun
		SetSensorAngle(GetOwnData().SensorAngle + _sensorRotation);
		SetGunAngle(GetOwnData().SensorAngle);

		if (_sensorData.Length > 0)
		{
			// Start shooting if an enemy is found
			_enemyFound = true;
			Shoot();
		}
		else if (_sensorData.Length == 0 && _enemyFound == true)
		{
			// Reverse the scan rotation if an enemy is lost
			_enemyFound = false;
			_sensorRotation = -_sensorRotation;
		}
	}
}
