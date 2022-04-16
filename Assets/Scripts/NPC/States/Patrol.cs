using System;
using NPC.Waypoints;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NPC.States
{
    [CreateAssetMenu(fileName = "PatrolState", menuName = "EnemyStates/Patrol", order = 2)]
    public class Patrol : AbstractState
    {
        private ConnectedWaypoint[] _patrolPoints;
        private int _patrolPointIndex;

        public override void OnEnable()
        {
            base.OnEnable();
            StateType = FSMStateType.Patrol;
            _patrolPointIndex = -1;
        }

        public override bool EnterState()
        {
            EnteredState = false;
            if (base.EnterState())
            {
                //等待點抓取
                _patrolPoints = _enemy.PatrolPoints;

                if (_patrolPoints == null || _patrolPoints.Length == 0)
                {
                    Debug.LogError("PatrolState: Failed to grab patrol points from Enemy.");
                   
                }
                else
                {
                    if (_patrolPointIndex< 0)
                    {
                        _patrolPointIndex = Random.Range(0, _patrolPoints.Length);
                    }
                    else
                    {
                        _patrolPointIndex = (_patrolPointIndex + 1) % _patrolPoints.Length;
                    }
                    
                    SetDestination(_patrolPoints[_patrolPointIndex]);
                    EnteredState = true;
                }
            }

            return EnteredState;
        }
        
        public override void UpdateState()
        {
            if (EnteredState)
            {
                if (Vector3.Distance(_navMeshAgent.transform.position, _patrolPoints[_patrolPointIndex].transform.position) <= 1f)
                {
                    _fsm.EnterState(FSMStateType.Idle);
                }
            }
        }
        
        
        private void SetDestination(ConnectedWaypoint destination)
        {
            if (_navMeshAgent != null && destination != null)
            {
                _navMeshAgent.SetDestination(destination.transform.position);
            }
        }
    }
}