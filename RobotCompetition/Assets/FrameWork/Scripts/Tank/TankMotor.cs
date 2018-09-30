using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

namespace Framework
{
    [RequireComponent(typeof(Rigidbody))]
    public class TankMotor : MonoBehaviour
    {

		[Header("TankParts")]
		[SerializeField] Transform _gunTransform;
		[SerializeField] Transform _sensorTransform;

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
		[SerializeField] TankCanvas _turretCanvas;
		
		[Header("Values")]
		[SerializeField] float _moveSpeed = 3.0f;
		[SerializeField] float _rotationSpeed = 45f;
		[SerializeField] float _startHealth = 100.0f;
		[SerializeField] float _gunCooldown = 0.5f;

        Rigidbody _rigid;
        TankData _accessData;
        AudioSource _audioControl;

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

        bool _isDestroyed;

		public Action OnTankDestroyed;

		void Awake()
		{
			_rigid = GetComponent<Rigidbody>();
			_audioControl = GetComponent<AudioSource>();

			_accessData = new TankData();
			_tanksInSensor = new List<TankMotor>();

			_currentHealth = _startHealth;
		}

		void Start()
        {
            _previousPosition = transform.position;
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

            _audioControl.PlayOneShot(_damagedSound);

            if (_currentHealth <= 0f)
                DestroyRobot();
        }

        //Function that is called when health reaches zero. Shows an explosion and sets the UI to 'defeated'
        private void DestroyRobot()
        {
			if (_isDestroyed)
				return;

			if (OnTankDestroyed != null)
				OnTankDestroyed.Invoke();

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
            _tanksInSensor.RemoveAll(Tank => Tank == null);

			TankData[] sensorData = new TankData[_tanksInSensor.Count];
			for (int i = 0; i < _tanksInSensor.Count; i++)
			{
				sensorData[i] = _tanksInSensor[i].GetTankData();
			}

            return sensorData;
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

				//TODO: Create bulletparent
                GameObject newBullet = Instantiate(_bulletPrefabs[bulletType], BattleManager.Singleton().GetBulletContainer());
				Transform bulletTransform = newBullet.transform;
				bulletTransform.position = _gunTransform.position + (_gunTransform.rotation * _bulletSpawnOffset);
				bulletTransform.rotation = _gunTransform.rotation;


				newBullet.GetComponent<BulletBehaviour>().SetShooter(this);

                _audioControl.PlayOneShot(_shootSound);
            }
        }

		public void SetBodyColor(Color color)
		{
			//_bodyCanvas.material.color = color;
			_bodyCanvas.ChangeColor(color);
		}

		public void SetTurretColor(Color color)
		{
			_turretCanvas.ChangeColor(color);
			//_turretCanvas.material.color = color;
		}

        private void OnTriggerEnter(Collider col)
        {
            if (col.isTrigger)
                return;

			_tanksInSensor.RemoveAll(RobotData => RobotData == null);

			TankMotor motor = col.GetComponent<TankMotor>();

			if (motor != null && !_tanksInSensor.Contains(motor))
				_tanksInSensor.Add(motor);
        }

        private void OnTriggerExit(Collider col)
        {
			if (col.isTrigger)
				return;

			_tanksInSensor.RemoveAll(RobotData => RobotData == null);

			TankMotor motor = col.GetComponent<TankMotor>();

			if (motor != null && _tanksInSensor.Contains(motor))
				_tanksInSensor.Remove(motor);
		}

        //Function for collisions, sends messages to RobotControl and all derived classes
        private void OnCollisionEnter(Collision col)
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
			SetTankAngle(_accessData.TankAngle);
			SetGunAngle(_accessData.GunAngle);
			SetSensorAngle(_accessData.SensorAngle);
			SetMovePower(0.0f);
		}

		public TankData GetTankData()
		{
			return _accessData.RetreiveData();
		}
    }
}