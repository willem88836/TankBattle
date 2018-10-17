using UnityEngine;
using Framework;

[RequireComponent(typeof(TankMotor))]
public abstract class TankController : MonoBehaviour
{

    /* RULES:
        * no GetComponent<> allowed this also means you are not allowed to call transform.position on your own robot, you can access your own data with MyData.<what you want to know>
        * no transform.GetChild() allowed
        * no GameObject.Find() allowed, or similar things
        * no RayCasts or similar environment checks like CheckBox or OverlapBox
        * your behaviour has to be derived from RobotControl, take a look at the example AI's for a better understanding of the functions below.
        * you are allowed to look at other scripts but you are absolutely not allowed to change them.
        */

    //References to required components
    private TankMotor _motor;

	// Assign the motor component
	void Awake()
	{
		_motor = GetComponent<TankMotor>();
	}

	// Called when the tank collides with a wall
	protected virtual void OnWallCollision()
	{

	}

	// Called when the tank collides with another tank
	protected virtual void OnTankCollision()
	{

	}

	// Called when this tank is hit by a bullet
	protected virtual void OnBulletHit()
	{

	}

	// Set the body color of your tank
    protected void SetBodyColor(Color color)
	{
		_motor.SetBodyColor(color);
	}

	// Set the turret color of your rank
	protected void SetTurretColor(Color color)
	{
		_motor.SetTurretColor(color);
	}

    // Set the move power of your tank, 1 is full power forward, -1 is full power backwards
    protected void SetMovePower(float movePower)
    {
        _motor.SetMovePower(movePower);
    }

    // Set the target tank angle of your tank, your tank will rotate towards this angle untill it receives a new angle
    protected void SetTankAngle(float degree)
    {
        _motor.SetTankAngle(degree);
    }

	// Set the target gun angle of your tank, your gun will rotate towards this angle untill it receives a new angle
	protected void SetGunAngle(float degree)
    {
        _motor.SetGunAngle(degree);
    }

	// Set the target sensor angle of your tank, your sensor will rotate towards this angle untill it receives a new angle
	protected void SetSensorAngle(float degree)
    {
        _motor.SetSensorAngle(degree);
    }

    // Shoot a bullet, note that this has a cooldown. It will do nothing when the gun is still cooling down
    protected void Shoot()
    {
        _motor.Shoot();
    }

    // Read your sensor and get the data from all the tanks currently in your sensor
    protected TankData[] ReadSensor()
    {
        return _motor.ReadSensor();
    }

    // Retreive your own tank's data
    protected TankData GetOwnData()
    {
		return _motor.GetTankData();
    }
}