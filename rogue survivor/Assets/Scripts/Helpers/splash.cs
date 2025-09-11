using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class splash : MonoBehaviour
{
    public TextMeshProUGUI loadingText;
    public Image loadingBar;
    private AsyncOperation asyncOperation;
    private float targetValue, currentValue;
    public string loadDialogue1, loadDialogue2;
    public int loadLevel;
    public Animator anim;

    private void OnEnable()
    {
        Time.timeScale = 1;
    }

    private void Start()
    {        
        StartCoroutine(LoadScene());
    }

    private void Update()
    {
        if (asyncOperation != null) 
        {
            targetValue = asyncOperation.progress / 0.9f;
            currentValue = Mathf.MoveTowards(currentValue, targetValue, 3 * Time.deltaTime);
            loadingBar.fillAmount = currentValue;

            if (anim != null)
            {
                anim.SetFloat("Blend", currentValue);
            }

            if (currentValue <= 0.6f)
            {
                loadingText.text = loadDialogue1;
            }

            else
            {
                loadingText.text = loadDialogue2;
            }


            if (Mathf.Approximately(currentValue, 1))
            {
                asyncOperation.allowSceneActivation = true;
            }

        }

    }

    private IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(0.2f);
        asyncOperation = SceneManager.LoadSceneAsync(loadLevel);
        asyncOperation.allowSceneActivation = false;        
    }
}
