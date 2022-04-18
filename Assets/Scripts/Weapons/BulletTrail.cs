using System;
using CodeMonkey.Utils;
using DamageSys;
using General;
using NPC;
using UnityEngine;

namespace Weapons
{
    public class BulletTrail : SingletonMonoBehavior<BulletTrail>
    {
        private Vector3 _shootDir;

        public float speed = 100f;

        public Rigidbody rigidBody;
        
        
        
        private void Start()
        {
            rigidBody = GetComponent<Rigidbody>();
            rigidBody.velocity += _shootDir * speed * Time.deltaTime;
        }

        private void OnEnable()
        {
            //Vector3 relativePos = _target.position - transform.position;
            var shootDir = new Vector3();
            _shootDir = shootDir; 
            transform.eulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(shootDir));
        }

        private void OnTriggerEnter(Collider other)
        {
            var hittable = other.gameObject.GetComponent<IDamageable>();
            
            if (other.transform.CompareTag("Room"))
            {
                Destroy(gameObject);
            }

            if (other != null && !other.CompareTag("Bullet"))
            {
                hittable.ReceiveDamage(other);
            }
        }

       /* private void Update()
        {
            float moveSpeed = 100f;
            transform.position += _shootDir * moveSpeed * Time.deltaTime;
        }*/

        
    }
}
