using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class BaseAIProperties : MonoBehaviour
{
    public float health, accuracy;
    protected float tempHealth;
    public GameObject wholeBody, deathParticle, deathReward;
    public Animator anim;
    public NavMeshAgent agent;
    public Transform target;
    protected bool isHit;
    [Header("Meshes")]
    public Renderer[] bodyPart;
    protected MaterialPropertyBlock[] propertyBlock;
    public Color col;
    protected Color lerpColor;
    protected float colorChangeTime;

    public virtual void Start()
    {
        tempHealth = health;

        propertyBlock = new MaterialPropertyBlock[bodyPart.Length];

        for (int i = 0; i < propertyBlock.Length; i++)
        {
            propertyBlock[i] = new MaterialPropertyBlock();
            bodyPart[i].GetPropertyBlock(propertyBlock[i]);
            bodyPart[i].gameObject.SetActive(true);
        }

    }

    public void hitComplete()
    {
        isHit = false;
        if (agent.enabled)
        {
            agent.isStopped = false;
        }
        anim.SetBool("isHit", false);
    }

    protected void colorChangeForDamage()
    {
        if (colorChangeTime > 0)
        {
            colorChangeTime -= Time.deltaTime;
            lerpColor = Color.Lerp(lerpColor, col, 60 * Time.deltaTime);
        }
        else
        {
            lerpColor = Color.Lerp(lerpColor, Color.black, 60 * Time.deltaTime);
        }

        for (int i = 0; i < propertyBlock.Length; i++)
        {
            propertyBlock[i].SetColor("_EmissionColor", lerpColor);
            bodyPart[i].SetPropertyBlock(propertyBlock[i]);
        }
    }
}
