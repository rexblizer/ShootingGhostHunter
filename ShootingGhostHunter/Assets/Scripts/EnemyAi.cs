using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAi : MonoBehaviour
{
    public float enemyMaxHealth = 5; 
    public float enemyHealth = 1;
    //1 = sword(yellow), 2 = Archer (green), 3 = Tank (red), 4 = Mage (blue),
    public int numberEnemyClass;
    //Movment/AI variables
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask whatIsGround, whatIsPlayer, whatIsEnemy;
    private Vector3 turnDirection;
    [SerializeField] private bool playerBehindWall;
    [SerializeField] private Vector3 lastKnownPlayerPosition;
    [SerializeField] private bool hasSeenPlayer = false;

    [SerializeField] private Transform pfAmmoCrate;
    [SerializeField] private Transform pfHealthOrb;

    //Walking Around Randomly
    private Vector3 walkPoint;
    private bool walkPointSet;
    [SerializeField] private float walkPointRange;

    //Attacking
    [SerializeField] private Transform AttackSpawn;
    [SerializeField] private Transform pfSwordAttack;
    [SerializeField] private Transform pfArcherAttack;
    [SerializeField] private Transform pfTankAttack;
    [SerializeField] private Transform pfMageAttack;
    [SerializeField] private float swordTimeBetweenAttacks;
    [SerializeField] private float archerTimeBetweenAttacks;
    [SerializeField] private float tankTimeBetweenAttacks;
    [SerializeField] private float mageTimeBetweenAttacks;
    [SerializeField] private bool alreadyAttacked;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    //Patroling
    [SerializeField] private Transform patrolPointA;
    [SerializeField] private Transform patrolPointB;
    [SerializeField] private bool isPatroling = false;
    //currentPatrolTarget 1 = PointA, currentPatrolTarget 2 = PointB
    [SerializeField] private int currentPatrolTarget = 1;

    private void Awake()
    {
        DeterminEnemyClass();
        enemyHealth = enemyMaxHealth;
        currentPatrolTarget = 1;
        player = GameObject.Find("PlayerArmature").transform;
        agent = GetComponent<NavMeshAgent>();
        InvokeRepeating("PlayerBehindWallCheck", 0.5f, 0.5f);
    }

    private void Update()
    {
        DeathCheck();
        StateCheck();
    }
    public void StateCheck()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerBehindWall)
        {
            if (!playerInSightRange && !playerInAttackRange) CheckLastKnownPlayerPosition();
            if (playerInSightRange && !playerInAttackRange) ChasePlayer();
            if (playerInAttackRange && playerInSightRange) AttackPlayer();
        }
        else
        {
            CheckLastKnownPlayerPosition();
        }
    }
    public void IdleBehaviour()
    {
        if (isPatroling) 
        { 
            Patrol();
        }
        else
        {
            RunAround();
        }
        
    }
    public void CheckLastKnownPlayerPosition()
    {
        if (hasSeenPlayer == true)
        {
            Vector3 distanceToLastKnownPlayerLocation = transform.position - lastKnownPlayerPosition;
            agent.SetDestination(lastKnownPlayerPosition);
            if (distanceToLastKnownPlayerLocation.magnitude < 1.3f)
            {
                hasSeenPlayer = false;
            }
        }
        else
        {
            IdleBehaviour();
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
        if (!alreadyAttacked) 
        {
            switch (numberEnemyClass)
            {
                case (1):
                    AttackSword();
                    break;
                case (2):
                    AttackArcher();
                    break;
                case (3):
                    AttackTank();
                    break;
                case (4):
                    AttackMage();
                    break;
            }
        }
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
        if (currentPatrolTarget == 1 && !playerInSightRange)
        {
            agent.SetDestination(DestinationA);
            if (distanceToPatrolpointA.magnitude < 1.3f)
            {
                ChoosePatrolTarget();
            }
        }
        else
        if (currentPatrolTarget == 2 && !playerInSightRange)
        {
            agent.SetDestination(DestinationB);
            if (distanceToPatrolpointB.magnitude < 1.3f)
            {
                ChoosePatrolTarget();
            }
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
    private void DeathCheck()
    {
        if (enemyHealth <= 0)
        {
            int ammoDropChance = (Random.Range(1, 3));
            if (ammoDropChance == 1)
            {
                Instantiate(pfAmmoCrate, transform.position, Quaternion.identity);
            }
            int healthDropChance = (Random.Range(1, 3));
            if (healthDropChance == 1)
            {
                Instantiate(pfHealthOrb, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
            GetComponentInParent<EnemySpawner>().EnemyKilled();
        }
    }

    private void DeterminEnemyClass()
    {
        if (numberEnemyClass != 1 && numberEnemyClass != 2 && numberEnemyClass != 3 && numberEnemyClass != 4)
        {
            float rand = Random.value;
            if(rand <= 0.25)
            {
                numberEnemyClass = 1;
            }
            else if (rand <= 0.5 && rand > 0.25)
            {
                numberEnemyClass = 2;
            }
            else if (rand <= 0.75 && rand > 0.5)
            {
                numberEnemyClass = 3;
            }
            else if (rand > 0.75)
            {
                numberEnemyClass = 4;
            }
        }
            switch (numberEnemyClass)
            {
                case (1):
                    gameObject.GetComponent<Renderer>().material.color = Color.yellow;
                    swordTimeBetweenAttacks = 5;
                    attackRange = 3;
                    sightRange = 6;
                    break;
                case (2):
                    gameObject.GetComponent<Renderer>().material.color = Color.green;
                    archerTimeBetweenAttacks = 5;
                    attackRange = 5;
                    sightRange = 12;
                    break;
                case (3):
                    gameObject.GetComponent<Renderer>().material.color = Color.red;
                    tankTimeBetweenAttacks = 5;
                    attackRange = 2;
                    sightRange = 6;
                    break;
                case (4):
                    gameObject.GetComponent<Renderer>().material.color = Color.blue;
                    mageTimeBetweenAttacks = 5;
                    attackRange = 6;
                    sightRange = 10;
                    break;
            }
    }

    private void OnDrawGizmosSelected()

    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(transform.position, sightRange);
    }

    private void AttackSword()
    {
        Instantiate(pfSwordAttack, AttackSpawn.position, Quaternion.identity);
        alreadyAttacked = true;
        Invoke("ResetAttack", swordTimeBetweenAttacks);
    }

    private void AttackArcher()
    {
        Instantiate(pfArcherAttack, AttackSpawn.position, Quaternion.identity);
        alreadyAttacked = true;
        Invoke("ResetAttack", archerTimeBetweenAttacks);
    }
    private void AttackTank()
    {
        Instantiate(pfTankAttack, AttackSpawn.position, Quaternion.identity);
        alreadyAttacked = true;
        Invoke("ResetAttack", tankTimeBetweenAttacks);
    }
    private void AttackMage()
    {
        Instantiate(pfMageAttack, AttackSpawn.position, Quaternion.identity);
        alreadyAttacked = true;
        Invoke("ResetAttack", mageTimeBetweenAttacks);
    }
    private void ResetAttack()
    {
        CancelInvoke("ResetAttack");
        alreadyAttacked = false;
    }
}
