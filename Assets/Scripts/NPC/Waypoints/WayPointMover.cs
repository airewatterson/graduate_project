using System;
using UnityEngine;

namespace NPC.Waypoints
{
    public class WayPointMover : MonoBehaviour
    {
        [SerializeField] private WayPoints waypoints;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float distanceThreshold = 0.1f;

        private Transform _currentWayPoint;

        private Enemy _enemy;

        private void Start()
        {
            _enemy = GetComponent<Enemy>();
            _currentWayPoint = waypoints.GetNextWaypoint(_currentWayPoint);
            transform.position = _currentWayPoint.position;
            
            _currentWayPoint = waypoints.GetNextWaypoint(_currentWayPoint);
            transform.LookAt(_currentWayPoint);
        }

        private void Update()
        {
            if (_enemy.enemyHp<=0)
            {
                moveSpeed = 0;
                return;
            }
            transform.position = Vector3.MoveTowards(transform.position, _currentWayPoint.position, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position,_currentWayPoint.position) < distanceThreshold)
            {
                _enemy.enemyAnimator.SetBool("isRunning",true);
                _currentWayPoint = waypoints.GetNextWaypoint(_currentWayPoint);
                transform.LookAt(_currentWayPoint);
            }
        }
    }
}
