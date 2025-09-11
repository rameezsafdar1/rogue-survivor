using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class upgradesManager : MonoBehaviour
{
    public GameObject[] randomUpgrades;

    [Header("> Drone Upgrades")]
    public Drone drone;
    public Button droneButton;
    public GameObject[] droneStars;
    private int currentDrone;
    public TextMeshProUGUI droneDetails;
    public string[] droneDetailsString;

    [Header("> Blade Upgrades")] 
    public Button bladeButton;
    public GameObject[] bladeStars, blades;
    private int currentBlade;
    public TextMeshProUGUI bladeDetails;
    public string[] bladeDetailsString;

    [Header("> Gun Upgrades")]
    public playerStats Player;
    public Button gunButton;
    public GameObject[] gunStars, guns;
    private int currentGun;
    public TextMeshProUGUI gunDetails;
    public string[] gunDetailsString;

    [Header("> Returner Upgrades")]
    public Button returnerButton;
    public GameObject[] returnerStars, returners;
    private int currentReturner;
    public TextMeshProUGUI returnerDetails;
    public string[] returnerDetailsString;

    [Header("> Thunder Upgrades")]
    public Button thunderButton;
    public thunderManager thunder_manager;
    public GameObject[] thunderStars;
    private int currentThunder;
    public TextMeshProUGUI thunderDetails;
    public string[] thunderDetailsString;


    public void updateDrone()
    {
        if (!drone.gameObject.activeSelf)
        {
            drone.gameObject.SetActive(true);
        }

        drone.shootingCount += 2;
        currentDrone++;

        for (int i = 0; i < currentDrone; i++)
        {
            droneStars[currentDrone - 1].SetActive(true);
        }
        droneDetails.text = droneDetailsString[currentDrone - 1];
        EffectsManager.Instance.setTimeScale(1);

        if (currentDrone >= 3)
        {
            droneButton.interactable = false;
        }

    }

    public void updateBlade()
    {
        currentBlade++;

        for (int i = 0; i < currentBlade; i++)
        {
            bladeStars[currentBlade - 1].SetActive(true);
        }

        blades[currentBlade - 1].SetActive(true);

        bladeDetails.text = bladeDetailsString[currentBlade - 1];
        EffectsManager.Instance.setTimeScale(1);

        if (currentBlade >= 3)
        {

            for (int i = 0; i < blades.Length; i++)
            {
                blades[i].SetActive(true);
            }

            bladeButton.interactable = false;
        }
    }

    public void updateGun()
    {
        currentGun++;

        gunStars[currentGun].SetActive(true);

        for (int i = 0; i < guns.Length; i++)
        {
            guns[i].SetActive(false);
        }

        guns[1].SetActive(true);

        gunDetails.text = gunDetailsString[currentGun - 1];
        EffectsManager.Instance.setTimeScale(1);

        if (currentGun >= 2)
        {
            Player.damage += 20;
            gunButton.interactable = false;
        }
    }

    public void updateReturner()
    {
        currentReturner++;

        for (int i = 0; i < currentReturner; i++)
        {
            returnerStars[currentReturner - 1].SetActive(true);
        }

        returners[currentReturner - 1].SetActive(true);

        returnerDetails.text = returnerDetailsString[currentReturner - 1];
        EffectsManager.Instance.setTimeScale(1);

        if (currentReturner >= 3)
        {
            returnerButton.interactable = false;
        }
    }

    public void updateThunder()
    {
        currentThunder++;

        for (int i = 0; i < currentThunder; i++)
        {
            thunderStars[currentThunder - 1].SetActive(true);
        }

        thunder_manager.totalThunders += 3;

        if (currentThunder >= 3)
        {
            thunderButton.interactable = false; 
        }
        EffectsManager.Instance.setTimeScale(1);
    }

    public void openRandomUpgrades()
    {
        int randomNumber = Random.Range(0, randomUpgrades.Length);

        for (int i = 0; i < randomUpgrades.Length; i++)
        {
            randomUpgrades[i].SetActive(false);
        }

        for (int i = 0; i < 3; i++)
        {
            randomNumber += 1;

            if (randomNumber >= randomUpgrades.Length)
            {
                randomNumber = 0;
            }

            randomUpgrades[randomNumber].SetActive(true);
        }
    }

}
