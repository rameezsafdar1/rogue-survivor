using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blades : baseWeapon
{
    public TMPro.TextMeshPro damagetext;

    private void OnEnable()
    {
        damagetext.text = damage.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ai")
        {
            CinemachineShake.Instance.ShakeCamera(2, 0.1f);
            //Vibration.Vibrate(6);
            impactparticle.transform.parent = EffectsManager.Instance.instParent;
            impactparticle.SetActive(true);
            other.GetComponent<iDamagable>().takeDamage(damage);
        }
    }
}
