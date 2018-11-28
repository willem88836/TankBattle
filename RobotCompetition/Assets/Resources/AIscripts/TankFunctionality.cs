using UnityEngine;

public class TankFunctionality : TankController
{
	// Called only once at the start.
	void Start ()
	{ }
	
	// Called every frame.
	void Update ()
	{ }

	// Called when the tank collides with a wall.
	protected override void OnWallCollision()
	{ }

	// Called when the tank collides with a tank.
	protected override void OnTankCollision()
	{ }

	// Called when the tank is hit by a bullet.
	protected override void OnBulletHit()
	{ }

	// This function contains all commands that can be called.
	void Functionality()
	{
		// Set the body color of the tank.
		SetBodyColor(Color.red);

		// Set the gun and sensor color of the tank.
		SetGunColor(Color.red);

		// Set the movespeed of the tank, between -1 and 1. 
		// Positive is forwards, negative is backwards.
		SetMovePower(0);

		// Set in whch direction your tank is moving, in degrees. 0 is north, 90 is east, etc.
		SetTankAngle(0);

		// Set in whch direction your gun is aiming, in degrees. 0 is north, 90 is east, etc
		SetGunAngle(0);

		// Set in whch direction your sensor is aiming, in degrees. 0 is north, 90 is east, etc.
		SetSensorAngle(0);

		// shoot continuously, in te direction the gun is aiming. 
		Shoot();

		// Tankdata contains information about your tank, or other tanks.
		TankData myTank;
		TankData[] otherTanks;

		// To get information about your own tank.
		myTank = GetOwnData();

		// To get information about other tanks.
		otherTanks = ReadSensor();

		// The following information can be retrieved from this:
		float health = myTank.Health;
		Vector3 position = myTank.Position;
		float moveSpeed = myTank.MoveSpeed;
		float tankAngle = myTank.TankAngle;
		float gunAngle = myTank.GunAngle;
		float sensorAngle = myTank.SensorAngle;
	}
}
