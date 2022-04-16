using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace NPC.Waypoints
{
    public class ConnectedWaypoint : Waypoint
    {
        [SerializeField] protected float connectivityRadius = 50f;

        private List<ConnectedWaypoint> _connections;

        // Start is called before the first frame update
        private void Start()
        {
            //獲取所有等待點清單
            var allWayPoints = GameObject.FindGameObjectsWithTag("WayPoints");

            _connections = new List<ConnectedWaypoint>();
            
            //檢查等待點之間是否連結
            foreach (var t in allWayPoints)
            {
                var nextWayPoint = t.GetComponent<ConnectedWaypoint>();
                
                //舉例，若是找到了等待點
                if (nextWayPoint == null) continue;
                if (Vector3.Distance(transform.position, nextWayPoint.transform.position) <= connectivityRadius && nextWayPoint != this)
                {
                    _connections.Add(nextWayPoint);
                }
            }
        }

        //顯示等待點，取代Ｗａｙｐｏｉｎｔ代碼在編輯器內顯示的內容
        public override void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            var position = transform.position;
            Gizmos.DrawSphere(position,debugDrawRadius);
            
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(position, connectivityRadius);
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public ConnectedWaypoint NextWayPoint(ConnectedWaypoint previousWaypoint)
        {
            if (_connections.Count == 0)
            {
                //找不到下一個等待點時顯示
                Debug.LogError("Insufficient Waypoint Count");
                return null;
            }
            else if (_connections.Count == 1 && _connections.Contains(previousWaypoint))
            {
                //只有一個顯示點時回覆
                return previousWaypoint;
            }
            //如果上述兩種情況未發生，尋找隨機等待點，同時他不是前一個等待點
            else
            {
                ConnectedWaypoint nextWaypoint;
                int nextIndex = 0;

                do
                {
                    nextIndex = UnityEngine.Random.Range(0, _connections.Count);
                    nextWaypoint = _connections[nextIndex];
                } while (nextWaypoint == previousWaypoint);

                return nextWaypoint;
            }
            
        }
    }
}
