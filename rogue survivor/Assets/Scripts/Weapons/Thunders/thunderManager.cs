using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class thunderManager : MonoBehaviour
{
    public GameObject[] thunders;
    public GameObject thunderUI;
    public float waitTime;
    private float tempWaitTime;
    public int totalThunders;

    private void Update()
    {
        tempWaitTime += Time.deltaTime;

        if (tempWaitTime >= waitTime && totalThunders > 0)
        {
            for (int i = 0; i < totalThunders; i++)
            {
                thunders[i].SetActive(true);
            }
            thunderUI.SetActive(true);
            tempWaitTime = 0;
        }
    }

}
