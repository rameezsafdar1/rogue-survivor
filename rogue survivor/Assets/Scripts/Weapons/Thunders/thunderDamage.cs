using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class thunderDamage : baseWeapon
{
    private Collider col;
    public TMPro.TextMeshPro damageText;

    private void OnEnable()
    {
        damageText.text = damage.ToString();
        if (col != null)
        {
            col.enabled = true;
        }
    }
    private void Start()
    {
        col = transform.GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ai")
        {
            impactparticle.transform.parent = EffectsManager.Instance.instParent;
            impactparticle.SetActive(true);
            other.GetComponent<iDamagable>().takeDamage(damage);
            col.enabled = false;
        }
    }
}
