using System;
using System.Runtime.CompilerServices;
using DamageSys;
using General;
using NPC.Sight.Scripts;
using NPC.Waypoints;
using Photon.Pun;
using Player.Input;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace NPC
{
    [RequireComponent(typeof(NavMeshAgent),typeof(FieldOfView),typeof(MoveTrap))]
    [RequireComponent(typeof(PhotonView),typeof(PhotonTransformView))]
    public class Enemy : MonoBehaviour, IDamageable
    {

        
        
        private NavMeshAgent _navMeshAgent;

        //Attack參數
        public Transform gunPoint; 
        public GameObject bullet; 
        public float weaponRange = 10f; 
        public  Animator muzzleFlashAnimator;
        private IDamageable _damageableImplementation;
        
        //Enemy生命參數
        public int enemyHp = 1;
        public Animator enemyAnimator;


        public void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }


        private void Update()
        {
            if (enemyHp > 0) return;
            //CameraShake.Instance.ShakeCamera(5f,.1f);
            enemyAnimator.SetBool("isDead",true);
            Invoke(nameof(DisableEnemy),3);
        }

        public void ReceiveDamage(Collider hit)
        {
            Debug.LogError("Get Hit!");
        }

        private void DisableEnemy()
        {
            gameObject.SetActive(false);
        }
        
        private void Revive()
        {
            enemyAnimator.SetBool("isDead",false);
            enemyHp = 1;
            gameObject.SetActive(true);
        }
        
        public void TakeDamage(int damage)
        {

            enemyHp -= damage;
            Debug.Log(enemyHp);
            if(enemyHp <= 0)
            {
                enemyAnimator.SetBool("isDead",true);
                Invoke(nameof(DisableEnemy), 3);
            }
        }
    }
}
