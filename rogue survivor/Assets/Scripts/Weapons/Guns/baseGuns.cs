using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class baseGuns : MonoBehaviour
{
    public fieldOfView Fov;
    [Range(0.1f, 2f)]
    public float shootingFrequency;
    protected float tempShootTime;
    public Transform instPoint;
    public Bullet[] bullets;
    protected int currentBullet;
    public Animator anim;
    public Transform child;
    protected Transform closestEnemy;

    public virtual void Update()
    {
        if (Fov.detectedObjects.Count > 0)
        {
            closestEnemy = Fov.closestEnemy();
            Quaternion rot = Quaternion.LookRotation(closestEnemy.position - transform.position);
            rot.x = 0; rot.z = 0;
            child.rotation = Quaternion.Slerp(child.rotation, rot, 10 * Time.deltaTime);

            tempShootTime += Time.deltaTime;

            if (tempShootTime >= shootingFrequency)
            {
                shootingBehavior();
                tempShootTime = 0;
            }
        }
        else
        {
            anim.SetBool("Shoot", false);
            child.localRotation = Quaternion.Slerp(child.localRotation, Quaternion.identity, 10 * Time.deltaTime);
        }

    }

    public virtual void shootingBehavior()
    {
        anim.SetBool("Shoot", true);
        bullets[currentBullet].target = closestEnemy;
        bullets[currentBullet].transform.position = instPoint.position;
        bullets[currentBullet].gameObject.SetActive(true);
        currentBullet++;

        if (currentBullet >= bullets.Length)
        {
            currentBullet = 0;
        }
    }

}

