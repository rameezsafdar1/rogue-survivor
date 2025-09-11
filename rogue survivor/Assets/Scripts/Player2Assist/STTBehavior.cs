using player2_sdk;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class STTBehavior : MonoBehaviour
{
    [SerializeField] private Button recordBtn;


    public void RecordButtonToggle()
    {
        recordBtn.onClick.Invoke();
    }
}
