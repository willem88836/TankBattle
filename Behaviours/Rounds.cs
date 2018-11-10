using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rounds : TankController
{
	TankData _ownTankData;
	TankData[] _enemyData;

	float _startAngle = 45f;
	bool _canMove = false;

	float _rotateSpeed = 0.3f;
	float _sensorSpeed = 90;


	static void Main() { }

	void Start()
	{
		StartCoroutine(InitStartAngle());

		SetBodyColor(Color.blue);
		SetTurretColor(Color.black);
	}

	void Update ()
	{
		_ownTankData = GetOwnData();
		_enemyData = ReadSensor();

		Movement();
		Aim();
	}

	IEnumerator InitStartAngle()
	{
		yield return new WaitUntil(
			() => _ownTankData != null);

		_startAngle = _ownTankData.TankAngle - 45;

		StartCoroutine(StartMoving());
	}

	IEnumerator StartMoving()
	{
		yield return new WaitUntil(
			() => _ownTankData.TankAngle == _startAngle 
			|| _ownTankData.TankAngle == 360 + _startAngle);

		// start moving
		SetMovePower(1);
		_canMove = true;
	}

	// Move
	void Movement()
	{
		if (_canMove == false)
			RotateToStartAngle();
		else
			MoveInCircles();
	}

	// Rotate to a set angle before moving in circles
	void RotateToStartAngle()
	{
		SetTankAngle(_startAngle);
	}

	// rotate in circles
	void MoveInCircles()
	{
		SetTankAngle(_ownTankData.TankAngle + _rotateSpeed);
	}

	// Aim
	void Aim()
	{
		Shoot();

		if (_enemyData.Length == 0)
			LookForTarget();
		else
			LockOnTarget();
	}

	// Look for a target
	void LookForTarget()
	{
		SetSensorAngle(_ownTankData.SensorAngle + _sensorSpeed);
		SetGunAngle(_ownTankData.SensorAngle);
	}

	// Keep focussing on a target once its found
	void LockOnTarget()
	{
		TankData enemy = _enemyData[0];
		Vector3 enemyPosition = enemy.Position;

		Quaternion lookDirection = Quaternion.LookRotation(
			(enemyPosition - _ownTankData.Position).normalized);

		SetSensorAngle(lookDirection.eulerAngles.y);
		SetGunAngle(_ownTankData.SensorAngle);
	}
}
