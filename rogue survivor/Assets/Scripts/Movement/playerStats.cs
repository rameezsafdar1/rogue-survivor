using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class playerStats : MonoBehaviour, iDamagable
{
    [SerializeField] private UpgradesSTT upgradeManager;
    public float health;
    private float maxHealth;
    public float damage;
    public Image healthBar, healFill;
    public UnityEvent onDeadEvent;
    private float healTimer;


    private void Start()
    {
        health += saveManager.Instance.loadCustomFloats("additionalHealth");
        maxHealth = health;
    }

    private void Update()
    {
        healTimer += Time.deltaTime;        
        healFill.fillAmount = healTimer / 15f;
    }

    public void takeDamage(float damage)
    {
        health -= damage;

        float fillval = health / maxHealth;
        fillval = Mathf.Clamp(fillval, 0.2f, 1f);

        healthBar.fillAmount = fillval;

        if (health <= 0)
        {
            gameObject.SetActive(false);
            if (onDeadEvent != null)
            {
                onDeadEvent.Invoke();
            }           
        }

    }

    public void increaseHealth()
    {
        health += (25 / 100) * maxHealth;

        if (health > maxHealth)
        {
            health = maxHealth;
        }

        float fillval = health / maxHealth;
        fillval = Mathf.Clamp(fillval, 0.2f, 1f);

        healthBar.fillAmount = fillval;
    }

    public void HealComplete()
    {
        if (healTimer < 15)
        {
            upgradeManager.PerkFail("Heal cooldown not complete");
            return;
        }

        health = maxHealth;

        if (health > maxHealth)
        {
            health = maxHealth;
        }

        float fillval = health / maxHealth;
        fillval = Mathf.Clamp(fillval, 0.2f, 1f);

        healthBar.fillAmount = fillval;
        healTimer = 0;
    }

}
