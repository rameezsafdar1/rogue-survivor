using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class flyToUI : MonoBehaviour
{
    public Transform UIRef;
    public float speed, accuracy, sizeChangeSpeed = 2;
    public Vector3 offset, finalScale = new Vector3(1, 1, 1);
    private Vector3 startScale;
    public UnityEvent onUIreach;

    private void Awake()
    {
        startScale = transform.localScale;
    }

    public virtual void OnEnable()
    {
        transform.parent = GameManager.Instance.Player.transform;
        UIRef.GetComponent<delayTrigger>().callDelayedEvent();
    }

    private void Update()
    {
        Vector3 screenPoint = UIRef.position + offset;
        Vector3 worldPos = EffectsManager.Instance.mainCamera.ScreenToWorldPoint(screenPoint);
        transform.position = Vector3.MoveTowards(transform.position, worldPos, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, worldPos) <= accuracy)
        {
            EffectsManager.Instance.playPickupSound();

            if (onUIreach != null)
            {
                onUIreach.Invoke();
            }

            Destroy(gameObject);
        }

        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.Lerp(transform.localScale, finalScale, sizeChangeSpeed * Time.deltaTime);

    }

    public void setParent()
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = startScale;
    }
}
