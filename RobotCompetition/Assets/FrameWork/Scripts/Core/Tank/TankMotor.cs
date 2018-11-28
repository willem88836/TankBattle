using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Core
{
	[RequireComponent(typeof(Rigidbody))]
    public class TankMotor : MonoBehaviour
    {
		const string TANKTAG = "Tank";

		[Header("TankParts")]
		[SerializeField] Transform _gunTransform;
		[SerializeField] Transform _sensorTransform;
		[SerializeField] Transform _raycastTransform;

		[Header("Bullets")]
		[SerializeField] GameObject[] _bulletPrefabs = new GameObject[0];
		[SerializeField] Vector3 _bulletSpawnOffset = Vector3.zero;

        [Header("Sounds")]
        [SerializeField] AudioClip _shootSound;
        [SerializeField] AudioClip _damagedSound;

		[Header("Visuals")]
		[SerializeField] GameObject _destroyedVisual;
		[SerializeField] GameObject _tankVisual;
		[SerializeField] TankCanvas _bodyCanvas;
		[SerializeField] TankCanvas _gunCanvas;
		[SerializeField] TankCanvas _sensorCanvas;
		[SerializeField] TankHealth_UI _healthUI;

		[Header("Values")]
		[SerializeField] float _moveSpeed = 3.0f;
		[SerializeField] float _rotationSpeed = 45f;
		[SerializeField] float _startHealth = 100.0f;
		[SerializeField] float _gunCooldown = 0.5f;

		BattleManager _battleManager;
        Rigidbody _rigid;
        TankData _accessData;
		Type _behaviour;

		List<TankMotor> _tanksInSensor;

        float _currentHealth = 0f;
        float _currentGunCooldown = 0f;
		float _rapidGunValue = 0f;

		// Values that will be set by external behaviours
        float _movePower = 0f;
        float _targetTankAngle = 0f;
        float _targetGunAngle = 0f;
        float _targetSensorAngle = 0f;

        Vector3 _previousPosition;
        float _calculatedSpeed;

        public bool _isDestroyed;

		public Action<Type> OnTankDestroyed;

		void Awake()
		{
			_rigid = GetComponent<Rigidbody>();

			_battleManager = BattleManager.Singleton();

			_accessData = new TankData();
			_tanksInSensor = new List<TankMotor>();

			_currentHealth = _startHealth;
			_healthUI.tank = transform;
		}

		void Start()
        {
            _previousPosition = transform.position;

			StopTank();
		}

        void Update()
        {
			// Stop computing behaviour when already destroyed
			if (_isDestroyed)
				return;

            if (_currentGunCooldown > 0.0f)
				_currentGunCooldown -= Time.deltaTime;

			if (_currentGunCooldown <= 0.0f)
			{
				_rapidGunValue = _currentGunCooldown;
				_currentGunCooldown = 0.0f;
			}

            ApplyInput();
        }

        //Calculate true velocity
        private void FixedUpdate()
        {
            Vector3 calcVel = (transform.position - _previousPosition) / Time.fixedDeltaTime;
            _previousPosition = transform.position;
			_calculatedSpeed = calcVel.magnitude;

			_calculatedSpeed = Vector3.Dot(calcVel, transform.forward);

			// Update accessData, does this still need a seperate variable then?
			_accessData.MoveSpeed = _calculatedSpeed;
            //calcSpeed = transform.InverseTransformDirection(calcVel).z;
        }

        //Apply changes to the robot based on the input
        private void ApplyInput()
        {
            //prevent child rotation on the object
            Quaternion prevGunRot = _gunTransform.rotation;
            Quaternion preSensorRot = _sensorTransform.rotation;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0f, _targetTankAngle, 0f), Time.deltaTime * _rotationSpeed);
			_gunTransform.rotation = prevGunRot;
			_sensorTransform.rotation = preSensorRot;

            _rigid.velocity = transform.forward * _movePower * _moveSpeed;

			_gunTransform.rotation = Quaternion.RotateTowards(_gunTransform.rotation, Quaternion.Euler(0f, _targetGunAngle, 0f), Time.deltaTime * _rotationSpeed);
			_sensorTransform.rotation = Quaternion.RotateTowards(_sensorTransform.rotation, Quaternion.Euler(0f, _targetSensorAngle, 0f), Time.deltaTime * _rotationSpeed);

			// Update accessData
			_accessData.Position = transform.position;
			_accessData.TankAngle = transform.eulerAngles.y;
			_accessData.GunAngle = _gunTransform.eulerAngles.y;
			_accessData.SensorAngle = _sensorTransform.eulerAngles.y;
		}

        //Damage this robot, called when hit by a bullet
        public void Damage(float amount)
        {
            _currentHealth -= amount;
			_accessData.Health = _currentHealth;

			if (_healthUI)
				_healthUI.Damage(amount);

			//_audioControl.PlayOneShot(_damagedSound);
			_battleManager.PlaySound(_damagedSound);

			gameObject.SendMessage("OnBulletHit");

            if (_currentHealth <= 0f && !_battleManager.InfiniteHealth())
                DestroyRobot();
        }

        //Function that is called when health reaches zero. Shows an explosion and sets the UI to 'defeated'
        private void DestroyRobot()
        {
			if (_isDestroyed)
				return;

			if (OnTankDestroyed != null)
				OnTankDestroyed.Invoke(_behaviour);

            _isDestroyed = true;
            //string[] defeatString = new string[5] { " got destroyed!", " was defeated!", " got annihilated!", " perished!", " was slain!" };

            // Show a destroy visual
            GetComponent<BoxCollider>().enabled = false;

			_destroyedVisual.SetActive(true);
			_tankVisual.SetActive(false);

            Destroy(gameObject, 1);
        }

        //Functions that are accessable by the robotControl base class

        //Set the movement speed
        public void SetMovePower(float power)
        {
			//set min to -0.5f to make it move slower when backing up?
			power = Mathf.Clamp(power, -1.0f, 1.0f);

			_movePower = power; 
        }

        //Set the target robot rotation
        public void SetTankAngle(float targetAngle)
        {
            _targetTankAngle = targetAngle;
        }

        //Set the target gun rotation
        public void SetGunAngle(float targetAngle)
        {
            _targetGunAngle = targetAngle;
        }

        //Set the target sensor rotation
        public void SetSensorAngle(float targetAngle)
        {
            _targetSensorAngle = targetAngle;
        }

        //Retreives all data from tanks in the sensor, returns an AccesData array
        public TankData[] ReadSensor()
        {
			List<TankData> sensorData = new List<TankData>();

			for (int i = _tanksInSensor.Count - 1; i >= 0; i--)
			{
				TankMotor motor = _tanksInSensor[i];

				// Sorts the tanks in sensor. 
				if (motor == null)
				{
					_tanksInSensor.RemoveAt(i);
					continue;
				}

				// Checks if there is no obstruction 
				// between this and the other tank.
				Ray ray = new Ray(
					_raycastTransform.position, 
					motor.transform.position - _raycastTransform.position);

				RaycastHit hitInfo;
				if (Physics.Raycast(ray, out hitInfo)
					&& hitInfo.transform.tag == TANKTAG)
				{
					TankData data = motor.GetTankData();
					sensorData.Add(data);
				}
			}

            return sensorData.ToArray();
        }

		public void Shoot()
		{
			Shoot(0);
		}

        public void Shoot(int bulletType)
        {
            if (_currentGunCooldown <= 0f)
            {
                _currentGunCooldown = _gunCooldown + _rapidGunValue;

                GameObject newBullet = Instantiate(_bulletPrefabs[bulletType], BattleManager.Singleton().GetBulletContainer());
				Transform bulletTransform = newBullet.transform;
				bulletTransform.position = transform.position + (_gunTransform.rotation * _bulletSpawnOffset);
				bulletTransform.rotation = _gunTransform.rotation;


				newBullet.GetComponent<BulletBehaviour>().SetShooter(this);

				//_audioControl.PlayOneShot(_shootSound);
				_battleManager.PlaySound(_shootSound);
            }
        }

		public void SetBodyColor(Color color)
		{
			//_bodyCanvas.material.color = color;
			_bodyCanvas.ChangeColor(color);
		}

		public void SetGunColor(Color color)
		{
			_gunCanvas.ChangeColor(color);
			_sensorCanvas.ChangeColor(color);
		}

        void OnTriggerEnter(Collider col)
        {
            if (col.isTrigger)
                return;

			_tanksInSensor.RemoveAll(RobotData => RobotData == null);

			TankMotor motor = col.GetComponent<TankMotor>();

			if (motor != null && !_tanksInSensor.Contains(motor))
				_tanksInSensor.Add(motor);
        }

       void OnTriggerExit(Collider col)
        {
			if (col.isTrigger)
				return;

			_tanksInSensor.RemoveAll(RobotData => RobotData == null);

			TankMotor motor = col.GetComponent<TankMotor>();

			if (motor != null && _tanksInSensor.Contains(motor))
				_tanksInSensor.Remove(motor);
		}

        //Function for collisions, sends messages to RobotControl and all derived classes
        void OnCollisionEnter(Collision col)
        {
			TankMotor tank = col.transform.GetComponent<TankMotor>();

			if (tank != null)
			{
				gameObject.SendMessage("OnTankCollision");
				return;
			}

			ArenaWall wall = col.transform.GetComponent<ArenaWall>();

			if (wall != null)
			{
				gameObject.SendMessage("OnWallCollision");
			}
        }

		public void StopTank()
		{
			SetTankAngle(transform.eulerAngles.y);
			SetGunAngle(_gunTransform.eulerAngles.y);
			SetSensorAngle(_sensorTransform.eulerAngles.y);
			SetMovePower(0.0f);
		}

		public void SetBehaviour(Type behaviour)
		{
			gameObject.AddComponent(behaviour);
			gameObject.name = behaviour.ToString();

			_behaviour = behaviour;
		}

		public TankData GetTankData()
		{
			return _accessData.RetreiveData();
		}

		void OnDestroy()
		{
			if (_healthUI)
				_healthUI.DestroyUI();
		}
	}
}
