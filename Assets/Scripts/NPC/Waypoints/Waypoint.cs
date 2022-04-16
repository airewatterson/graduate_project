using System;
using General;
using UnityEngine;

namespace NPC.Waypoints
{
    public class Waypoint : SingletonMonoBehavior<Waypoint>
    {
        [SerializeField] protected float debugDrawRadius;

        public virtual void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position ,debugDrawRadius);
        }
    }
}
