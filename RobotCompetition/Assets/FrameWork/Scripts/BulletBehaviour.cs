using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

namespace Framework
{
    [RequireComponent(typeof(Rigidbody))]
    public class BulletBehaviour : MonoBehaviour
    {

        //Variables that hold references to components
        private Rigidbody rigid;
        private Transform shooter;

        //Variables that determine behaviour
        private float damage = 10f;
        private float flyingSpeed = 10f;

        // a bool that is set to false when it hits something. Used to visualise the explosion 
        private bool isFunctional = true;

        //Assigns variables
        void Start()
        {
            rigid = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void Update()
        {
            //Setting the start velocity when the bullet is spawned, this just ensures the velocity stays constant
            if (isFunctional)
                rigid.velocity = rigid.velocity.normalized * flyingSpeed;
        }

        public void SetShooter(Transform newShooter)
        {
            //Sets the shooter so the bullet does not hit itself
            shooter = newShooter;
        }

        //Collision check
        private void OnTriggerEnter(Collider col)
        {
            //If the other collider is a trigger, do nothing
            if (col.isTrigger || col.transform == shooter)
            {
                return;
            }

            if (col.tag == "Tank")
            {
                //If the object is a tank do damage
                col.transform.GetComponent<RobotMotor>().DamageRobot(damage);
                col.gameObject.SendMessage("OnBulletCollision");
            }
            //Destroy this object when it collided with something
            Explode();
        }

        // visualize the explosion of the bullet & remove the functionality of the bullet.
        private void Explode()
        {
			rigid.velocity = Vector3.zero;
			rigid.isKinematic = true;
            GetComponent<CapsuleCollider>().enabled = false;
            GetComponent<MeshRenderer>().enabled = false;
            transform.GetChild(0).gameObject.SetActive(true);
            isFunctional = false;
            Destroy(gameObject, 1);
        }
    }
}