using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levelManager : MonoBehaviour
{
    public float maxLengthforLevel;
    public AIWaveManager waveManager;
    public int bossRemaining;
    public delayTrigger delayTrigger;

    private void OnEnable()
    {
        waveManager.maxLevelLength = maxLengthforLevel;
        waveManager.bossRemaining = bossRemaining;
        waveManager.dm = delayTrigger;
    }



}
