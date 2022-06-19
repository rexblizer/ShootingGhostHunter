using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UiController : MonoBehaviour
{
    public bool healthLow;
    [SerializeField] private GameObject outline;

    public Slider healthBarSlider;
    public Gradient healthBarGradient;
    public Image healthBarFill;
    public Image healthBarHeart;

    [SerializeField] private GameObject rangedUltCooldown;
    public Image rangedCDFill;
    private float rangedCD;
    private bool rangedUltCoolingDown;
    [SerializeField] private int rangedCooldownDisplay;
    [SerializeField] private TMP_Text rangedCooldownCounter;

    [SerializeField] private GameObject meleeUltCooldown;
    public Image meleeCDFill;
    private float meleeCD;
    private bool meleeUltCoolingDown;
    [SerializeField] private int meleeCooldownDisplay;
    [SerializeField] private TMP_Text meleeCooldownCounter;

    [SerializeField] private GameObject ammoCounter;

    [SerializeField] private TMP_Text deathText;

    private void Update()
    {
        if (!PlayerStatus.hasMeleeUlt) meleeUltCooldown.SetActive(false);
        else meleeUltCooldown.SetActive(true);
        if (!PlayerStatus.hasRangedUlt) rangedUltCooldown.SetActive(false);
        else rangedUltCooldown.SetActive(true);
        if (healthLow) outline.SetActive(true); else outline.SetActive(false);
        if(rangedUltCoolingDown == true)
        {
            rangedCDFill.fillAmount -= 1.0f / rangedCD * Time.deltaTime;
        }
        if (meleeUltCoolingDown == true)
        {
            meleeCDFill.fillAmount -= 1.0f / meleeCD * Time.deltaTime;
        }
    }

    public void SetHealthBar(int health)
    {
        healthBarSlider.value = health;
        healthBarFill.color = healthBarGradient.Evaluate(healthBarSlider.normalizedValue);
        healthBarHeart.color = healthBarGradient.Evaluate(healthBarSlider.normalizedValue);
    }

    public void SetHealthBarMaxHealth(int health)
    {
        healthBarSlider.maxValue = health;
        healthBarFill.color = healthBarGradient.Evaluate(healthBarSlider.normalizedValue);
        healthBarHeart.color = healthBarGradient.Evaluate(healthBarSlider.normalizedValue);
    }

    public void StartMeleeUltCD(float CD)
    {
        meleeCDFill.fillAmount = 1f;
        meleeCooldownDisplay = Mathf.RoundToInt(CD);
        meleeCooldownCounter.text = meleeCooldownDisplay.ToString();
        Invoke("ReduceMeleeCooldown", 1f);
        meleeCD = CD;
        meleeUltCoolingDown = true;
    }

    public void StartRangedUltCD(float CD)
    {
        rangedCDFill.fillAmount = 1f;
        rangedCooldownDisplay = Mathf.RoundToInt(CD);
        rangedCooldownCounter.text = rangedCooldownDisplay.ToString();
        Invoke("ReduceRangedCooldown", 1f);
        rangedCD = CD;
        rangedUltCoolingDown = true;
    }

    private void ReduceRangedCooldown()
    {
        rangedCooldownDisplay = rangedCooldownDisplay - 1;
        rangedCooldownCounter.text = rangedCooldownDisplay.ToString();
        if (rangedCooldownDisplay > 0)
        {
            Invoke("ReduceRangedCooldown", 1f);
        }
        else
        {
            rangedCooldownCounter.text = "";
        }
    }

    private void ReduceMeleeCooldown()
    {
        meleeCooldownDisplay = meleeCooldownDisplay - 1;
        meleeCooldownCounter.text = meleeCooldownDisplay.ToString();
        if (meleeCooldownDisplay > 0)
        {
            Invoke("ReduceMeleeCooldown", 1f);
        }
        else
        {
            meleeCooldownCounter.text = "";
        }
    }

    public void DeathTextActivate()
    {
        deathText.text = "You died a horrific and brutal death! Press R to try again";
    }
}
