using System;
using System.Runtime.CompilerServices;
using NPC.Waypoints;
using UnityEngine;
using UnityEngine.AI;

namespace NPC
{
    [RequireComponent(typeof(NavMeshAgent), typeof(FiniteStateMachine))]
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private ConnectedWaypoint[] _patrolPoints;
        
        
        private NavMeshAgent _navMeshAgent;
        private FiniteStateMachine _finiteStateMachine;
        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _finiteStateMachine = GetComponent<FiniteStateMachine>();
        }
        
        public ConnectedWaypoint[] PatrolPoints => _patrolPoints;
    }
}
