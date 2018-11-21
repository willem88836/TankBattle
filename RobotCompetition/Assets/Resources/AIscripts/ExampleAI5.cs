using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleAI5 : TankController
{
	// This AI will move forwards, untill it hits a wall. It will then keep moving along the wall.
	// The scanner will move around. When an enemy is spotted, it will shoot.
	// if the sensor loses sight of an enemy, it will move in the other direction.

	float _movePower = 1; // The speed at which the tank moves
	float _moveDirection; // The direction the tank is moving in
	bool _isMovingBack;
	float _moveBackDuration = 0.5f;

	TankData[] _sensorData;
	float _sensorRotation = 90;
	bool _enemyFound = false;

	void Start ()
	{
		//Changes the tank color to a random color
		SetBodyColor(Random.ColorHSV());
		SetTurretColor(Random.ColorHSV());
	}
	
	void Update ()
	{
		Movement();
		Scan();
	}

	// decide what kind of movement is used
	void Movement()
	{
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

	// briefly back away from the wall after it has been hit
	void MoveBack()
	{
		// Move backwards
		SetMovePower(-_movePower);

		// Move back for 0.5 seconds
		_moveBackDuration -= Time.deltaTime;

		if (_moveBackDuration <= 0)
			_isMovingBack = false;
	}

	protected override void OnWallCollision()
	{
		// increase the movedirection by 90 degrees when a wall is hit
		_moveDirection += 90;
		_moveBackDuration = 0.5f;
		_isMovingBack = true;
	}

	void Scan()
	{
		// Get the other tankdata from the sensor, if they are found
		_sensorData = ReadSensor();

		// rotate the sensor and the gun
		SetSensorAngle(GetOwnData().SensorAngle + _sensorRotation);
		SetGunAngle(GetOwnData().SensorAngle);

		if (_sensorData.Length > 0)
		{
			// start shooting if an enemy is found
			_enemyFound = true;
			Shoot();
		}
		else if (_sensorData.Length == 0 && _enemyFound == true)
		{
			// reverse the scan rotation if an enemy is lost
			_enemyFound = false;
			_sensorRotation = -_sensorRotation;
		}
	}
}
