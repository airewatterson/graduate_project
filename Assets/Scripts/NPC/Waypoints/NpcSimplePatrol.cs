using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace NPC.Waypoints
{
    public class NpcSimplePatrol : MonoBehaviour
    {
        //NPC是否在點上等待？
        [SerializeField] private bool patrolWaiting;
        //點上等待多久？
        [SerializeField] private float totalWaitTime = 3f;
        //可能會轉向哪個方向？
        [SerializeField] private float switchProbability = 0.2f;
        //指定等待點
        [SerializeField] private List<Waypoint> patrolPoints;
        
        
        //NPC基礎數值
        private NavMeshAgent _navMeshAgent;
        private int _currentPatrolIndex;
        private bool _travelling;
        private bool _waiting;
        private bool _patrolForward;
        private float _waitTimer;
        

        // Start is called before the first frame update
        private void Start()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();

            if (_navMeshAgent == null)
            {
                Debug.Log("The NavMeshAgent is not attached to:" + gameObject.name);
            }
            else
            {
                if (patrolPoints != null && patrolPoints.Count>=2)
                {
                    _currentPatrolIndex = 0;
                    SetDestination();
                }
                else
                {
                    Debug.Log("Insufficient patrol points for basic patrolling behaviour.");
                }
            }
        }

        // Update is called once per frame
        private void Update()
        {
            //是否接近等待點？
            if (_travelling && _navMeshAgent.remainingDistance <= 1.0f)
            {
                _travelling = false;
                //若是該點為等待點，則等待時間
                if (patrolWaiting)
                {
                    _waiting = true;
                    _waitTimer = 0f;
                }
                else
                {
                    ChangePatrolPoint();
                    SetDestination();
                }
            }
            
            //若是正在等待
            if (_waiting)
            {
                _waitTimer += Time.deltaTime;
                if (_waitTimer >= totalWaitTime)
                {
                    _waiting = false;
                    
                    ChangePatrolPoint();
                    SetDestination();
                }
            }
        }

        private void SetDestination()
        {
            if (patrolPoints != null)
            {
                var targetVector = patrolPoints[_currentPatrolIndex].transform.position;
                _navMeshAgent.SetDestination(targetVector);
                _travelling = true;
            }
        }
        
        
        //更換移動點，同時具有可能向前或是向後移動的功能
        private void ChangePatrolPoint()
        {
            if (Random.Range(0f,1f) <= switchProbability)
            {
                _patrolForward = !_patrolForward;
            }

            if (_patrolForward)
            {
                _currentPatrolIndex = (_currentPatrolIndex + 1) % patrolPoints.Count;
            }
            else
            {
                if (--_currentPatrolIndex < 0)
                {
                    _currentPatrolIndex = patrolPoints.Count - 1;
                }
            }
        }
    }
}
