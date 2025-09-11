using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Boomerang : baseWeapon
{
    public float speed;
    public Vector3 destOffset;
    private Vector3 destPoint;
    private bool returning = true;
    public UnityEvent onReturnEvent;
    public TMPro.TextMeshPro damageText;

    private void OnEnable()
    {
        returning = false;
        destPoint = transform.localPosition + destOffset;
        damageText.text = damage.ToString();
    }

    private void Update()
    {
        if (!returning)
        {
            if (Vector3.Distance(transform.localPosition, destPoint) <= 0.2f)
            {
                returning = true;
            }
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, destPoint, speed * Time.deltaTime);
        }

        else
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, Vector3.zero, speed * Time.deltaTime);

            if (Vector3.Distance(transform.localPosition, Vector3.zero) <= 0.2f)
            {
                if (onReturnEvent != null)
                {
                    onReturnEvent.Invoke();
                }
            }

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ai")
        {
            impactparticle.transform.parent = EffectsManager.Instance.instParent;
            impactparticle.SetActive(true);
            other.GetComponent<iDamagable>().takeDamage(damage);
        }
    }

}
