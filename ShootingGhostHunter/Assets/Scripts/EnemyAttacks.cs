using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttacks : MonoBehaviour
{
    //1 = Sword, 2 = Archer, 3 = Tank, 4 = Mage
    private Transform player;
    private Vector3 targetPosition;
    [SerializeField] private int attackOfClass;

    [SerializeField] private float swordAttackDuration;
    [SerializeField] private int swordAttackDamage;

    [SerializeField] private int archerMoveSpeedAttack;
    [SerializeField] private int archerAttackDamage;

    [SerializeField] private float tankAttackDuration;
    [SerializeField] private int tankAttackDamage;

    [SerializeField] private int mageMoveSpeedAttack;
    [SerializeField] private float mageAttackDuration;
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private float mageAttackExplosionRange;
    [SerializeField] private int mageAttackDamage;


    public void Awake()
    {
        player = GameObject.Find("PlayerArmature").transform;
        targetPosition = new Vector3(player.transform.position.x, player.transform.position.y + 1.5f, player.transform.position.z);

        switch (attackOfClass)
        {
            case (1):
                transform.LookAt(targetPosition);
                break;
            case (2):
                transform.LookAt(targetPosition);
                transform.Rotate(90, 0, 0, Space.Self);
                Vector3 flyTowards = (targetPosition - transform.position).normalized;
                gameObject.GetComponent<Rigidbody>().AddForce(flyTowards * 20f, ForceMode.Impulse);
                break;
            case (3):
                transform.LookAt(targetPosition);
                break;
            case (4):
                Destroy(gameObject, mageAttackDuration);
                break;
        }
    }
    //behaviour of the Attack
    void Update()
    {
        switch (attackOfClass)
        {
            case (1):
                Destroy(gameObject, swordAttackDuration);
                break;
            case (2):
                break;
            case (3):
                Destroy(gameObject, tankAttackDuration);
                break;
            case (4):
                targetPosition = new Vector3(player.transform.position.x, player.transform.position.y + 1.5f, player.transform.position.z);
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, mageMoveSpeedAttack * Time.deltaTime);
                break;
        }
    }

    //effect of the Attack
    private void OnTriggerEnter(Collider col)
    {
        if(attackOfClass == 2 && col.tag != "Attack" && col.tag != "Room" || attackOfClass == 4 && col.tag != "Attack" && col.tag != "Room")
        {
            if(attackOfClass == 4)
            {
                bool playerInExplosionRange = Physics.CheckSphere(transform.position, mageAttackExplosionRange, whatIsPlayer);
                if (playerInExplosionRange)
                {
                    player.GetComponent<ThirdPersonShooterController>().HealthDecrease(mageAttackDamage);
                }
            }
            Destroy(gameObject);
        }
        if (col.tag == "Player")
        {
            switch (attackOfClass)
            {
                case (1):
                    player.GetComponent<ThirdPersonShooterController>().HealthDecrease(swordAttackDamage);
                    break;

                case (2):
                    player.GetComponent<ThirdPersonShooterController>().HealthDecrease(archerAttackDamage);
                    break;

            case (3):
                    player.GetComponent<ThirdPersonShooterController>().HealthDecrease(tankAttackDamage);
                    break;

                case (4):
                    
                    break;
            }
        }
    }
}
