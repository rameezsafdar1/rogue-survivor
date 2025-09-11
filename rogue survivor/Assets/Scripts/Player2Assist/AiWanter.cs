using UnityEngine;
using UnityEngine.AI;

public class AIWanter : MonoBehaviour
{
    public float wanderRadius = 10f;   
    public float wanderDelay = 1f;     

    private NavMeshAgent agent;
    private float remainingDistanceThreshold = 0.5f; 
    private bool isSearching = false;
    private Animator anim;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        SetNewDestination();
    }

    void Update()
    {
        if (!agent.pathPending && agent.remainingDistance <= remainingDistanceThreshold && !isSearching)
        {
            StartCoroutine(FindNewPointAfterDelay());
        }

        anim.SetFloat("Velocity", agent.velocity.magnitude);
    }

    System.Collections.IEnumerator FindNewPointAfterDelay()
    {
        isSearching = true;
        yield return new WaitForSeconds(wanderDelay);
        SetNewDestination();
        isSearching = false;
    }

    void SetNewDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }
}
