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

    private AudioSource Atkaudio;
    public AudioClip Punch;

    [SerializeField]private KeyCode attackKey;


    // Update is called once per frame
    private void Start()
    {
        Atkaudio = GetComponent<AudioSource>();
    }
    void Update()
    {
        Atkaudio.clip = Punch; 
        if (UnityEngine.Input.GetKeyDown(attackKey))
        {
            PlayerAttack();
            Atkaudio.Play();           
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

}
