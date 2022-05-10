using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using UnityEngine.InputSystem;
using TMPro;

public class ThirdPersonShooterController : MonoBehaviour
{
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform debugTransform;
    [SerializeField] private Transform pfRangedBasic;
    [SerializeField] private Transform pfMeleeBasic;
    [SerializeField] private Transform pfRangedUlt;
    [SerializeField] private Transform pfMeleeUlt;
    [SerializeField] private Transform spawnBulletPosition;
    [SerializeField] private Transform spawnMeleePosition;
    [SerializeField] private Transform spawnRangedUltPosition;
    [SerializeField] private Transform VfxHitRed;
    public Vector3 mouseWorldPosition = Vector3.zero;

    //Ammo
    [SerializeField] private int maxAmmo;
    [SerializeField] private int ammoRemaining = 5;
    [SerializeField] private int ammoIncreasePerCrate = 5;
    [SerializeField] private TMP_Text ammoCounter;

    //Health
    [SerializeField] private int healthRemaining = 50;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int healthIncreasePerOrb = 10;
    [SerializeField] private TMP_Text healthCounter;

    //cooldowns für angriffe
    [SerializeField] private bool rangedBasicAvailable = true;
    [SerializeField] private bool meleeBasicAvailable = true;
    [SerializeField] private bool meleeUltAvailable = true;
    [SerializeField] private bool rangedUltAvailable = true;

    [SerializeField] private float rangedBasicCooldown = 0.5f;
    [SerializeField] private float meleeBasicCooldown = 1f;
    [SerializeField] private float meleeUltCooldown = 5f;
    [SerializeField] private float rangedUltCooldown = 5f;

    //private ThirdPersonController thirdPersonController;
    private StarterAssetsInputs starterAssetsInputs;

    private void Awake()
    {
        //thirdPersonController = GetComponent<ThirdPersonController>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
    }
    void Update()
    {
        ammoCounter.text = "Ammo: " + ammoRemaining;
        healthCounter.text = "Leben: " + healthRemaining + "/" + maxHealth;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            mouseWorldPosition = raycastHit.point;
            debugTransform.position = mouseWorldPosition;
        }

        if (starterAssetsInputs.shoot && rangedBasicAvailable && ammoRemaining > 0)
        {
            ammoRemaining = ammoRemaining - 1;
            starterAssetsInputs.shoot = false;
            rangedBasicAvailable = false;
            pfRangedBasic.GetComponent<PlayerAttacks>().Setup(mouseWorldPosition);
            Vector3 aimDir = (mouseWorldPosition - spawnBulletPosition.position).normalized;
            Instantiate(pfRangedBasic, spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
            Invoke("RangedBasicReset", rangedBasicCooldown);
        }
        if (starterAssetsInputs.melee && meleeBasicAvailable)
        {
            starterAssetsInputs.melee = false;
            meleeBasicAvailable = false;
            Vector3 aimDir = (mouseWorldPosition - spawnMeleePosition.position).normalized;
            Instantiate(pfMeleeBasic, spawnMeleePosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
            Invoke("MeleeBasicReset", meleeBasicCooldown);
        }
        if (starterAssetsInputs.meleeUlt && meleeUltAvailable)
        {
            starterAssetsInputs.meleeUlt = false;
            meleeUltAvailable = false;
            Vector3 meleeUltSpawnPosition = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 1, gameObject.transform.position.z);
            Instantiate(pfMeleeUlt, meleeUltSpawnPosition, Quaternion.identity);
            Invoke("MeleeUltReset", meleeUltCooldown);
        }
        if (starterAssetsInputs.shootUlt && rangedUltAvailable)
        {
            starterAssetsInputs.shootUlt = false;
            rangedUltAvailable = false;
            LookAtMouseWorldPosition();
            pfRangedUlt.GetComponent<PlayerAttacks>().Setup(mouseWorldPosition);
            Instantiate(pfRangedUlt, spawnRangedUltPosition.position, Quaternion.identity);
            Invoke("RangedUltReset", rangedUltCooldown);
        }
    }
    private void LookAtMouseWorldPosition()
    {
        Vector3 lookAtTarget = new Vector3(mouseWorldPosition.x, gameObject.transform.position.y, mouseWorldPosition.z);
        gameObject.transform.LookAt(lookAtTarget);
    }
    private void RangedBasicReset()
    {
        rangedBasicAvailable = true;
    }
    private void MeleeBasicReset()
    {
        meleeBasicAvailable = true;
    }
    private void MeleeUltReset()
    {
        meleeUltAvailable = true;
    }
    private void RangedUltReset()
    {
        rangedUltAvailable = true;
    }
    public void AmmoIncrease()
    {
        ammoRemaining = ammoRemaining + ammoIncreasePerCrate;
        Debug.Log(ammoRemaining);
        if(ammoRemaining > maxAmmo)
        {
            ammoRemaining = maxAmmo;
        }
    }
    public void HealthIncrease()
    {
        healthRemaining = healthRemaining + healthIncreasePerOrb;
        Debug.Log(healthRemaining);
        if (healthRemaining > maxHealth)
        {
            healthRemaining = maxHealth;
        }
    }
}
