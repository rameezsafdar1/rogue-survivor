using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class droneMissile : MonoBehaviour
{
    public Transform target;
    private BaseAIProperties ai;
    private float retargetTime;
    public float speed;
    [HideInInspector]
    public float damage;
    public GameObject impactParticle;
    public TextMeshPro damageText;

    private void OnEnable()
    {
        retargetTime = 0;
        ai = target.GetComponent<BaseAIProperties>(); 
        damageText.transform.parent.gameObject.SetActive(true);
        damageText.text = damage.ToString();
    }

    private void Update()
    {
        retargetTime += Time.deltaTime;

        if (retargetTime >= 0.5f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target.position - transform.position), 10 * Time.deltaTime);
        }

        transform.position += transform.forward * speed * Time.deltaTime;


        if (ai.health <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ai")
        {
            impactParticle.transform.position = transform.position;
            impactParticle.transform.parent = EffectsManager.Instance.instParent;
            impactParticle.SetActive(true);
            other.GetComponent<iDamagable>().takeDamage(damage);
            gameObject.SetActive(false);
        }
    }

}
