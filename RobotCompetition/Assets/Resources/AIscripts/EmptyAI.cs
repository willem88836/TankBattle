﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyAI : TankController {

    //Only change color to white
    //Mainly for testing purposes of your AI against this AI

	// Cannot call Awake

	void Start()
    {
		// Called once when the tank is spawned
    }

	void Update()
	{
		// Called every frame after the tank is spawned
	}

	protected override void OnWallCollision()
	{
		// Called when this tank collides with a wall
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
