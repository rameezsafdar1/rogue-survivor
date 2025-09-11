using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotator : MonoBehaviour
{
    public Vector3 angles;

    private void Update()
    {
        transform.Rotate(angles.x * Time.deltaTime, angles.y * Time.deltaTime, angles.z * Time.deltaTime);
    }

}
