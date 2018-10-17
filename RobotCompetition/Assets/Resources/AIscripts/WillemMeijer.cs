using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class WillemMeijer : TankController
{
	private class Memory
	{
		private const int MAXDATA = 200;

		public string Name = "";
		public float LastUpdateInterval = 0;
		public float LastUpdatedAt = 0;
		public Queue<TankData> Data = new Queue<TankData>();

		public void Add(TankData data)
		{
			Data.Enqueue(data);

			if (Data.Count > MAXDATA)
			{
				Data.Dequeue();
			}
		}
	}

	private readonly float[] WEIGHTS = new float[] { 1, 1, 1, 1 };


	private Dictionary<string, Memory> memories = new Dictionary<string, Memory>();


	private void Update()
	{
		TankData[] currentData = ReadSensor();
		TankData ownData = GetOwnData();

		MemorizeAll(currentData, ownData);

		TankData target = ChooseTarget(currentData, ownData);


		float angle = GetAnticipatedAngle(target);

		SetGunAngle(angle);
		SetSensorAngle(angle);
		Shoot();
	}


	/// <summary>
	///		Adds all data to the memories.
	/// </summary>
	private void MemorizeAll(TankData[] currentData, TankData ownData)
	{
		foreach (TankData data in currentData)
		{
			Memorize(data);
		}
		Memorize(ownData);
	}
	/// <summary>
	///		Adds one piece of data to memories.
	/// </summary>
	private void Memorize(TankData data)
	{
		string name = data.TankName;

		if (!memories.ContainsKey(name))
		{
			Memory newMemory = new Memory()
			{
				Name = name,
			};
			memories.Add(name, newMemory);
		}

		Memory memory = memories[name];
		memory.LastUpdateInterval = Time.time - memory.LastUpdatedAt;
		memory.LastUpdatedAt = Time.time;
		memory.Add(data);
	}

	/// <summary>
	///		Determines what tank is the next target.
	/// </summary>
	private TankData ChooseTarget(TankData[] currentData, TankData self)
	{
		float weight = 0;
		int index = 0;

		for(int i = 0; i < currentData.Length; i++)
		{
			TankData other = currentData[i];

			float distance = WEIGHTS[0] * (other.Position - self.Position).magnitude;
			float health = WEIGHTS[1] * other.Health;
			float sensorAngle = WEIGHTS[2] * other.SensorAngle;
			float gunAngle = WEIGHTS[3] * other.GunAngle;

			float currentWeight = distance + health + sensorAngle + gunAngle;

			if (currentWeight > weight)
			{
				index = i;
				weight = currentWeight;
			}
		}

		return currentData[index];
	}





	private float GetAnticipatedAngle(TankData target)
	{
		throw new NotImplementedException();
	}
}
