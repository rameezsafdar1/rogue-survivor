using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour
{
    public fieldOfView fov;
    public Transform target, rotationReference;
    public float speed, shootingFrequency, damage;
    private float tempTime;
    private bool shootCompleted = true;
    public droneMissile[] bullets;
    [SerializeField]
    private int currentBullet;
    public Transform[] instPoints;
    public int shootingCount;

    private void OnEnable()
    {
        tempTime = shootingFrequency;
    }

    private void FixedUpdate()
    {
        transform.rotation = rotationReference.rotation;
        transform.position = Vector3.Lerp(transform.position, target.position, speed * Time.deltaTime);

        if (shootCompleted && fov.detectedObjects.Count > 0)
        {
            tempTime += Time.deltaTime;
            if (tempTime >= shootingFrequency)
            {
                shootCompleted = false;
                tempTime = 0;
                StartCoroutine(shootMissiles());
            }
        }
    }

    private IEnumerator shootMissiles()
    {
        for (int i = 0; i < shootingCount; i++)
        {
            if (fov.detectedObjects.Count > 0)
            {
                yield return new WaitForSeconds(0.2f);
                int randomNumber1 = Random.Range(0, fov.detectedObjects.Count);

                if (randomNumber1 >= fov.detectedObjects.Count)
                {
                    shootCompleted = true;
                    break;
                }
                bullets[currentBullet].transform.position = instPoints[0].position;
                bullets[currentBullet].damage = damage;
                bullets[currentBullet].target = fov.detectedObjects[randomNumber1];
                bullets[currentBullet].gameObject.SetActive(true);
                currentBullet++;
                if (currentBullet >= bullets.Length)
                {
                    currentBullet = 0;
                }

                bullets[currentBullet].transform.position = instPoints[1].position;
                bullets[currentBullet].damage = damage;

                int randomNumber = Random.Range(0, fov.detectedObjects.Count);

                if (randomNumber >= fov.detectedObjects.Count)
                {
                    shootCompleted = true;
                    break;
                }
                bullets[currentBullet].target = fov.detectedObjects[randomNumber];
                bullets[currentBullet].gameObject.SetActive(true);
                currentBullet++;
                if (currentBullet >= bullets.Length)
                {
                    currentBullet = 0;
                }
            }
        }
        shootCompleted = true;
    }

}
