using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class magnet : MonoBehaviour
{
    [Range(0.1f, 2f)]
    public float detectionFrequency;
    public float radius;
    public LayerMask detectionMask;
    public Transform target;
    private void Start()
    {
        StartCoroutine(detect());
    }

    private void Update()
    {
        target.rotation = Quaternion.identity;
    }

    private IEnumerator detect()
    {
        while (true)
        {
            yield return new WaitForSeconds(detectionFrequency);

            Collider[] cols = Physics.OverlapSphere(transform.position, radius, detectionMask);

            for (int i = 0; i < cols.Length; i++)
            {
                cols[i].transform.parent = target;
                cols[i].GetComponent<curveFollower>().setMyTarget(target.localPosition);
                cols[i].gameObject.layer = 0;
            }

        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
