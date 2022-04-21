using UnityEngine;
using UnityEngine.AI;

namespace NPC.Waypoints
{
    public class MoveTrap : MonoBehaviour
    {
        private NavMeshAgent _agent;
        public Transform[] waypoint;
        private int _pointIndex;
        private Vector3 _target;
        private Enemy _enemy;
        private void Start()
        {
            _enemy = GetComponent<Enemy>();
            _agent = GetComponent<NavMeshAgent>();
            Destination();
        }

        // Update is called once per frame
        private void Update()
        {
            if (_enemy.enemyHp<=0)
            {
                _agent.speed = 0;
            }
            else
            {
                _agent.speed = 2.25f;
                _enemy.enemyAnimator.SetBool("isRunning",true);
            }
            
            
            if (Vector3.Distance(transform.position, _target) < 1)
            {
                IteratePointIndex();
                Destination();
                
            }
        }

        private void Destination()
        {
            _target = waypoint[_pointIndex].position;
            _agent.SetDestination(_target);
        }

        private void IteratePointIndex()
        {
            _pointIndex++;
            if (_pointIndex == waypoint.Length)
            {
                _pointIndex = 0;
            }
        }







    }
}
