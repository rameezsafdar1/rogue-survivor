using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movementAnimationController : MonoBehaviour
{
    public Animator anim;
    public CharacterController controller;   
    

    private void Update()
    {
        anim.SetFloat("Velocity", controller.velocity.magnitude);
    }

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();

        if (rb != null && rb.linearVelocity.magnitude < 2f)
        {
            rb.AddForce(transform.forward* 20f, ForceMode.Impulse);
        }

    }

}
