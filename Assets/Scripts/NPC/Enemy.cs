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
        public float speed;
        public Transform gunPoint; 
        public GameObject bullet; 
        public float weaponRange = 10f; 
        public  Animator muzzleFlashAnimator;
        private IDamageable _damageableImplementation;


        public void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _finiteStateMachine = GetComponent<FiniteStateMachine>();
        }
        
        public ConnectedWaypoint[] PatrolPoints => patrolPoints;

        

        public void ReceiveDamage(RaycastHit2D hit)
        {
            //GetDamage(hit);
            
        }

        public void ReceiveDamage(Collider hit)
        {
            Debug.LogError("Get Hit!");
        }


        
    }
}
