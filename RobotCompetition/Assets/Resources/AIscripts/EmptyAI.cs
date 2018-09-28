using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyAI : RobotControl {

    //Only change color to white
    //Mainly for testing purposes of your AI against this AI

	// Cannot call Awake

	void Start()
    {
		//SetBodyColor(Color.white);
		//SetTurretColor(Color.yellow);
    }

	void Update()
	{
		TankData ownData = GetOwnData();

		SetTankAngle(ownData.TankAngle + 90);
		SetMovePower(0.5f);
	}

	protected override void OnWallCollision()
	{

	}

	protected override void OnTankCollision()
	{

	}

	protected override void OnBulletCollision()
	{

	}
}
