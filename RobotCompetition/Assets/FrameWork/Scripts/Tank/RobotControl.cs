using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

[RequireComponent(typeof(TankMotor))]
public class RobotControl : MonoBehaviour
{

    /* RULES:
        * no GetComponent<> allowed this also means you are not allowed to call transform.position on your own robot, you can access your own data with MyData.<what you want to know>
        * no transform.GetChild() allowed
        * no GameObject.Find() allowed, or similar things
        * no RayCasts or similar environment checks like CheckBox or OverlapBox
        * your artificial intelligence has to be derived from RobotControl, take a look at the example ai for a better understanding of the functions below.
        * you are allowed to look at other scripts but you are absolutely not allowed to change them.
        */

    //References to required components
    private TankMotor _motor;

	void Awake()
	{
		_motor = GetComponent<TankMotor>();
	}

    //Needs to be referenced like: protected override void OnWallCollision()
    //Called when the robot enters a collision with a wall
    protected virtual void OnWallCollision()
    {

    }

    //Needs to be referenced like: protected override void OnTankCollision()
    //Called when the robot enters a collision with another robot
    protected virtual void OnTankCollision()
    {

    }

    protected void SetBodyColor(Color color)
	{
		_motor.SetBodyColor(color);
	}

	protected void SetTurretColor(Color color)
	{
		_motor.SetTurretColor(color);
	}

    //Changes the move speed input variable in the motor script
    protected void SetMovePower(float movePower)
    {
        _motor.SetMovePower(movePower);
    }

    //Changes the robot target rotation input variable in the motor script
    protected void SetTankAngle(float degree)
    {
        _motor.SetTankAngle(degree);
    }

    //Changes the gun target rotation input variable in the motor script
    protected void SetGunAngle(float degree)
    {
        _motor.SetGunAngle(degree);
    }

    //Changes the sensor target rotation input variable in the motor script
    protected void SetSensorAngle(float degree)
    {
        _motor.SetSensorAngle(degree);
    }

    //Attempts to shoot a bullet, this has a cooldown
    protected void Shoot()
    {
        _motor.Shoot();
    }

    //Retreive the data of all the robot's AccesData in the sensor and returns it as an array
    protected TankData[] ReadSensor()
    {
        return _motor.ReadSensor();
    }

    //Retreive your own robot's data
    protected TankData GetOwnData()
    {
		return _motor.GetTankData();
    }
}