using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class activityManager : MonoBehaviour
{
    public float totalEvents;
    private float currentEvents;
    public float delay;
    public UnityEvent onEventComplete;

    public void eventDone()
    {
        currentEvents++;

        if (currentEvents >= totalEvents)
        {
            if (onEventComplete != null && gameObject.activeInHierarchy)
            {
                StartCoroutine(wait());
            }
        }

    }

    private IEnumerator wait()
    {
        yield return new WaitForSeconds(delay);
        onEventComplete.Invoke();
    }


}
