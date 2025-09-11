using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Bgmixer : MonoBehaviour
{
    public AudioSource BGM;
    public UnityEvent onAudioEnd;

    private void Update()
    {
        if (!BGM.isPlaying)
        {
            if (onAudioEnd != null)
            {
                onAudioEnd.Invoke();
            }
        }
    }

}
