using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public struct bodySets
{
    public Renderer[] parts;
}

public class boxer : BaseAIProperties, iDamagable
{
    public AIWaveManager Manager;
    [NonReorderable]
    public bodySets[] bodyPartSets;


    public override void Start()
    {
        int currentSet = GameManager.Instance.currentSet;

        bodyPart = new Renderer[bodyPartSets[currentSet].parts.Length];

        for (int i = 0; i < bodyPart.Length; i++)
        {
            bodyPart[i] = bodyPartSets[currentSet].parts[i];
        }


        base.Start();
    }

    private void OnEnable()
    {
        agent.enabled = true;
        transform.tag = "Ai";
        gameObject.layer = 6;
        isHit = false;
        anim.SetBool("isHit", false);
        wholeBody.SetActive(true);
        if (tempHealth > 0)
        {
            health = tempHealth;
        }
    }

    private void Update()
    {
        if (EffectsManager.Instance.frozen)
        {
            return;
        }


        colorChangeForDamage();
        if (health > 0 && !isHit)
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
        if (health > 0)
        {
            isHit = true;
            colorChangeTime = 0.1f;
            agent.velocity = Vector3.zero;
            agent.isStopped = true;
            agent.ResetPath();
            anim.SetBool("isHit", true);
            health -= damage;
            if (health <= 0)
            {
                death();
            }
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
        Instantiate(deathReward, transform.position, Quaternion.identity);
        StartCoroutine(wait());
        wholeBody.SetActive(false);
    }

    private IEnumerator wait()
    {
        yield return new WaitForSeconds(3.5f);
        Manager.Agents.Add(agent.gameObject);
        agent.gameObject.SetActive(false);
    }
}
