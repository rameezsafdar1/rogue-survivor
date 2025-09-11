using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool debugMode;
    public TextMeshProUGUI totalCashText, totalTimeSpentText, totalKillsText;
    public GameObject Player;
    [HideInInspector]
    public float totalTime;
    private int totalKills;
    [HideInInspector]
    public float minutes;
    public GameObject[] allLevels;
    [HideInInspector]
    public int currentSet;
    public TextMeshProUGUI chapterText;
    public int currentFOV;

    private void Awake()
    {
        allLevels[0].SetActive(true);
    }

    private void Start()
    {
        totalCashText.text = EffectsManager.Instance.currencyShortener(saveManager.Instance.loadCash());
        chapterText.text = "Chapter " + (saveManager.Instance.loadCustomInts("chapter") + 1).ToString();

        currentSet = Random.Range(0, 2);

        if (currentSet == saveManager.Instance.loadCustomInts("bodySet"))
        {
            Debug.Log("set changed");
            currentSet++;
            if (currentSet >= 3)
            {
                currentSet = 0;
            }
        }

        saveManager.Instance.saveCustomInts("bodySet", currentSet);

        if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        Instance = this;
    }

    private void Update()
    {
        totalTime += Time.deltaTime;

        minutes = Mathf.FloorToInt(totalTime / 60);
        float seconds = Mathf.FloorToInt(totalTime % 60);

        totalTimeSpentText.text = minutes.ToString() + "m " + seconds.ToString() + "s";

        EffectsManager.Instance.changeFov(currentFOV);

    }

    public void addCash(int cash)
    {
        saveManager.Instance.addCash(cash);
        totalCashText.text = EffectsManager.Instance.currencyShortener(saveManager.Instance.loadCash());
    }    

    public void killAdded()
    {
        totalKills++;
        totalKillsText.text = EffectsManager.Instance.currencyShortener(totalKills);
    }
}
