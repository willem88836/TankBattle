using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

[RequireComponent(typeof(RobotData))]
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
    private TankMotor motor;
    private RobotData data;
    private Renderer[] colorParts = new Renderer[4];

    //Assigns required variables, your AI needs to call this function this way: protected override void Start()
    protected virtual void Start()
    {
        //Reference components
        motor = GetComponent<TankMotor>();
        data = GetComponent<RobotData>();

        //Reference colorable parts
        colorParts[0] = transform.GetChild(0).GetChild(0).GetComponent<Renderer>();
        colorParts[1] = transform.GetChild(2).GetChild(0).GetComponent<Renderer>();
        colorParts[2] = transform.GetChild(2).GetChild(1).GetComponent<Renderer>();
        colorParts[3] = transform.GetChild(2).GetChild(2).GetComponent<Renderer>();
    }

    //Needs to be referenced like: protected override void OnWalCollision()
    //Called when the robot enters a collision with a wall
    protected virtual void OnWallCollision()
    {

    }

    //Needs to be referenced like: protected override void OnTankCollision()
    //Called when the robot enters a collision with another robot
    protected virtual void OnTankCollision()
    {

    }

    //Needs to be referenced like: protected override void OnBulletCollision()
    //Called when the robot is hit by a bullet
    protected virtual void OnBulletCollision()
    {

    }

    //Needs to be referenced like: protected override void OnVictory()
    //Called when your robot has won
    protected virtual void OnVictory()
    {

    }

    //Changes the colors of seperate parts
    protected void ChangeColor(Color gunColor, Color bodyColor, Color wheelColor)
    {
        ApplyColor(colorParts[0], gunColor);
        ApplyColor(colorParts[1], bodyColor);
        ApplyColor(colorParts[2], wheelColor);
        ApplyColor(colorParts[3], wheelColor);
    }

    //Changes the color of the whole tank
    protected void ChangeColor(Color robotColor)
    {
        for (int i = 0; i < colorParts.Length; i++)
        {
            ApplyColor(colorParts[i], robotColor);
        }
    }

    //Applies the color to the individual colorable part
    private void ApplyColor(Renderer part, Color newColor)
    {
        for (int i = 0; i < colorParts.Length; i++)
        {
            part.materials[0].color = newColor;
        }
    }

    //Changes the move speed input variable in the motor script
    protected void MoveRobot(float movePower)
    {
        motor.MoveRobot(movePower);
    }

    //Changes the robot target rotation input variable in the motor script
    protected void RotateRobot(float degree)
    {
        motor.RotateRobot(degree);
    }

    //Changes the gun target rotation input variable in the motor script
    protected void RotateGun(float degree)
    {
        motor.RotateGun(degree);
    }

    //Changes the sensor target rotation input variable in the motor script
    protected void RotateSensor(float degree)
    {
        motor.RotateSensor(degree);
    }

    //Attempts to shoot a bullet, this has a cooldown
    protected void Shoot(int index)
    {
        motor.Shoot(index);
    }

    //Retreive the data of all the robot's AccesData in the sensor and returns it as an array
    protected TankData[] FindTanks()
    {
        return motor.ReadSensor();
    }

    //Retreive your own robot's data
    protected AccessData MyData
    {
        get { return data.GetData(); }
    }
}