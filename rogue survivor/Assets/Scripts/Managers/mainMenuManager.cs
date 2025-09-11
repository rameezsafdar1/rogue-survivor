using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mainMenuManager : MonoBehaviour
{
    public int levelNumber;
    public splash Splash;

    public void setLevelNumber()
    {
        levelNumber = saveManager.Instance.loadCustomInts("currentLevel");
    }

    public void setScene(int sceneNumber)
    {
        Splash.loadLevel = sceneNumber;
        Splash.gameObject.SetActive(true);
    }

    public void startLatestBattle()
    {
        int x = saveManager.Instance.loadCustomInts("currentLevel");

        if (x < 1)
        {
            //levelNumber = x;
            setScene(2);
        }


        if (x == 1)
        {
            //levelNumber = x;
            setScene(3);
        }

        if (x == 2)
        {
            //levelNumber = x;
            setScene(4);
        }
    }
}
