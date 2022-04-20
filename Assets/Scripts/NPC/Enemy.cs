using System;
using System.Runtime.CompilerServices;
using DamageSys;
using General;
using NPC.Waypoints;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace NPC
{
    [RequireComponent(typeof(NavMeshAgent), typeof(FiniteStateMachine))]
    public class Enemy : MonoBehaviour, IDamageable
    {
        [FormerlySerializedAs("_patrolPoints")] [SerializeField] private ConnectedWaypoint[] patrolPoints;
        
        
        private NavMeshAgent _navMeshAgent;
        private FiniteStateMachine _finiteStateMachine;
        
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
            _finiteStateMachine = GetComponent<FiniteStateMachine>();
        }
        
        public ConnectedWaypoint[] PatrolPoints => patrolPoints;


        private void Update()
        {
            if (enemyHp > 0) return;
            enemyAnimator.SetBool("isDead",true);
            Invoke(nameof(DisableEnemy),3);
            Invoke(nameof(Revive),10);
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
                Invoke(nameof(Revive),10);
            }
        }
    }
}
