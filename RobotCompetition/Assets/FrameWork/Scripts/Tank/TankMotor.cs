using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Framework;

namespace Framework
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(RobotData))]
    public class TankMotor : MonoBehaviour
    {

		[Header("TankParts")]
		[SerializeField] Transform _gunTransform;
		[SerializeField] Transform _sensorTransform;

		//Reference to all spawnable bullet prefabs
		[SerializeField] private GameObject[] _bulletPrefabs = new GameObject[0];

        //Reference to the shoot sound
        [SerializeField] private AudioClip _shootSound;

        //Reference to the hit sound
        [SerializeField] private AudioClip _damagedSound;

        //Reference to required components and objects
        private Rigidbody _rigid;
        private TankData _data;
        private AudioSource _audioControl;
		private List<TankMotor> _tanksInSensor;

        public bool invinsible = false;

        //Collection of variables
        public bool hasHealthUI; // this is needed to prevent a lot of errors connected to the UI
        public Transform playerUI;
        private Text playerNameText;
        private Slider healthSlider;
        private Slider redPartSlider;
        private float redSliderDelay = 0f;
        private float redSliderAmount = 0f;

        private float maxHealth = 100f;
        private float currentHealth = 0f;
        private float gunCooldown = 0f;
        private float gunCoolTime = 0.5f;

        private readonly float rotationSpeed = 45f;
        private readonly float maxMoveSpeed = 3f;

        private float moveSpeed = 0f;
        private float targetRotation = 0f;
        private float targetGunRotation = 0f;
        private float targetSensorRotation = 0f;

        private Vector3 prevPos;
        private float calcSpeed;

        private bool isDestroyed;

        public UnityEvent onDestroy = new UnityEvent();

		//Assign all components needed
		void Awake()
		{
			_rigid = GetComponent<Rigidbody>();
			_audioControl = GetComponent<AudioSource>();

			_data = new TankData();
			_tanksInSensor = new List<TankMotor>();
		}

		//Assign all components needed
		void Start()
        {
            prevPos = transform.position;
            currentHealth = maxHealth;

            if (hasHealthUI)
            {
                healthSlider = playerUI.GetChild(1).GetComponent<Slider>();
                redPartSlider = playerUI.GetChild(0).GetComponent<Slider>();
                playerNameText = playerUI.GetChild(2).GetComponent<Text>();

                healthSlider.value = redPartSlider.value = redSliderAmount = maxHealth;
                string UIname = name.Replace("_", " ");
                playerNameText.text = UIname;
            }
        }

        //Apply all the changes and update the robot data
        void Update()
        {
			if (isDestroyed)
				return;

            _tanksInSensor.RemoveAll(RobotData => RobotData == null);

            if (gunCooldown > 0f)
				gunCooldown -= Time.deltaTime;

            ApplyInput();

			_data.Update(this);

            ReduceRedSlider();
        }

        //Calculate true velocity
        private void FixedUpdate()
        {
            Vector3 calcVel = (transform.position - prevPos) / Time.fixedDeltaTime;
            prevPos = transform.position;
			calcSpeed = calcVel.magnitude;
            //calcSpeed = transform.InverseTransformDirection(calcVel).z;
        }

        //Apply changes to the robot based on the input
        private void ApplyInput()
        {
            //prevent child rotation on the object
            Quaternion prevGunRot = _gunTransform.rotation;
            Quaternion preSensorRot = _sensorTransform.rotation;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0f, targetRotation, 0f), Time.deltaTime * rotationSpeed);
            _gunTransform.rotation = prevGunRot;
            _sensorTransform.rotation = preSensorRot;

            _rigid.velocity = transform.forward * moveSpeed;
            _gunTransform.rotation = Quaternion.RotateTowards(_gunTransform.rotation, Quaternion.Euler(0f, targetGunRotation, 0f), Time.deltaTime * rotationSpeed);
            _sensorTransform.rotation = Quaternion.RotateTowards(_sensorTransform.rotation, Quaternion.Euler(0f, targetSensorRotation, 0f), Time.deltaTime * rotationSpeed);
        }

        //Damage this robot, called when hit by a bullet
        public void DamageRobot(float amount)
        {
            currentHealth -= amount;
            _audioControl.PlayOneShot(_damagedSound);

            if (currentHealth <= 0f && !invinsible)
                DestroyRobot();

            if (hasHealthUI)
            {
                redSliderDelay = 0.8f;
                healthSlider.value = currentHealth;
            }
        }

        //Reduce the red part of the healthbar after a delay
        private void ReduceRedSlider()
        {
            if (hasHealthUI)
            {
                if (redSliderDelay > 0)
                    redSliderDelay -= Time.deltaTime;

                if (redSliderAmount != healthSlider.value && redSliderDelay <= 0)
                    redSliderAmount = healthSlider.value;

                if (redSliderAmount < redPartSlider.value)
                    redPartSlider.value -= 20 * Time.deltaTime;
            }
        }

        //Function that is called when health reaches zero. Shows an explosion and sets the UI to 'defeated'
        private void DestroyRobot()
        {
            isDestroyed = true;
            string[] defeatString = new string[5] { " got destroyed!", " was defeated!", " got annihilated!", " perished!", " was slain!" };

            // make the robot invisible and stop functionality, and animate the explosion
            GetComponent<BoxCollider>().enabled = false;
            for (int i = 0; i < 3; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            if (hasHealthUI)
            {
                Destroy(healthSlider.gameObject);
                Destroy(redPartSlider.gameObject);
                playerNameText.text = name + defeatString[Random.Range(0, defeatString.Length)];
            }
            transform.GetChild(3).gameObject.SetActive(true);
            Destroy(gameObject, 1);
        }

        //Functions that are accessable by the robotControl base class

        //Set the movement speed
        public void MoveRobot(float movePower)
        {
			//set min to -0.5f to make it move slower when backing up
			movePower = Mathf.Clamp(movePower, -1.0f, 1.0f);

			//TODO: Do this calculation in movement part?
            moveSpeed = maxMoveSpeed * movePower;
        }

        //Set the target robot rotation
        public void RotateRobot(float targetAngle)
        {
            targetRotation = targetAngle;
        }

        //Set the target gun rotation
        public void RotateGun(float targetAngle)
        {
            targetGunRotation = targetAngle;
        }

        //Set the target sensor rotation
        public void RotateSensor(float targetAngle)
        {
            targetSensorRotation = targetAngle;
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

        //Shoot a bulletType if the gun is not on cooldown
        public void Shoot(int bulletType)
        {
            if (gunCooldown <= 0f)
            {
                gunCooldown = gunCoolTime;

				//TODO: Make actual bulletpoint in hierarchy?
                GameObject newBullet = Instantiate(_bulletPrefabs[bulletType], _gunTransform.position + new Vector3(0, 1.5f, 0), _gunTransform.rotation);

                newBullet.GetComponent<BulletBehaviour>().SetShooter(transform);

				// TODO: Do we want this physics based?
                newBullet.GetComponent<Rigidbody>().velocity = newBullet.transform.forward * 7f;

                _audioControl.PlayOneShot(_shootSound);
            }
        }

        //Functions for the sensor, sensor has to be the only trigger on the robot for this to work correctly
        private void OnTriggerEnter(Collider col)
        {
            if (col.isTrigger)
                return;

			TankMotor motor = col.GetComponent<TankMotor>();

			if (motor != null && !_tanksInSensor.Contains(motor))
			{
				_tanksInSensor.Add(motor);
			}
        }

        private void OnTriggerExit(Collider col)
        {
			if (col.isTrigger)
				return;

			TankMotor motor = col.GetComponent<TankMotor>();

			if (motor != null && _tanksInSensor.Contains(motor))
			{
				_tanksInSensor.Remove(motor);
			}
		}

        //Function for collisions, sends messages to RobotControl and all derived classes
        private void OnCollisionEnter(Collision col)
        {
            if (col.transform.tag == "Wall")
            {
                gameObject.SendMessage("OnWallCollision");
            }
            else if (col.transform.tag == "Tank")
            {
                gameObject.SendMessage("OnTankCollision");
            }
        }

		public TankData GetTankData()
		{
			return _data;
		}

		public float GetHealth()
		{
			return currentHealth;
		}

		public Vector3 GetPosition()
		{
			return transform.position;
		}

		public float GetCalculatedMoveSpeed()
		{
			return calcSpeed;
		}

		public float GetTankAngle()
		{
			return transform.eulerAngles.y;
		}

		public float GetGunAngle()
		{
			return _gunTransform.eulerAngles.y;
		}

		public float GetSensorAngle()
		{
			return _sensorTransform.eulerAngles.y;
		}

        private void OnDestroy()
        {
            currentHealth = -1;
            onDestroy.Invoke();
        }
    }
}