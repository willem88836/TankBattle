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
    public class RobotMotor : MonoBehaviour
    {

        //Reference to all spawnable bullet prefabs
        [SerializeField]
        private GameObject[] bulletPrefabs = new GameObject[0];

        //Reference to the shoot sound
        [SerializeField]
        private AudioClip shootSound;

        //Reference to the hit sound
        [SerializeField]
        private AudioClip hitSound;

        //Reference to required components and objects
        private Rigidbody rigid;
        private RobotData data;
        private AudioSource audioControl;
        private Transform gun;
        private Transform sensor;
        private List<RobotData> sensorTanks = new List<RobotData>();

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
        public float GetCurrentHealth { get { return currentHealth; } }
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
        void Start()
        {
            rigid = GetComponent<Rigidbody>();
            data = GetComponent<RobotData>();
            audioControl = GetComponent<AudioSource>();
            gun = transform.GetChild(0);
            sensor = transform.GetChild(1);

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
            sensorTanks.RemoveAll(RobotData => RobotData == null);

            if (gunCooldown > 0f)
            {
                gunCooldown = Mathf.Clamp(gunCooldown - Time.deltaTime, 0, gunCoolTime);
            }

            if (!isDestroyed)
                ApplyInput();

            ReduceRedSlider();
        }

        //Calculate true velocity
        private void FixedUpdate()
        {
            Vector3 calcVel = ((transform.position - prevPos) / Time.fixedDeltaTime);
            prevPos = transform.position;
            calcSpeed = transform.InverseTransformDirection(calcVel).z;
        }

        //Apply changes to the robot based on the input
        private void ApplyInput()
        {
            //prevent child rotation on the object
            Quaternion prevGunRot = gun.rotation;
            Quaternion preSensorRot = sensor.rotation;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0f, targetRotation, 0f), Time.deltaTime * rotationSpeed);
            gun.rotation = prevGunRot;
            sensor.rotation = preSensorRot;

            rigid.velocity = transform.forward * moveSpeed;
            gun.rotation = Quaternion.RotateTowards(gun.rotation, Quaternion.Euler(0f, targetGunRotation, 0f), Time.deltaTime * rotationSpeed);
            sensor.rotation = Quaternion.RotateTowards(sensor.rotation, Quaternion.Euler(0f, targetSensorRotation, 0f), Time.deltaTime * rotationSpeed);

            data.UpdateData(currentHealth, transform.position, calcSpeed, transform.eulerAngles.y, gun.eulerAngles.y, sensor.eulerAngles.y);
        }

        //Damage this robot, called when hit by a bullet
        public void DamageRobot(float amount)
        {
            currentHealth -= amount;
            audioControl.PlayOneShot(hitSound);
            //Debug.Log(name + " health: " + currentHealth);
            if (currentHealth <= 0f && !invinsible)
            {
                DestroyRobot();
            }
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
            moveSpeed = maxMoveSpeed * Mathf.Clamp(movePower, -1f, 1f);
        }

        //Set the target robot rotation
        public void RotateRobot(float degree)
        {
            targetRotation = degree;
        }

        //Set the target gun rotation
        public void RotateGun(float degree)
        {
            targetGunRotation = degree;
        }

        //Set the target sensor rotation
        public void RotateSensor(float degree)
        {
            targetSensorRotation = degree;
        }

        //Retreives all data from tanks in the sensor, returns an AccesData array
        public AccessData[] FindTanks()
        {
            sensorTanks.RemoveAll(RobotData => RobotData == null);
            AccessData[] tempData = new AccessData[sensorTanks.Count];
            Vector3 pos = transform.position;
            for (int i = 0; i < tempData.Length; i++)
            {
                tempData[i] = sensorTanks[i].GetData(pos);
            }
            return tempData;
        }

        //Shoot a bulletType if the gun is not on cooldown
        public void Shoot(int bulletType)
        {
            if (gunCooldown <= 0f)
            {
                gunCooldown = gunCoolTime;
                GameObject newBullet = Instantiate(bulletPrefabs[bulletType], gun.position + new Vector3(0, 1.5f, 0), gun.rotation);
                newBullet.GetComponent<BulletBehaviour>().SetShooter(transform);
                newBullet.GetComponent<Rigidbody>().velocity = newBullet.transform.forward * 7f;
                audioControl.PlayOneShot(shootSound);
            }
        }

        //Functions for the sensor, sensor has to be the only trigger on the robot for this to work correctly
        private void OnTriggerEnter(Collider col)
        {
            if (col.isTrigger)
            {
                return;
            }
            if (col.tag == "Tank" && col.transform != transform)
            {
                if (!sensorTanks.Contains(col.GetComponent<RobotData>()))
                {
                    sensorTanks.Add(col.GetComponent<RobotData>());
                }
            }
        }

        private void OnTriggerExit(Collider col)
        {
            if (col.isTrigger)
            {
                return;
            }
            if (col.tag == "Tank" && col.transform != transform)
            {
                if (sensorTanks.Contains(col.GetComponent<RobotData>()))
                {
                    sensorTanks.Remove(col.GetComponent<RobotData>());
                }
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

        private void OnDestroy()
        {
            currentHealth = -1;
            onDestroy.Invoke();
        }
    }
}