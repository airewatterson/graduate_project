using UnityEngine;

namespace DamageSys
{
    public interface IDamageable
    {
        public void ReceiveDamage(Collider hit);
    }
}   