using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class mainMenuUpgrades : MonoBehaviour
{
    public Image fill1, fill2;
    public Button b1, b2;
    private float upHealth, upSpeed;
    public float cashForSpeed, cashForHealth;
    public TMPro.TextMeshProUGUI cashText;

    private void OnEnable()
    {
        cashText.text = currencyShortener(saveManager.Instance.loadCash());
    }

    private void Start()
    {
        upHealth = saveManager.Instance.loadCustomFloats("additionalHealth");
        fill1.fillAmount = upHealth / 300;
        if (upHealth >= 300)
        {
            b1.interactable = false;
        }

        upSpeed = saveManager.Instance.loadCustomFloats("additionalSpeed");
        fill2.fillAmount = upSpeed / 3;
        if (upSpeed >= 3)
        {
            b2.interactable = false;
        }
    }

    public void upgradeHealth()
    {
        float cash = saveManager.Instance.loadCash();
        if (cash >= cashForHealth)
        {
            cash -= 5000;
            saveManager.Instance.addCash(-(int)cash);
            cashText.text = currencyShortener(saveManager.Instance.loadCash());
            upHealth += 100;
            fill1.fillAmount = upHealth / 300;
            saveManager.Instance.saveCustomFloats("additionalHealth", upHealth);

            if (upHealth >= 300)
            {
                b1.interactable = false;
            }
        }
    }

    public void updateSpeed()
    {
        float cash = saveManager.Instance.loadCash();
        if (cash >= cashForSpeed)
        {
            cash -= 5000;
            saveManager.Instance.addCash(-(int)cash);
            cashText.text = currencyShortener(saveManager.Instance.loadCash());
            upSpeed += 1;
            fill2.fillAmount = upSpeed / 3;
            saveManager.Instance.saveCustomFloats("additionalSpeed", upSpeed);
            if (upSpeed >= 3)
            {
                b2.interactable = false;
            }
        }

    }

    public string currencyShortener(float currency)
    {
        string converted = "";

        if (currency >= 1000)
        {
            converted = (currency / 1000).ToString() + "K";
        }
        else
        {
            converted = currency.ToString();
        }
        return converted;
    }

}
