using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class transformSetter : MonoBehaviour
{
    public Transform newPosition;

    public void setPosRot()
    {
        transform.position = newPosition.position;
        transform.rotation = newPosition.rotation;
    }
}
