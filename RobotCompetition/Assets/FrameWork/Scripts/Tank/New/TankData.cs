using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankData
{
	[HideInInspector] public string TankName = "";

	[HideInInspector] public float Health = 0f;
	[HideInInspector] public Vector3 Position = Vector3.zero;
	[HideInInspector] public float MoveSpeed = 0f;
	[HideInInspector] public float TankAngle = 0f;
	[HideInInspector] public float GunAngle = 0f;
	[HideInInspector] public float SensorAngle = 0f;

	public void Update(Framework.TankMotor motor)
	{
		Health = motor.GetHealth();
		Position = motor.GetPosition();
		MoveSpeed = motor.GetCalculatedMoveSpeed();
		TankAngle = motor.GetTankAngle();
		GunAngle = motor.GetGunAngle();
		SensorAngle = motor.GetSensorAngle();
	}

	public TankData CopyTo(TankData data)
	{
		if (data == null)
			data = new TankData();

		data.Health = this.Health;
		data.Position = this.Position;
		data.MoveSpeed = this.MoveSpeed;
		data.TankAngle = this.TankAngle;
		data.GunAngle = this.GunAngle;
		data.SensorAngle = this.SensorAngle;

		return data;
	}

	public TankData RetreiveData()
	{
		TankData newData = new TankData();
		return this.CopyTo(newData);
	}
}
