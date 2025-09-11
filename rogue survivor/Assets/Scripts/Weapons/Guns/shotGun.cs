using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shotGun : baseGuns
{
    public Transform[] instPointsAdditional;

    public override void shootingBehavior()
    {
        if (currentBullet >= bullets.Length)
        {
            currentBullet = 0;
        }
        anim.SetBool("Shoot", true);
        bullets[currentBullet].target = closestEnemy;
        bullets[currentBullet].transform.position = instPoint.position;
        bullets[currentBullet].gameObject.SetActive(true);
        currentBullet++;

        for (int i = 0; i < instPointsAdditional.Length; i++)
        {
            if (currentBullet >= bullets.Length)
            {
                currentBullet = 0;
            }
            bullets[currentBullet].target = null;
            bullets[currentBullet].transform.position = instPointsAdditional[i].position;
            bullets[currentBullet].transform.rotation = instPointsAdditional[i].rotation;
            bullets[currentBullet].gameObject.SetActive(true);
            currentBullet++;
        }
        
        tempShootTime = 0;
    }

}
