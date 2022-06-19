using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class ThirdPersonShooterController : MonoBehaviour
{
    [SerializeField] UnityEvent PauseMenuButtonPressed;
    [SerializeField] private GameObject swordGeometry;
    [SerializeField] UnityEvent OnDeath;
    private PlayerInput playerInput;
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
    [SerializeField] private UiController uiController;

    //cooldowns für angriffe
    [SerializeField] private bool rangedBasicAvailable = true;
    [SerializeField] private bool meleeBasicAvailable = true;
    [SerializeField] private bool meleeUltAvailable = true;
    [SerializeField] private bool rangedUltAvailable = true;

    [SerializeField] private float rangedBasicCooldown = 0.5f;
    [SerializeField] private float meleeBasicCooldown = 1f;
    [SerializeField] private float meleeUltCooldown = 15f;
    [SerializeField] private float rangedUltCooldown = 2f;

    //private ThirdPersonController thirdPersonController;
    private StarterAssetsInputs starterAssetsInputs;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        uiController.SetHealthBarMaxHealth(maxHealth);
        //thirdPersonController = GetComponent<ThirdPersonController>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
    }
    void Update()
    {
        if (playerInput.currentActionMap.name == "Player")
        {
            playerInput.currentActionMap.FindAction("PauseMenu").performed += context => PauseMenuButtonPressed.Invoke();
        }
        if (!PlayerStatus.hasSword && !PlayerStatus.hasRangedUlt) swordGeometry.SetActive(false); else swordGeometry.SetActive(true);
        float healthPercent = 1.0f * healthRemaining / maxHealth;
        if (healthPercent <= 0.25f) uiController.healthLow = true; else uiController.healthLow = false;
        if (playerInput.currentActionMap.name == "Death")
        {
            playerInput.currentActionMap.FindAction("Restart").performed += contex => Restart();
        }
        if (healthRemaining < 0) healthRemaining = 0;
        ammoCounter.text = ammoRemaining.ToString();
        healthCounter.text = healthRemaining.ToString();
        if (healthRemaining <= 0)
        {
            NoHealthRemaining();
        }
        uiController.SetHealthBar(healthRemaining);
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            mouseWorldPosition = raycastHit.point;
            debugTransform.position = mouseWorldPosition;
        }

        if (starterAssetsInputs.shoot)
        {
            starterAssetsInputs.shoot = false;
                if (rangedBasicAvailable && ammoRemaining > 0)
                {
                    StartCoroutine(Shoot());
                }
        }
        if (starterAssetsInputs.melee)
        {
            starterAssetsInputs.melee = false;
            if (meleeBasicAvailable && PlayerStatus.hasSword)
            {
                meleeBasicAvailable = false;
                Vector3 aimDir = (mouseWorldPosition - spawnMeleePosition.position).normalized;
                Instantiate(pfMeleeBasic, spawnMeleePosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
                Invoke("MeleeBasicReset", meleeBasicCooldown);
            }
        }
        if (starterAssetsInputs.meleeUlt)
        {
            starterAssetsInputs.meleeUlt = false;
            if (PlayerStatus.hasMeleeUlt)
            {
                if (meleeUltAvailable)
                {
                    meleeUltAvailable = false;
                    uiController.StartMeleeUltCD(meleeUltCooldown);
                    Vector3 meleeUltSpawnPosition = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 1, gameObject.transform.position.z);
                    Instantiate(pfMeleeUlt, meleeUltSpawnPosition, Quaternion.identity);
                    Invoke("MeleeUltReset", meleeUltCooldown);
                }
            }
        }
        if (starterAssetsInputs.shootUlt)
        {
            starterAssetsInputs.shootUlt = false;
            if (PlayerStatus.hasRangedUlt)
            {
                if (rangedUltAvailable)
                {
                    rangedUltAvailable = false;
                    uiController.StartRangedUltCD(rangedUltCooldown);
                    LookAtMouseWorldPosition();
                    pfRangedUlt.GetComponent<PlayerAttacks>().Setup(mouseWorldPosition);
                    Instantiate(pfRangedUlt, spawnRangedUltPosition.position, Quaternion.identity);
                    Invoke("RangedUltReset", rangedUltCooldown);
                }
            }
        }
    }
 
    IEnumerator Shoot()
    {
        Vector3 aimDir = (mouseWorldPosition - spawnBulletPosition.position).normalized;
        ammoRemaining = ammoRemaining - 1;
        rangedBasicAvailable = false;
        pfRangedBasic.GetComponent<PlayerAttacks>().Setup(mouseWorldPosition);
        yield return new WaitForSecondsRealtime(0.1f);
        Instantiate(pfRangedBasic, spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
        Invoke("RangedBasicReset", rangedBasicCooldown);
        StopCoroutine(Shoot());
        yield return null;
    }

    private void NoHealthRemaining()
    {
        playerInput.SwitchCurrentActionMap("Death");
        OnDeath.Invoke();
    }

    private void Restart()
    {
        PlayerStatus.hasSword = false;
        PlayerStatus.hasMeleeUlt = false;
        PlayerStatus.hasRangedUlt = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
        //Debug.Log(ammoRemaining);
        if(ammoRemaining > maxAmmo)
        {
            ammoRemaining = maxAmmo;
        }
    }
    public void HealthIncrease()
    {
        healthRemaining = healthRemaining + healthIncreasePerOrb;
        //Debug.Log(healthRemaining);
        if (healthRemaining > maxHealth)
        {
            healthRemaining = maxHealth;
        }
    }
    public void HealthDecrease(int Amount)
    {
        healthRemaining = healthRemaining - Amount;
    }
}
