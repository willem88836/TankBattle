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
		public TankData NewestData = null;

		public void Add(TankData data)
		{
			Data.Enqueue(data);

			if (Data.Count > MAXDATA)
			{
				Data.Dequeue();
			}

			NewestData = data;
		}

		public int Count { get { return Data.Count; } }
	}

	private readonly float[] WEIGHTS = new float[] { 1f, 1f, 1f, 1f };
	private const int MEMORYINTERVAL = 3;
	private int memoryIndex;

	private Dictionary<string, Memory> memories = new Dictionary<string, Memory>();

	private void Start()
	{
		GetOwnData();
	}
	private void Update()
	{
		TankData[] currentData = ReadSensor();
		TankData ownData = GetOwnData();

		if (memoryIndex % MEMORYINTERVAL == 0)
		{
			MemorizeAll(currentData, ownData);
			memoryIndex = 1;
		}
		memoryIndex++;

		TankData target = ChooseTarget(currentData, ownData);


		TankData prediction = PredictTargetBehaviour(target);



		//SetGunAngle(angle);
		//SetSensorAngle(angle);
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

	/// <summary>
	///		Predicts in what manner the tank is going to update next. 
	///		Returns the expected Update.
	/// </summary>
	private TankData PredictTargetBehaviour(TankData target)
	{
		/// TODO: This does not accurately predict the target's position. Predict where it is in X seconds based on the current info.
		Memory memory = memories[target.TankName];
		TankData prediction;
		TankData newest = memory.NewestData;

		if (newest == null)
			return new TankData();

		
		newest.CopyTo(out prediction);

		TankData prev = new TankData();
		foreach(TankData current in memory.Data)
		{
			if (current == memory.Data.Peek())
			{
				prev = current;
				continue;
			}

			prediction.MoveSpeed += current.MoveSpeed - prev.MoveSpeed;
			prediction.TankAngle += current.TankAngle - prev.TankAngle;
			prediction.GunAngle += current.GunAngle - prev.GunAngle;
			prediction.SensorAngle += current.SensorAngle - prev.SensorAngle;

			prev = current;
		}

		prediction.MoveSpeed /= memory.Count;
		prediction.TankAngle /= memory.Count;
		prediction.GunAngle /= memory.Count;
		prediction.SensorAngle /= memory.Count;

		return prediction;
	}


}
