using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class checkUnlock : MonoBehaviour
{
    public string myName;
    public UnityEvent unlockEvent, afterUnlockEvent;

    private void Start()
    {
        if (saveManager.Instance.loadCustomFloats(myName) > 0)
        {
            if (unlockEvent != null)
            {
                unlockEvent.Invoke();
            }
        }
    }

    public void buyProduct()
    {
        saveManager.Instance.saveCustomFloats(myName, 1);
        if (unlockEvent != null)
        {
            unlockEvent.Invoke();
        }
    }

    public void afterUnlock()
    {
        if (afterUnlockEvent != null)
        {
            afterUnlockEvent.Invoke();
        }
    }


}
