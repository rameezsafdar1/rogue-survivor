using UnityEngine;
using UnityEngine.Events;

public class animationBasics : MonoBehaviour
{
    public UnityEvent animationEndEvent;
    [Header("Screen Shake Settings")]
    public float intensity;
    public float duration;
    public bool isPlayer;

    public void turnOff()
    {
        gameObject.SetActive(false);
    }

    public void callEvent()
    {
        if (animationEndEvent != null)
        {
            animationEndEvent.Invoke();
            if (isPlayer)
            {
                callVibration(6);
            }
        }
    }

    public void callVibration(long intensity)
    {
//        Vibration.Vibrate(intensity);
    }

    public void callCameraShake()
    {
        CinemachineShake.Instance.ShakeCamera(intensity, duration);
    }

    public void setPositionDown(float value)
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + value, transform.position.z);
    }
}
