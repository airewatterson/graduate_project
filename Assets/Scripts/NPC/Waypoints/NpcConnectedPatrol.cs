using UnityEngine;
using UnityEngine.AI;

namespace NPC.Waypoints
{
    public class NpcConnectedPatrol : MonoBehaviour
    {
        //NPC是否在點上等待？
        [SerializeField] private bool patrolWaiting;
        //點上等待多久？
        [SerializeField] private float totalWaitTime = 60f;

        //NPC基礎數值
        private NavMeshAgent _navMeshAgent;
        private ConnectedWaypoint _currentWayPoint;
        private ConnectedWaypoint _previousWayPoint;

        private int _currentPatrolIndex;
        private bool _travelling;
        private bool _waiting;
        private bool _patrolForward;
        private float _waitTimer;
        private int _wayPointsVisited;
        

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
                if (_currentWayPoint == null)
                {
                    //隨機抓取編輯器內所有等待點
                    var allWayPoints = GameObject.FindGameObjectsWithTag("WayPoints");

                    if (allWayPoints.Length>0)
                    {
                        while (_currentWayPoint == null)
                        {
                            int random = Random.Range(0, allWayPoints.Length);
                            ConnectedWaypoint startingWayPoint = allWayPoints[random].GetComponent<ConnectedWaypoint>();
                            
                            //如果有找到
                            if (startingWayPoint != null)
                            {
                                _currentWayPoint = startingWayPoint;
                            }
                        }
                    }
                    
                }
                else
                {
                    Debug.LogError("Failed to find any way points for use in the scene.");
                }
            }
            SetDestination();
        }

        // Update is called once per frame
        private void Update()
        {
            //是否接近等待點？
            if (_travelling && _navMeshAgent.remainingDistance <= 1.0f)
            {
                _travelling = false;
                _wayPointsVisited++;
                //若是該點為等待點，則等待時間
                if (patrolWaiting)
                {
                    _waiting = true;
                    _waitTimer = 0f;
                }
                else
                {
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
                    
                    SetDestination();
                }
            }
        }

        private void SetDestination()
        {
            if (_wayPointsVisited > 0)
            {
                var nextWayPoint = _currentWayPoint.NextWayPoint(_previousWayPoint);
                _previousWayPoint = _currentWayPoint;
                _currentWayPoint = nextWayPoint;
            }

            Vector3 targetVector = _currentWayPoint.transform.position;
            _navMeshAgent.SetDestination(targetVector);
            _travelling = true;
        }
        
        
    }
}
