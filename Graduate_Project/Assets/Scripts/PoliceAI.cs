using General;
using UnityEngine;
using UnityEngine.AI;
public class PoliceAI : SingletonMonoBehavior<PoliceAI>
{
    public NavMeshAgent agent;
    
    public Transform player;
    
    public LayerMask isGround, isPlayer;

    public float policeHealth=3f;

    

    
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


    public override void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
        
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
        agent.SetDestination(player.position);
        
    }

    private void AttackPlayer()//§ðÀ»ª±®a
    {
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
    }



    public void TakeDamage(int damage)
    {

        policeHealth -= damage;
        Debug.Log(policeHealth);
        if(policeHealth <= 0)
        {
            Invoke(nameof(DestroyPolice), 1f);
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
