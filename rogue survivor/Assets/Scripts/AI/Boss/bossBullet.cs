using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossBullet : MonoBehaviour
{
    public SharkBoss Parent;
    public float speed;
    public Rigidbody rb;
    private float tempTime;
    [HideInInspector]
    public Transform target;
    public GameObject impactparticle;

    private void OnEnable()
    {
        rb.isKinematic = false;
        impactparticle.transform.parent = this.transform;
        impactparticle.transform.localPosition = Vector3.zero;

        if (target != null)
        {
            Vector3 direction = (target.position - transform.position);
            rb.AddForce(direction.normalized * speed, ForceMode.Impulse);
        }
        else
        {
            rb.AddForce(transform.forward * speed, ForceMode.Impulse);
        }
    }

    private void Update()
    {
        tempTime += Time.deltaTime;

        if (tempTime >= 1f)
        {
            tempTime = 0;
            impactparticle.transform.parent = EffectsManager.Instance.instParent;
            impactparticle.SetActive(true);
            rb.isKinematic = true;
            gameObject.SetActive(false);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            rb.isKinematic = true;
            //Vibration.Vibrate(10);
            impactparticle.transform.parent = EffectsManager.Instance.instParent;
            impactparticle.SetActive(true);
            other.GetComponent<iDamagable>().takeDamage(Parent.damage);
            gameObject.SetActive(false);
        }
    }

}
