using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttacks : MonoBehaviour
{
    public int attackType;
    [SerializeField] private Vector3 targetPosition;
    private Vector3 bulletTarget;
    [SerializeField] private Transform player;

    //Ranged Variables
    [SerializeField] private Transform vfxHitRed;
    [SerializeField] private Transform vfxHitGreen;
    [SerializeField] private float moveSpeed = 50f;
    [SerializeField] private Vector3 lastPosition;
    [SerializeField] private int rangedDamage = 1;
    [SerializeField] private int rangedUltDamage = 4;
    [SerializeField] private float rangedUltAttackTime = 2.5f;

    //Melee Variables
    [SerializeField] private float meleeAttackTime = 0.5f;
    [SerializeField] private float meleeUltAttackTime = 1f;
    [SerializeField] private int meleeDamage = 2;
    [SerializeField] private int meleeUltDamage = 4;

    //Get the target point from another Script
    public void Setup(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }

    private void Start()
    {
        player = GameObject.Find("PlayerArmature").transform;
        if(attackType == 1)
        {
            bulletTarget = targetPosition;
            LookAtMouseWorldPosition();
        }else
        if (attackType == 2)
        {
            gameObject.transform.parent = player;
            LookAtMouseWorldPosition();
        }
        else
        if (attackType == 3)
        {
            gameObject.transform.parent = player;
        }
        else
        if (attackType == 4)
        {
            gameObject.transform.RotateAround(gameObject.transform.position, player.right, 90);
        }
    }

    //Behaviour of the Attack
    private void Update()
    {
        targetPosition = player.GetComponent<ThirdPersonShooterController>().mouseWorldPosition;
        if (attackType == 1)
        {
            lastPosition = transform.position;
            Vector3 moveDir = (bulletTarget - transform.position).normalized;
            transform.position += moveDir * moveSpeed * Time.deltaTime;

        }else if (attackType == 2)
        {
            LookAtMouseWorldPosition();
            Vector3 LookAtPosition = new Vector3(targetPosition.x, gameObject.transform.position.y, targetPosition.z);
            gameObject.transform.LookAt(LookAtPosition);
            Destroy(gameObject, meleeAttackTime);
        }
        else if (attackType == 3)
        {
            Destroy(gameObject, meleeUltAttackTime);
        }
        else if (attackType == 4)
        {
            Destroy(gameObject, rangedUltAttackTime);
        }
    }
    //Interactions of the attack
    private void OnTriggerEnter(Collider col)
    {
        Vector3 pushDirection = (col.transform.position - player.transform.position);
        string tag = col.tag;
        switch (attackType)
        {
            case (1):
                if(tag != "Room" && tag != "Attack")
                {
                Destroy(gameObject);
                }
                if (tag == "Enemy")
                {
                    Instantiate(vfxHitRed, lastPosition, Quaternion.identity);
                    col.gameObject.GetComponent<Rigidbody>().AddForce(pushDirection * 1.5f, ForceMode.Impulse);
                    Debug.Log("Enemy hit");
                    col.GetComponent<EnemyAi>().enemyHealth = col.GetComponent<EnemyAi>().enemyHealth - rangedDamage;
                }
                break;
            case (2):
                if (tag == "Enemy")
                {
                    col.GetComponent<EnemyAi>().enemyHealth = col.GetComponent<EnemyAi>().enemyHealth - meleeDamage;
                    col.gameObject.GetComponent<Rigidbody>().AddForce(pushDirection * 2, ForceMode.Impulse);
                    Debug.Log("Gegner Geschlagen");
                }
                break;
            case (3):
                if (tag == "Enemy")
                {
                    col.GetComponent<EnemyAi>().enemyHealth = col.GetComponent<EnemyAi>().enemyHealth - meleeUltDamage;
                    col.gameObject.GetComponent<Rigidbody>().AddForce(pushDirection * 4, ForceMode.Impulse);
                    Debug.Log("Melee Ult Hit");
                }
                break;
            case (4):
                if (tag == "Enemy")
                {
                    col.GetComponent<EnemyAi>().enemyHealth = col.GetComponent<EnemyAi>().enemyHealth - rangedUltDamage;
                    col.gameObject.GetComponent<Rigidbody>().AddForce(pushDirection * 2, ForceMode.Impulse);
                    Debug.Log("Range Ult Hit");
                }
                break;
        }
    }
    private void LookAtMouseWorldPosition()
    {
        Vector3 lookAtTarget = new Vector3(targetPosition.x, player.position.y, targetPosition.z);
        player.LookAt(lookAtTarget);
    }
}
