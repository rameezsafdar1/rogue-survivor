using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class autoOff : MonoBehaviour
{
    public float delay;
    private float tempTime;
    public UnityEvent onOff;

    private void OnEnable()
    {
        tempTime = 0;
    }

    private void Update()
    {
        tempTime += Time.deltaTime;

        if (tempTime >= delay)
        {
            tempTime = 0;
            if (onOff != null)
            {
                onOff.Invoke();
            }
            gameObject.SetActive(false);
        }
    }

    public void resetTime()
    {
        tempTime = 0;
    }
}
