using General;
using UnityEngine;

namespace Player.Input
{
    public class AnimationEvents : SingletonMonoBehavior<AnimationEvents>
    {
        [SerializeField] private Player player;
        private Animator _animator;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            player = GetComponentInParent<Player>();
        }

        public void AnimationRevert()
        {
            player.isAttacking = false;
            _animator.SetBool("isPunching",false);
        }

        public void Damage(Collider other)
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                Debug.Log("Hurt");
            }
        }
    }
}
