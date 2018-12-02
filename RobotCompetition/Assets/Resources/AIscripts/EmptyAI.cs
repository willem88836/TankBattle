using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This is an empty behaviour class so you easily can get started.
 * The only thing you need to do is to rename this class and preferably the file as well, to make it match the new class name.
 * Enjoy building your own tank behaviour!
 */

public class EmptyAI : TankController
{
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
