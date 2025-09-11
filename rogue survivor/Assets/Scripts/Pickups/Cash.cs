using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cash : MonoBehaviour
{
    public void addUserXp()
    {
        EffectsManager.Instance.xpIncreased();
        GameManager.Instance.addCash(5);
        Destroy(gameObject);
    }
}
