using System;
using NPC.Waypoints;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace NPC.States
{
    [CreateAssetMenu(fileName = "PatrolState", menuName = "EnemyStates/Patrol", order = 2)]
    public class Patrol : AbstractState
    {
        [SerializeField] private Material fovVisible;
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
                _patrolPoints = Enemy.PatrolPoints;

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
                    //fov
                    NavMeshAgent.speed = 2.25f;
                    fovVisible.color = Color.white;
                    Debug.Log("now finding player");
                    
                    
                    SetDestination(_patrolPoints[_patrolPointIndex]);
                    Animator.SetBool("isRunning",true);
                    EnteredState = true;
                }
            }

            return EnteredState;
        }
        
        public override void UpdateState()
        {
            if (EnteredState)
            {
                if (Vector3.Distance(NavMeshAgent.transform.position, _patrolPoints[_patrolPointIndex].transform.position) <= 1f)
                {
                    Animator.SetBool("isRunning",false);
                    Fsm.EnterState(FSMStateType.Idle);
                }
                if (FOV.findPlayer)
                {
                    Fsm.EnterState(FSMStateType.Attack);
                }
            }
        }
        
        
        private void SetDestination(ConnectedWaypoint destination)
        {
            if (NavMeshAgent != null && destination != null)
            {
                NavMeshAgent.SetDestination(destination.transform.position);
            }
        }
    }
}