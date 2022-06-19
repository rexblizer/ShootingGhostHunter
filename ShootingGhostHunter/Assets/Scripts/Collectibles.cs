using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectibles : MonoBehaviour
{
    [SerializeField] private LayerMask whatIsPlayer;
    private Transform player;
    [SerializeField] private float pickUpTime = 0.3f;

    [SerializeField] private bool ammoPlayerinPickUpRange;
    [SerializeField] private float ammoPickUpRange;
    [SerializeField] private float ammoPickUpSpeed = 3f;

    [SerializeField] private bool healthPlayerinPickUpRange;
    [SerializeField] private float healthPickUpRange;
    [SerializeField] private float healthPickUpSpeed;

    //1=gun, 2=meleeUlt, 3=rangedUlt
    [SerializeField] private int upgradeNumber;
    [SerializeField] private bool upgradePlayerinPickUpRange;
    [SerializeField] private float upgradePickUpRange;
    [SerializeField] private float upgradePickUpSpeed;
    [SerializeField] private GameObject gunUpgrade;
    [SerializeField] private GameObject meleeUltUpgrade;
    [SerializeField] private GameObject rangedUltUpgrade;

    [SerializeField] private float rotationSpeed;
    [SerializeField] private float hoverHeight;
    [SerializeField] private float hoverSpeed;
    [SerializeField] private bool hoverAtMaxHeight;
    private Vector3 hoverStartPosition;
    private Vector3 hoverTargetPosition;

    private void Awake()
    {
        if(tag == "Upgrade")
        {
            hoverStartPosition = transform.position;
            NewTargetPosition();
            hoverHeight = 0.4f;
            hoverSpeed = 0.2f;
            rotationSpeed = 100f;
            switch (upgradeNumber)
            {
                case (1):
                    gunUpgrade.SetActive(true);
                    meleeUltUpgrade.SetActive(false);
                    rangedUltUpgrade.SetActive(false);
                    break;
                case (2):
                    gunUpgrade.SetActive(false);
                    meleeUltUpgrade.SetActive(true);
                    rangedUltUpgrade.SetActive(false);
                    break;
                case (3):
                    gunUpgrade.SetActive(false);
                    meleeUltUpgrade.SetActive(false);
                    rangedUltUpgrade.SetActive(true);
                    break;
            }
        }
        if(tag == "Health")
        {
            gameObject.GetComponent<Rigidbody>().useGravity = false;
        }
        player = GameObject.Find("PlayerArmature").transform;
    }
    void Update()
    {
        if(tag == "Upgrade")
        {
            Movement();
        }
        PickUpCheck();
        UpPicking();
    }
    private void Movement()
    {
        transform.position = Vector3.MoveTowards(transform.position, hoverTargetPosition, hoverSpeed * Time.deltaTime);

        Vector3 distanceToHoverTarget = transform.position - hoverTargetPosition;
        if (distanceToHoverTarget.magnitude < 0.1f)
        {
            NewTargetPosition();
            if (hoverAtMaxHeight == true) hoverAtMaxHeight = false; else hoverAtMaxHeight = true;
        }

            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
    }
    private void NewTargetPosition()
    {
        if (!hoverAtMaxHeight)
        {
            hoverTargetPosition = new Vector3(hoverStartPosition.x, hoverStartPosition.y + hoverHeight, hoverStartPosition.z);
            //Debug.Log(hoverTargetPosition);
        }
        else
        {
            hoverTargetPosition = hoverStartPosition;
        }
    }
    private void PickUpCheck()
    {
        if (tag == "Ammo")
        {
            ammoPlayerinPickUpRange = Physics.CheckSphere(transform.position, ammoPickUpRange, whatIsPlayer);
        } else
        if (tag == "Health")
        {
            healthPlayerinPickUpRange = Physics.CheckSphere(transform.position, healthPickUpRange, whatIsPlayer);
        } else if (tag == "Upgrade")
        {
            upgradePlayerinPickUpRange = Physics.CheckSphere(transform.position, upgradePickUpRange, whatIsPlayer);
        }
    }
    private void UpPicking()
    {
        if (ammoPlayerinPickUpRange && tag == "Ammo")
        {
            gameObject.GetComponent<Rigidbody>().useGravity = false;
            Vector3 moveTarget = new Vector3(player.position.x, player.position.y + 1, player.position.z);
            transform.position = Vector3.MoveTowards(transform.position, moveTarget, ammoPickUpSpeed * Time.deltaTime);
            Invoke("AmmoPickUpSucces", pickUpTime);
        } else if (!ammoPlayerinPickUpRange && tag == "Ammo")
        {
            CancelInvoke("AmmoPickUpSucces");
            gameObject.GetComponent<Rigidbody>().useGravity = true;
        }
        if (healthPlayerinPickUpRange && tag == "Health")
        {
            Vector3 moveTarget = new Vector3(player.position.x, player.position.y + 1, player.position.z);
            transform.position = Vector3.MoveTowards(transform.position, moveTarget, healthPickUpSpeed * Time.deltaTime);
            Invoke("HealthPickUpSucces", pickUpTime);
        }
        else if (!healthPlayerinPickUpRange && tag == "Health")
        {
            CancelInvoke("HealthPickUpSucces");
        }
        if (upgradePlayerinPickUpRange && tag == "Upgrade")
        {
            Vector3 moveTarget = new Vector3(player.position.x, player.position.y + 1, player.position.z);
            transform.position = Vector3.MoveTowards(transform.position, moveTarget, upgradePickUpSpeed * Time.deltaTime);
            Invoke("UpgradePickUpSucces", 0f);
        }
        else if (!upgradePlayerinPickUpRange && tag == "Upgrade")
        {
            CancelInvoke("UpgradePickUpSucces");
        }
    }
    private void AmmoPickUpSucces()
    {
            CancelInvoke("AmmoPickUpSucces");
            player.GetComponent<ThirdPersonShooterController>().AmmoIncrease();
            Destroy(gameObject);
    }
    private void HealthPickUpSucces()
    {
        CancelInvoke("HealthPickUpSucces");
        player.GetComponent<ThirdPersonShooterController>().HealthIncrease();
        Destroy(gameObject);
    }

    private void UpgradePickUpSucces()
    {
        CancelInvoke("UpgradePickUpSucces");
        switch (upgradeNumber)
        {
            case (1):
                PlayerStatus.hasSword = true;
                break;
            case (2):
                PlayerStatus.hasMeleeUlt = true;
                break;
            case (3):
                PlayerStatus.hasRangedUlt = true;
                break;
        }
        Destroy(gameObject);
    }
    private void OnDrawGizmosSelected()

    {
        if (tag == "Ammo")
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, ammoPickUpRange);
        }
        if (tag == "Health")
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, healthPickUpRange);
        }
    }
}
