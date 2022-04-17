using UnityEngine;

namespace DamageSys
{
    public interface IDamageable
    {
        public void ReceiveDamage(RaycastHit hit, Collider collider);
    }
}   