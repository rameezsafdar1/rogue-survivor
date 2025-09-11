using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class delayTrigger : MonoBehaviour
{
    public float delay;
    public UnityEvent delayEvent;
    private bool cancelled;
    
    public void callDelayedEvent()
    {
        if (delayEvent != null && gameObject.activeInHierarchy)
        {
            StartCoroutine(wait());
        }
    }

    public void callInstantEvent()
    {
        if (delayEvent != null)
        {
            delayEvent.Invoke();
        }
    }

    private IEnumerator wait()
    {
        yield return new WaitForSeconds(delay);
        delayEvent.Invoke();
    }

    public void addButtonEvents(Button btn)
    {
        if (!cancelled)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => delayEvent.Invoke());
            GameObject go = GameObject.FindGameObjectWithTag("Click");
            btn.onClick.AddListener(() => go.GetComponent<AudioSource>().Play());
        }
    }

    public void cancelButtonEvents()
    {
        cancelled = true;
    }


}
