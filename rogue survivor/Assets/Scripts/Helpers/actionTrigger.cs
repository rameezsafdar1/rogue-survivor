using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class actionTrigger : MonoBehaviour
{
    public string triggerTag;
    public UnityEvent onEnterEvent, onExitEvent;
    public bool dontDetect;

    public virtual void OnTriggerEnter(Collider other)
    {
        if (!dontDetect)
        {
            if (other.tag == triggerTag)
            {
                if (onEnterEvent != null)
                {
                    onEnterEvent.Invoke();
                }
            }
        }
    }

    public virtual void OnTriggerExit(Collider other)
    {
        if (!dontDetect)
        {
            if (other.tag == triggerTag)
            {
                if (onExitEvent != null)
                {
                    onExitEvent.Invoke();
                }
            }
        }
    }

    public void setDetectionOn() 
    {
        dontDetect = false;
    }

    public void setDetectionOff()
    {
        dontDetect = true;
    }


}
