using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class EffectsManager : MonoBehaviour
{
    public static EffectsManager Instance;
    public Camera mainCamera;
    public CinemachineVirtualCamera lookCamera;
    [SerializeField]
    private AudioSource[] pooledAudioSource;
    private int currentPooledAudio;
    private float tempPitchTime;
    [Header("Instantiate Settings")]
    public Transform instParent;
    [Header("User Level Settings")]
    public Image userLevelFill;
    private int currentXp, maxXp;
    private int userLevel = 1;
    public TextMeshProUGUI userLevelText;
    public AIWaveManager wavemanager;
    public GameObject upgradesMenu;
    public int scene;
    public splash Splash;

    private void Start()
    {
        maxXp = userLevel * 10;
        userLevelFill.fillAmount = (float)currentXp / (float)maxXp;
        userLevelText.text = userLevel.ToString();
        if (Instance != null)
        {
            Debug.Log("There was an instance");
            Destroy(this.gameObject);
        }
        Instance = this;
        Debug.Log("I am an instance");
    }

    private void Update()
    {
        if (currentPooledAudio > 0)
        {
            tempPitchTime += Time.deltaTime;

            if (tempPitchTime >= 0.5f)
            {
                tempPitchTime = 0;
                currentPooledAudio = 0;
            }
        }
    }


    public void changeFov(float value)
    {
        lookCamera.m_Lens.FieldOfView = Mathf.Lerp(lookCamera.m_Lens.FieldOfView, value, 1.5f * Time.deltaTime);
    }

    public void playPickupSound()
    {
        tempPitchTime = 0;
        pooledAudioSource[currentPooledAudio].Play();
        currentPooledAudio++;

        if (currentPooledAudio >= pooledAudioSource.Length)
        {
            currentPooledAudio = pooledAudioSource.Length - 1;
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


    public void CloseUpgradeMenu()
    {
        if (upgradesMenu != null && upgradesMenu.activeSelf)
        {
            upgradesMenu.SetActive(false);
            Debug.Log("Upgrade menu closed by EffectsManager.");
        }
    }


    public void xpIncreased(bool openMenu = true)

    {
        currentXp++;
        userLevelFill.fillAmount = (float)currentXp / (float)maxXp;

        if (currentXp >= maxXp)
        {
            if (wavemanager.waveTime > 3)
            {
                wavemanager.waveTime -= 3;
            }
            userLevel++;
            currentXp = 0;
            maxXp = userLevel * 10;
            userLevelText.text = userLevel.ToString();
            //upgradesMenu.SetActive(true);   

            if (openMenu)
                upgradesMenu.SetActive(true);

        }
    }

    public void setTimeScale(float f)
    {
        Time.timeScale = f;
    }

    public void changeScene(int n)
    {
        scene++;
        SceneManager.LoadScene(n);
    }
    public void setScene(int sceneNumber)
    {
        Splash.loadLevel = sceneNumber;
        Splash.gameObject.SetActive(true);
    }

}
