using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAi : MonoBehaviour
{

    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;
    public float health;
    public Vector3 turnDirection;
    [SerializeField] private bool playerBehindWall;
    [SerializeField] private Vector3 lastKnownPlayerPosition;
    [SerializeField] private bool hasSeenPlayer = false;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    [SerializeField] private Transform patrolPointA;
    [SerializeField] private Transform patrolPointB;
    [SerializeField] private bool isPatroling = false;
    [SerializeField] private int currentPatrolTarget = 1;

    private void Awake()
    {
        currentPatrolTarget = 1;
        player = GameObject.Find("PlayerArmature").transform;
        agent = GetComponent<NavMeshAgent>();
        InvokeRepeating("PlayerBehindWallCheck", 0.5f, 0.5f);
    }

    private void Update()
    {
        StateCheck();
    }
    public void StateCheck()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange && !isPatroling) RunAround();
        if (!playerInSightRange && !playerInAttackRange && isPatroling) Patrol();
        if (playerInSightRange && !playerInAttackRange && playerBehindWall && !isPatroling) RunAround();
        if (playerInSightRange && playerInAttackRange && playerBehindWall && isPatroling) Patrol();
        if (playerInSightRange && !playerInAttackRange && !playerBehindWall) ChasePlayer();
        if (playerInAttackRange && playerInSightRange && !playerBehindWall) AttackPlayer();

    }
    public void CheckLastKnownPlayerPosition()
    {
        if(lastKnownPlayerPosition != null)
        {
            Vector3 distanceToLastKnownPlayerLocation = transform.position - lastKnownPlayerPosition;
            agent.SetDestination(lastKnownPlayerPosition);
            if(distanceToLastKnownPlayerLocation.magnitude < 1.3f)
            {
                hasSeenPlayer = false;
            }
        }
    }
    private void RunAround()

    {
        if (!walkPointSet) SearchWalkPoint();
        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
        if(hasSeenPlayer == true)
        {
            CheckLastKnownPlayerPosition();
        }
    }
    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }
    private void ChasePlayer()

    {
        agent.SetDestination(player.position);
        lastKnownPlayerPosition = player.transform.position;
        hasSeenPlayer = true;
    }
    private void AttackPlayer()
    {
        lastKnownPlayerPosition = player.transform.position;
        hasSeenPlayer = true;
        TurnTowardsPlayer();
        //Make sure enemy doesn't move
        agent.SetDestination(transform.position);

        //transform.LookAt(player);

        //if (!alreadyAttacked)
        // {
        ///Attack code here
        //   Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
        // rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
        //  rb.AddForce(transform.up * 8f, ForceMode.Impulse);
        ///End of attack code

        //  alreadyAttacked = true;
        //  Invoke(nameof(ResetAttack), timeBetweenAttacks);
        //}
    }
    private void TurnTowardsPlayer()
    {
        turnDirection = player.position - transform.position;
        turnDirection.y = 0;
        turnDirection.Normalize();
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(turnDirection), 1f);
    }
    private void Patrol()
    {

        Vector3 DestinationA = new Vector3(patrolPointA.position.x, transform.position.y, patrolPointA.position.z);
        Vector3 DestinationB = new Vector3(patrolPointB.position.x, transform.position.y, patrolPointB.position.z);
        Vector3 distanceToPatrolpointA = transform.position - DestinationA;
        Vector3 distanceToPatrolpointB = transform.position - DestinationB;
        if (currentPatrolTarget == 1)
        {
            agent.SetDestination(DestinationA);
            if (distanceToPatrolpointA.magnitude < 1.3f)
            {
                ChoosePatrolTarget();
            }
        }
        else
        if (currentPatrolTarget == 2)
        {
            agent.SetDestination(DestinationB);
            if (distanceToPatrolpointB.magnitude < 1.3f)
            {
                ChoosePatrolTarget();
            }
        }
        if (hasSeenPlayer == true)
        {
            CheckLastKnownPlayerPosition();
        }
    }
    public void ChoosePatrolTarget()
    {
        if (currentPatrolTarget == 1)
        {
            currentPatrolTarget = 2;
        }
        else
        if (currentPatrolTarget == 2)
        {
            currentPatrolTarget = 1;
        }
    }
    public void PlayerBehindWallCheck()
    {
        Vector3 RayDirection = new Vector3(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y + 1, player.transform.position.z - transform.position.z);
        if (Physics.Raycast(transform.position, RayDirection, out RaycastHit hitInfo, 999f))
        {
            if (hitInfo.transform.tag == "Player")
            {
                //Debug.DrawRay(transform.position, RayDirection, Color.green, 99f);
                playerBehindWall = false;
            }
            else
            {
                //Debug.DrawRay(transform.position, RayDirection, Color.red, 99f);
                playerBehindWall = true;
            }
        }
    }
    private void OnDrawGizmosSelected()

    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
