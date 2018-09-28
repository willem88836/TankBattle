using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

namespace Framework
{
    [RequireComponent(typeof(Rigidbody))]
    public class BulletBehaviour : MonoBehaviour
    {
		[SerializeField] float _damage = 10f;
		[SerializeField] float _flyingSpeed = 10f;

		[Header("Visuals")]
		[SerializeField] GameObject _explodeVisual;
		[SerializeField] GameObject _bulletVisual;

		TankMotor _shooter;

        bool _hasHit = false;

		void Awake()
		{
			_hasHit = false;
		}

        void Update()
        {
			if (_hasHit)
				return;

			// Move bullet forward
			transform.position += transform.forward * _flyingSpeed * Time.deltaTime;
        }

        public void SetShooter(TankMotor newShooter)
        {
            _shooter = newShooter;
        }

        //Collision check
        private void OnTriggerEnter(Collider col)
        {
            if (col.isTrigger)
                return;

			TankMotor tank = col.GetComponent<TankMotor>();

			if (tank != null && tank != _shooter)
			{
				tank.Damage(_damage);
				tank.gameObject.SendMessage("OnBulletCollision");

				Explode();
				return;
			}

			ArenaWall wall = col.GetComponent<ArenaWall>();

			if (wall != null)
				Explode();
        }

        // visualize the explosion of the bullet & remove the functionality of the bullet.
        private void Explode()
        {
			if (_hasHit)
				return;

			_hasHit = true;

			_explodeVisual.SetActive(true);
			_bulletVisual.SetActive(false);

			// Destroy object after 1 sec
            Destroy(gameObject, 1);
        }
    }
}