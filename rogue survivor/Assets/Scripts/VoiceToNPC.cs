//using UnityEngine;
//using UnityEngine.Windows.Speech;
//using player2_sdk;

//public class VoiceToNPC : MonoBehaviour
//{
//    public Player2Npc player2Npc;  

//    private DictationRecognizer dictationRecognizer;

//    void Start()
//    {
//        dictationRecognizer = new DictationRecognizer();

//        dictationRecognizer.DictationResult += OnDictationResult;
//        dictationRecognizer.DictationHypothesis += text => Debug.Log("Listening: " + text);
//        dictationRecognizer.DictationComplete += OnDictationComplete;
//        dictationRecognizer.DictationError += OnDictationError;

//        StartDictation();
//    }

//    private void OnDictationResult(string text, ConfidenceLevel confidence)
//    {
//        if (string.IsNullOrWhiteSpace(text)) return;

//        Debug.Log("You said: " + text);
         
//        //player2Npc?.ReceiveVoiceInput(text);
//        EffectsManager.Instance.xpIncreased(openMenu: false);
//        EffectsManager.Instance.CloseUpgradeMenu();
//        Time.timeScale = 1f; 
//    }

//    private void OnDictationComplete(DictationCompletionCause cause)
//    {
//        Debug.Log("Dictation completed: " + cause);
         
//        if (cause != DictationCompletionCause.Complete)
//        {
//            RestartDictation();
//        }
//        else
//        {
//            RestartDictation();  
//        }
//    }

//    private void OnDictationError(string error, int hresult)
//    {
//        Debug.LogWarning("Dictation error: " + error + " (HRESULT: " + hresult + ")");
//        RestartDictation();
//    }

//    private void RestartDictation()
//    {
//        StopDictation();
//        StartDictation();
//    }

//    private void StartDictation()
//    {
//        if (dictationRecognizer != null && dictationRecognizer.Status != SpeechSystemStatus.Running)
//        {
//            try
//            {
//                dictationRecognizer.Start();
//                Debug.Log("Dictation started.");
//            }
//            catch (UnityException ex)
//            {
//                Debug.LogWarning("DictationRecognizer could not start: " + ex.Message);
//            }
//        }
//    }

//    private void StopDictation()
//    {
//        if (dictationRecognizer != null && dictationRecognizer.Status == SpeechSystemStatus.Running)
//        {
//            dictationRecognizer.Stop();
//            Debug.Log("Dictation stopped.");
//        }
//    }

//    private void Update()
//    { 
//        if (dictationRecognizer != null && dictationRecognizer.Status != SpeechSystemStatus.Running)
//        {
//            StartDictation();
//        }
//    }

//    private void OnDestroy()
//    {
//        if (dictationRecognizer != null)
//        {
//            StopDictation();
//            dictationRecognizer.Dispose();
//        }
//    }
//}
