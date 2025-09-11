using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class touchHit : MonoBehaviour
{
    public float damage;
    public Collider col;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<iDamagable>().takeDamage(damage);
            col.enabled = false;
        }
    }
}
