using System.Collections;
using System.Collections.Generic;
using General;
using Input;
using UnityEngine;
using UnityEngine.Serialization;

public class Attack : SingletonMonoBehavior<Attack>
{


    public Animator ani;
    public Transform atkPoint;
    public float atkRange = 1f;
    [FormerlySerializedAs("EnemyLayer")] public LayerMask enemyLayer;
    private static readonly int Attack1 = Animator.StringToHash("Attack");

    [SerializeField]private KeyCode attackKey;


    // Update is called once per frame
    void Update()
    {
        if (UnityEngine.Input.GetKeyDown(attackKey))
        {
            PlayerAttack();
        }
    }
    private void PlayerAttack()
    {
        ani.SetTrigger(Attack1);
        var hit= Physics.OverlapSphere(atkPoint.position, atkRange, enemyLayer);

        foreach(var enemy in hit)
        {
            Debug.Log("hit" + enemy.name);
            var police = enemy.GetComponent<PoliceAI>();
            var player1 = enemy.GetComponent<Player1>();
            var player2 = enemy.GetComponent<Player2>();
            if(police != null)
            {
                police.TakeDamage(1);
            }
        }
        


    }
   
    private void OnDrawGizmosSelected()// �T�{�d��
    {
        if (atkPoint == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(atkPoint.position, atkRange);
    }

}
