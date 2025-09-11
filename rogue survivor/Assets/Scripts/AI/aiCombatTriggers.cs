using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class aiCombatTriggers : MonoBehaviour
{
    public UnityEvent rightHandEvent1, rightHandEvent2;
    public UnityEvent leftHandEvent1, leftHandEvent2;

    public void RHE1()
    {
        rightHandEvent1.Invoke();
    }

    public void RHE2()
    {
        rightHandEvent2.Invoke();
    }

    public void LHE1()
    {
        leftHandEvent1.Invoke();
    }

    public void LHE2()
    {
        leftHandEvent2.Invoke();
    }

}
