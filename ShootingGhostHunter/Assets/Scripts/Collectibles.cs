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
 
    private void Awake()
    {
        if(tag == "Health")
        {
            gameObject.GetComponent<Rigidbody>().useGravity = false;
        }
        player = GameObject.Find("PlayerArmature").transform;
    }
    void Update()
    {
        PickUpCheck();
        UpPicking();
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
