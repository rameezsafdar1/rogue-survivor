using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIWaveManager : MonoBehaviour
{
    public GameManager Manager;
    public List<GameObject> Agents = new List<GameObject>(); 
    public float waveTime;
    private float tempTime;
    public float bossShowupTime, incrementForNextBoss;
    public GameObject bossPanel;
    public GameObject boss;
    [HideInInspector]
    public float maxLevelLength;
    private bool levelComplete;
    [HideInInspector]
    public int bossRemaining;
    [HideInInspector]
    public delayTrigger dm;
    public GameObject Player;
    public LayerMask detectionMask;
    public UnityEngine.UI.Image fillImageChapter;
    private float maxSeconds, currentSeconds;
    private void Start()
    {
        for (int i = 0; i < 11; i++)
        {
            Agents[i].SetActive(true);
            Agents.Remove(Agents[i]);
        }
        maxSeconds = maxLevelLength * 60;
    }

    private void Update()
    {
        currentSeconds += Time.deltaTime;
        float fillVal = currentSeconds / maxSeconds;
        fillImageChapter.fillAmount = fillVal;


        if (!levelComplete)
        {
            if (GameManager.Instance.minutes < maxLevelLength)
            {
                if (Agents.Count > 0 && !boss.activeSelf)
                {
                    tempTime += Time.deltaTime;

                    if (tempTime >= waveTime)
                    {
                        if (Agents.Count < 10)
                        {
                            for (int i = 0; i < Agents.Count; i++)
                            {
                                Agents[i].SetActive(true);
                            }
                            Agents.Clear();
                        }

                        else
                        {
                            for (int i = 0; i < 11; i++)
                            {
                                int x = Random.Range(0, Agents.Count - 1);
                                Agents[x].SetActive(true);
                                Agents.Remove(Agents[x]);
                            }
                        }

                        tempTime = 0;
                    }
                }

                if (Manager.minutes >= bossShowupTime && !boss.activeSelf)
                {
                    bossShowupTime = bossShowupTime * incrementForNextBoss;
                    bossPanel.SetActive(true);
                    boss.SetActive(true);
                }
            }
            else
            {
                if (bossRemaining <= 0)
                {
                    levelComplete = true;
                    StartCoroutine(destroyAllEnemies());
                }
            }
        }
    }

    private IEnumerator destroyAllEnemies()
    {
        Collider[] cols = Physics.OverlapSphere(Player.transform.position, 30, detectionMask);

        for (int i = 0; i < cols.Length; i++)
        {
            cols[i].GetComponent<iDamagable>().takeDamage(1000);
            yield return new WaitForSeconds(0.3f);
        }
        dm.callDelayedEvent();
    }

    public void bossDied()
    {
        bossRemaining--;       
    }

}
