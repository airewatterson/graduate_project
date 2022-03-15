using System.Linq.Expressions;
using General;
using UnityEngine;
using UnityEngine.Serialization;

namespace Input.Attack
{
    public class Attack : SingletonMonoBehavior<Attack>
    {


        [SerializeField]private Animator ani;
        [SerializeField]private Transform atkPoint;
        [SerializeField]private float atkRange = 1f;
        [FormerlySerializedAs("EnemyLayer")] public LayerMask enemyLayer;

        [SerializeField]private KeyCode attackKey;
        private static readonly int IsPunching = Animator.StringToHash("isPunching");


        // Update is called once per frame
        private void Update()
        {
            if (!UnityEngine.Input.GetKeyDown(attackKey)) return;
            PlayerAttack();
            FindObjectOfType<AudioManager>().Play("Punch");
            Invoke(nameof(LateCancelAnimation),1);
        }
        private void PlayerAttack()
        {
            ani.SetBool(IsPunching,true);
            var hit= Physics.OverlapSphere(atkPoint.position, atkRange, enemyLayer);

            foreach(var enemy in hit)
            {
                Debug.Log("hit" + enemy.name);
                var police = enemy.GetComponent<PoliceAI>();
                var player1 = enemy.gameObject.GetComponent<Player1>();
                var player2 = enemy.gameObject.GetComponent<Player2>();
                if(police != null)
                {
                    police.TakeDamage(1);
                }
                else if(player1 != null)
                {
                    GameManager.Instance.player1Hp--;
                }
                else if (player2 != null)
                {
                    GameManager.Instance.player2Hp--;
                }
            }
        }

        private void OnDrawGizmosSelected()// ½T»{½d³ò
        {
            if (atkPoint == null)
            {
                return;
            }
            Gizmos.DrawWireSphere(atkPoint.position, atkRange);
        }


        private void LateCancelAnimation()
        {
            ani.SetBool(IsPunching,false);
        }

    }
}
