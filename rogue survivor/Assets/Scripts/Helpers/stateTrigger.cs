using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class stateTrigger : MonoBehaviour
{
    public float onEnableDelay;
    public UnityEvent onEnableEvent, onDisableEvent;


    private void OnEnable()
    {
        if (onEnableEvent != null)
        {
            StartCoroutine(wait());
        }
    }

    private IEnumerator wait()
    {
        yield return new WaitForSeconds(onEnableDelay);
        onEnableEvent.Invoke();
    }

    private void OnDisable()
    {
        if (onDisableEvent != null)
        {
            onDisableEvent.Invoke();
        }
    }

}
