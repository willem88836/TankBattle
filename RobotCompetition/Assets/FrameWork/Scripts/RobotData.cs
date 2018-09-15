using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Framework;

namespace Framework
{
    [RequireComponent(typeof(RobotMotor))]
    public class RobotData : MonoBehaviour
    {

        private AccessData myData = new AccessData();

        //maybe needs to be moved?
        //Updates the data
        public void UpdateData(float newHealth, Vector3 newPos, float newSpeed, float robotRot, float gunRot, float sensorRot)
        {
            myData = new AccessData(newHealth, newPos, newSpeed, robotRot, gunRot, sensorRot);
        }

        //Not directly sending our data so other robots cannot change it
        public AccessData GetData(Vector3 otherPos)
        {
            AccessData copyData = new AccessData(myData.Health ,myData.Position, myData.MoveSpeed, myData.RobotRotation, myData.GunRotation, myData.SensorRotation);
            copyData.SetDistance(transform.position, otherPos);

            return copyData;
        }

        //mostly used for getting data of own robot
        public AccessData GetData()
        {
            return GetData(Vector3.zero);
        }
    }
}

//class that contains the viewable data from robots
[Serializable]
public class AccessData
{
    private float health = 0f;
    private Vector3 position = Vector3.zero;
    private float moveSpeed = 0f;
    private float robotRotation = 0f;
    private float gunRotation = 0f;
    private float sensorRotation = 0f;
    private float distance = 0f;

    public AccessData(float newHealth, Vector3 newPos, float newSpeed, float newRot, float newGunRot, float newSensorRot)
    {
        health = newHealth;
        position = newPos;
        moveSpeed = newSpeed;
        robotRotation = newRot;
        gunRotation = newGunRot;
        sensorRotation = newSensorRot;
    }

    public AccessData()
    {
        position = Vector3.zero;
        moveSpeed = 0f;
        robotRotation = 0f;
        gunRotation = 0f;
        sensorRotation = 0f;
        distance = 0f;
    }

    public void SetDistance(Vector3 myPos, Vector3 otherPos)
    {
        distance = Vector3.Distance(myPos, otherPos);
    }

    public float Health
    {
        get { return health; }
    }

    public Vector3 Position
    {
        get { return position; }
    }

    public float MoveSpeed
    {
        get { return moveSpeed; }
    }

    public float RobotRotation
    {
        get { return robotRotation; }
    }

    public float GunRotation
    {
        get { return gunRotation; }
    }

    public float SensorRotation
    {
        get { return sensorRotation; }
    }

    public float Distance
    {
        get { return distance; }
    }
}
