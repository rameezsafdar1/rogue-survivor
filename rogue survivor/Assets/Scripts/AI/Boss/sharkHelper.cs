using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sharkHelper : MonoBehaviour
{
    public SharkBoss mainHandler;

    public void attack()
    {
        mainHandler.shootMissiles();
    }

    public void dizzinessOver()
    {
        mainHandler.resetSemiHealth();
    }

}
