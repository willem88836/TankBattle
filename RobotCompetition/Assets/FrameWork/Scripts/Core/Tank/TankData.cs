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

	public void CopyTo(out TankData data)
	{
		data = new TankData()
		{
			Health = this.Health,
			Position = this.Position,
			MoveSpeed = this.MoveSpeed,
			TankAngle = this.TankAngle,
			GunAngle = this.GunAngle,
			SensorAngle = this.SensorAngle
		};
	}

	public TankData RetreiveData()
	{
		TankData newData;
		this.CopyTo(out newData);
		return newData;
	}
}
