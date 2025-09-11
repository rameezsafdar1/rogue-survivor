using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class thirdPersonMovement : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private float speed, rotationSpeed;
    private float gravity = -1f;
    private Vector3 direction;
    private float horizontal, vertical;

    private void Update()
    {
        horizontal = ControlFreak2.CF2Input.GetAxisRaw("Horizontal");
        vertical = ControlFreak2.CF2Input.GetAxisRaw("Vertical");
    }

    private void FixedUpdate()
    {
        movement();
    }

    private void movement()
    {
        

        direction = new Vector3(horizontal, 0, vertical).normalized;
        controller.Move(direction * speed * Time.deltaTime);
        makeGravity();
        rotation();

    }

    private void rotation()
    {
        if (direction.x != 0 || direction.z != 0)
        {
            float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            Quaternion newRot = Quaternion.Euler(0, angle, 0);

            transform.rotation = Quaternion.Slerp(transform.rotation, newRot, rotationSpeed * Time.deltaTime);
        }
    }

    private void makeGravity()
    {
        if (controller.isGrounded)
        {
            gravity = -1;
        }
        else
        {
            gravity -= speed * Time.deltaTime;
        }
        direction = new Vector3(direction.x, gravity, direction.z);
        controller.Move(direction * speed * Time.deltaTime);
    }

}
