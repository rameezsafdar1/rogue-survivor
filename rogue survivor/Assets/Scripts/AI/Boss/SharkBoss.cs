using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class SharkBoss : BaseAIProperties, iDamagable
{

    public GameObject[] instpoints, Bullets;
    private int shootLoopNumber;
    private int currentBulletNumber, instNumber;
    private float healthLoss;
    public float damage;
    public GameObject dizzinessPar;
    public GameObject cashPrefab;
    public Transform[] cashtargets;
    private bool xpGiven;
    public UnityEngine.UI.Image healthBar;
    public UnityEvent onDead;

    public override void Start()
    {
        base.Start();
        shootLoopNumber = 2;
        tempHealth = health;
        GameManager.Instance.currentFOV = 70;
    }

    private void Update()
    {
        colorChangeForDamage();

        if (health > 0 && healthLoss < 500)
        {
            float distance = Vector3.Distance(agent.transform.position, target.transform.position);
            if (distance > accuracy)
            {
                if (agent.isStopped)
                {
                    agent.isStopped = false;
                }
                anim.SetBool("Attack", false);
                agent.SetDestination(target.position);
            }
            else
            {
                if (!agent.isStopped)
                {
                    agent.velocity = Vector3.zero;
                    agent.isStopped = true;
                    agent.ResetPath();
                    anim.SetBool("Attack", true);
                }
                agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, Quaternion.LookRotation(target.position - agent.transform.position), 10 * Time.deltaTime);
            }
        }
    }

    public void takeDamage(float damage)
    {
        healthLoss += damage;

        if (healthLoss >= 500 && !xpGiven)
        {
            for (int i = 0; i < 20; i++)
            {
                EffectsManager.Instance.xpIncreased();
            }

            for (int i = 0; i < cashtargets.Length; i++)
            {
                GameObject go = Instantiate(cashPrefab, transform.position, Quaternion.identity);
                go.GetComponent<curveFollower>().setMyTarget(cashtargets[i].position);
                Destroy(go, 5);
            }

            dizzinessPar.SetActive(true);
            anim.SetBool("Dizzy", true);

            xpGiven = true;

        }

        colorChangeTime = 0.1f;
        agent.velocity = Vector3.zero;
        agent.isStopped = true;
        agent.ResetPath();
        health -= damage;
        healthBar.fillAmount = health / tempHealth;
        if (health <= 0)
        {

            death();
        }

    }

    private void death()
    {
        GameManager.Instance.killAdded();
        gameObject.tag = "Untagged";
        gameObject.layer = 0;
        agent.isStopped = true;
        agent.ResetPath();
        agent.enabled = false;
        deathParticle.SetActive(true);
        if (onDead != null)
        {
            onDead.Invoke();
        }
        Instantiate(deathReward, transform.position, Quaternion.identity);
        StartCoroutine(wait());
        wholeBody.SetActive(false);
    }

    private IEnumerator wait()
    {
        yield return new WaitForSeconds(3.5f);
        agent.gameObject.SetActive(false);
    }

    public void shootMissiles()
    {
        if (health > 0)
        {
            for (int i = 0; i < shootLoopNumber; i++)
            {
                Bullets[currentBulletNumber].transform.position = instpoints[instNumber].transform.position;
                Bullets[currentBulletNumber].transform.parent = EffectsManager.Instance.instParent;
                Bullets[currentBulletNumber].transform.rotation = Quaternion.LookRotation(target.position - instpoints[instNumber].transform.position);
                Bullets[currentBulletNumber].SetActive(true);
                instNumber++;
                currentBulletNumber++;

                if (instNumber >= 2)
                {
                    instNumber = 0;
                }

                if (currentBulletNumber >= Bullets.Length)
                {
                    currentBulletNumber = 0;
                }

            }
        }
    }

    public void resetSemiHealth()
    {
        healthLoss = 0;
        dizzinessPar.SetActive(false);
        anim.SetBool("Dizzy", false);
        xpGiven = false;
    }
}
