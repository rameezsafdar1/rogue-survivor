using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class looker : MonoBehaviour
{
    private void Update()
    {
        //transform.LookAt(EffectsManager.Instance.mainCamera.transform, Vector3.up);

        Quaternion rot = Quaternion.LookRotation(EffectsManager.Instance.mainCamera.transform.position - transform.position);
        rot.x = 0; rot.z = 0;

        transform.rotation = rot;
    }
}
