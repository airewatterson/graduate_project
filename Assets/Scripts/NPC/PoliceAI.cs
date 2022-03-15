using General;
using UnityEngine;
using UnityEngine.AI;
public class PoliceAI : SingletonMonoBehavior<PoliceAI>
{
    public NavMeshAgent agent;
    
    public Transform player;
    
    public LayerMask isGround, isPlayer;

    public float policeHealth=3f;

    [SerializeField] private Animator animator;
    
    //¹C¿º
    public Vector3 walkPoint;
    public float walkPointRange;

    private bool _walkPointSet;
    //°»´ú
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;
    //§ðÀ»
    public float attackCooldown;
    private bool _alreadyAttacked;
  
    public Transform atkPoint;
    private static readonly int IsWalking = Animator.StringToHash("isWalking");
    private static readonly int IsRunning = Animator.StringToHash("isRunning");
    private static readonly int IsAttacking = Animator.StringToHash("isAttacking");
    private static readonly int IsDead = Animator.StringToHash("isDead");


    public override void Awake()
    {
        player = GameManager.Instance.player1.transform;
        
        agent = GetComponent<NavMeshAgent>();


    }
    private void Update()
    {
        var position = transform.position;
        playerInSightRange = Physics.CheckSphere(position, sightRange, isPlayer);
        playerInAttackRange = Physics.CheckSphere(position, attackRange, isPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patrolling();
        if (playerInSightRange && !playerInAttackRange) Chasing();
        if (playerInAttackRange && playerInSightRange) AttackPlayer();
    }


    private void Patrolling()//¹C¿º
    {
        animator.SetBool(IsWalking,true);
        if (!_walkPointSet) SearchWalkPoint();
        if (_walkPointSet) agent.SetDestination(walkPoint);

        Vector3 distanceWalkPoint = transform.position - walkPoint;

        if (distanceWalkPoint.magnitude < 1f)
        {
            _walkPointSet = false;
        }
    }

    private void SearchWalkPoint()
    {
        var randomZ = Random.Range(-walkPointRange, walkPointRange);
        var randomX = Random.Range(-walkPointRange, walkPointRange);
        var position = transform.position;
        walkPoint = new Vector3(position.x + randomX, position.y, position.z + randomZ);
        if (Physics.Raycast(walkPoint, -transform.up, 2f, isGround))
        {
            _walkPointSet = true;
        }
    }

    private void Chasing()//°l³v
    {
        animator.SetBool(IsRunning, true);
        agent.SetDestination(player.position);
        animator.SetBool(IsWalking,false);
    }

    private void AttackPlayer()//§ðÀ»ª±®a
    {
        animator.SetBool(IsAttacking,true);
        animator.SetBool(IsWalking,false);
        animator.SetBool(IsRunning, false);
        agent.SetDestination(transform.position);
        transform.LookAt(player);
       
        //§ðÀ»§N«o
        if (!_alreadyAttacked)
        {
            var hit = Physics.OverlapSphere(atkPoint.position, attackRange, isPlayer);

            foreach (var player1 in hit)
            {
                Debug.Log("hit" + player1.name);
            }




            _alreadyAttacked = true;
            Invoke(nameof(ResetAttack), attackCooldown);
        }
    }

    private void ResetAttack()
    {
        _alreadyAttacked = false;
        animator.SetBool(IsAttacking,false);
    }



    public void TakeDamage(int damage)
    {

        policeHealth -= damage;
        Debug.Log(policeHealth);
        if(policeHealth <= 0)
        {
            animator.SetBool(IsDead,true);
            Invoke(nameof(DestroyPolice), 3);
        }
    }

    private void DestroyPolice()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()// ½T»{½d³ò
    {

        //°»´ú
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(atkPoint.position, attackRange);
    }
    
}
