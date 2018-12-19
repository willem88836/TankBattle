using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class WillemMeijer : TankController
{
	private class Memory
	{
		public const int MAXDATA = 200;

		public string Name = "";
		public float LastUpdateInterval = 0;
		public float LastUpdatedAt = 0;
		public List<TankData> Data = new List<TankData>();
		public TankData NewestData = null;

		public void Add(TankData data)
		{
			Data.Add(data);

			if (Data.Count > MAXDATA)
			{
				Data.RemoveAt(0);
			}

			NewestData = data;
		}

		public int Count { get { return Data.Count; } }
	}

	private readonly float[] WEIGHTS = new float[] { 1f, 1f, 1f, 1f };
	private const int MEMORYINTERVAL = 3;
	private const float BULLETSPEED = 10f;
	private const int DEFAULTROTATERANGE = 120;

	private int defaultRotateDirection = 1;
	private int defaultMoveDirection = 1;

	private int memoryIndex;

	private Dictionary<string, Memory> memories = new Dictionary<string, Memory>();


	private void Update()
	{
		TankData[] sensorData = ReadSensor();
		TankData self = GetOwnData();

		if (memoryIndex % MEMORYINTERVAL == 0)
		{
			MemorizeAll(sensorData, self);
			memoryIndex = 1;
		}
		memoryIndex++;

		if (sensorData.Length > 0)
		{
			TankData target = ChooseTarget(sensorData, self);

			Intercept(self, target);

			float sensorAngle = AngleBetween(self, target);
			SetSensorAngle(sensorAngle);

			defaultRotateDirection = (sensorAngle + 360) % 360 > self.SensorAngle % 360 ? 1 : -1;
		}
		else
		{
			float angle = self.SensorAngle + defaultRotateDirection;
			SetSensorAngle(angle);
			SetGunAngle(angle);

			SetMovePower(Mathf.Sin(Time.time) * defaultMoveDirection);
			SetTankAngle(Mathf.Sin(Time.time) * DEFAULTROTATERANGE * defaultMoveDirection);
		}

		Shoot();
	}

	protected override void OnWallCollision()
	{
		defaultMoveDirection *= -1;
	}
	protected override void OnTankCollision()
	{
		defaultMoveDirection *= -1;
	}


	/// <summary>
	///		Adds all data to the memories.
	/// </summary>
	private void MemorizeAll(TankData[] allData, TankData self)
	{
		foreach (TankData data in allData)
		{
			Memorize(data);
		}
		// Memorising yourself can be usefull. But I'm not sure how to use it yet. 
		//Memorize(self);
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
	private TankData ChooseTarget(TankData[] allData, TankData self)
	{
		float weight = 0;
		int index = 0;

		for(int i = 0; i < allData.Length; i++)
		{
			TankData other = allData[i];

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

		return allData[index];
	}

	/// <summary>
	///		Predicts in what manner the tank is going change in the next X time. 
	///		Returns the expected Update.
	/// </summary>
	private TankData PredictTargetBehaviour(TankData target, int memoryIndex)
	{
		// Selects the data required to do predictions.
		Memory memory = memories[target.TankName];

		//int memoryIndex = Memory.MAXDATA - Mathf.RoundToInt(time / (Time.deltaTime * MEMORYINTERVAL));

		TankData past = memory.Data[memoryIndex];
		TankData current = memory.Data[memory.Data.Count - 1];


		// Adds delta to current information.
		TankData prediction = new TankData()
		{
			Position = current.Position + (current.Position - past.Position),
			MoveSpeed = current.MoveSpeed + (current.MoveSpeed - past.MoveSpeed),
			TankAngle = current.TankAngle + (current.TankAngle - past.TankAngle),
			GunAngle = current.GunAngle + (current.GunAngle - past.GunAngle),
			SensorAngle = current.SensorAngle + (current.SensorAngle - past.SensorAngle),
		};

		return prediction;
	}

	/// <summary>
	///		Returns the angle in degrees between self and target.
	/// </summary>
	private float AngleBetween(TankData origin, TankData target)
	{
		Vector2 delta = new Vector2(
			target.Position.x - origin.Position.x, 
			target.Position.z - origin.Position.z);

		return (float)Math.Atan2(delta.x, delta.y) * Mathf.Rad2Deg;
	}

	/// <summary>
	///		Determines what the optimal gun rotation is to shoot at. 
	/// </summary>
	private void Intercept(TankData self, TankData target)
	{
		float lowestDeviation = float.MaxValue;
		TankData lowestDeviationPrediction = null;

		if (!memories.ContainsKey(target.TankName))
			return;

		Memory memory = memories[target.TankName];
		for (int i = 0; i < memory.Count; i++)
		{
			TankData prediction = PredictTargetBehaviour(target, i);

			float bulletSpeed = BULLETSPEED * Time.deltaTime;
			float distance = (prediction.Position - self.Position).sqrMagnitude;
			float bulletTravelTime = distance / bulletSpeed;

			float tankTravelTime = i * Time.deltaTime;

			float travelDeviation = Mathf.Abs(bulletTravelTime - tankTravelTime);

			if (lowestDeviationPrediction == null 
				|| travelDeviation < lowestDeviation)
			{
				lowestDeviationPrediction = prediction;
				lowestDeviation = travelDeviation;
			}
		}

		float gunAngle = AngleBetween(self, lowestDeviationPrediction);
		SetGunAngle(gunAngle);

		//Debug.DrawLine(self.Position, lowestDeviationPrediction.Position, Color.red);
	}
}
