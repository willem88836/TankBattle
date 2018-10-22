using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TomKattouw : TankController
{
	//Mainly for testing purposes of your AI against this AI

	// Cannot call Awake

	TankData _ownTankData;
	TankData[] _enemyData;

	float _movePower = 1;
	float _tankRotation = 0;
	Vector2 _tankRotationMinMax = new Vector2(-30, 30);
	float _sensorDirection = 90;
	bool _tankFound = false;

	float _changeMoveTimer = 3;
	Vector2 _changeMoveMinMax = new Vector2(1, 4);

	void Start()
	{
		// Called once when the tank is spawned
	}

	void Update()
	{
		_ownTankData = GetOwnData();
		_enemyData = ReadSensor();

		if (_enemyData.Length > 0)
			_tankFound = true;

		if (_enemyData.Length == 0 && _tankFound == true)
		{
			_sensorDirection *= -1;
			_tankFound = false;
		}

		Movement();
		RotateSensor();
		Aim();
		Shoot();
	}

	void Aim()
	{
		SetGunAngle(_ownTankData.SensorAngle);
	}

	void RotateSensor()
	{
		SetSensorAngle(_ownTankData.SensorAngle + _sensorDirection);
	}

	void Movement()
	{
		SetMovePower(_movePower);

		_changeMoveTimer -= Time.deltaTime;
		if (_changeMoveTimer <= 0)
		{
			_changeMoveTimer = Random.Range(_changeMoveMinMax.x, _changeMoveMinMax.y);
			_movePower *= -1;

			_tankRotation = Random.Range(_tankRotationMinMax.x, _tankRotationMinMax.y);
		}

		SetTankAngle(_ownTankData.SensorAngle + 90 + _tankRotation);
	}

	protected override void OnWallCollision()
	{
		_changeMoveTimer = 4;
		_movePower *= -1;
	}

	protected override void OnTankCollision()
	{
		// Called when this tank collides with another tank
	}

	protected override void OnBulletHit()
	{
		// Called when this tank is hit by a bullet
	}
}
