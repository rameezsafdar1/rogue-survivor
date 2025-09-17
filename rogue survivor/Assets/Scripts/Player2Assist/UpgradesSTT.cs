using player2_sdk;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UpgradesSTT : MonoBehaviour
{
    [SerializeField] private Player2STT behavior;
    [SerializeField] private TextMeshProUGUI perkStatusText;
    [SerializeField] private GameObject upgradesPanel;
    [SerializeField] private upgradesManager upgradesManager;
    [SerializeField] private playerStats player;

    public void CallUpgrade(string speech)
    {
        string normalized = speech.ToLower().Trim().Trim('.', '!', '?');
        Debug.LogWarning("Called with speech: " + normalized);

        if (upgradesPanel.activeSelf)
        {
            if (normalized.Contains("drone"))
            {
                upgradesManager.updateDrone();
                upgradesPanel.SetActive(false);
                return;
            }

            if (normalized.Contains("blades"))
            {
                upgradesManager.updateBlade();
                upgradesPanel.SetActive(false);
                return;
            }

            if (normalized.Contains("gun"))
            {
                upgradesManager.updateGun();
                upgradesPanel.SetActive(false);
                return;
            }

            if (normalized.Contains("boomerang"))
            {
                upgradesManager.updateReturner();
                upgradesPanel.SetActive(false);
                return;
            }

            if (normalized.Contains("thunder"))
            {
                upgradesManager.updateThunder();
                upgradesPanel.SetActive(false);
                return;
            }

            if (normalized.Contains("health"))
            {
                player.increaseHealth();
                upgradesPanel.SetActive(false);
                return;
            }

            string[] funnyPhrases = new string[]
            {
            "Boss, I could not find ",
            "Are you sure you want ",
            "Pay attention to the game, why do you need ",
            "Nope, not seeing any "
            };


            string randomPhrase = funnyPhrases[Random.Range(0, funnyPhrases.Length)];

            behavior.statusText.text = behavior.statusText.text.Replace("Equipping ", randomPhrase);
        }

        else
        {
            if (normalized.Contains("freeze"))
            {
                EffectsManager.Instance.Freeze();
                behavior.StopSTT();
                perkStatusText.text = "Freeze enabled"; 
                StartCoroutine(resetText());
                return;
            }

            if (normalized.Contains("chaos"))
            {
                EffectsManager.Instance.Chaos();
                behavior.StopSTT(); 
                perkStatusText.text = "Chaos enabled";
                StartCoroutine(resetText());
                return;
            }

            if (normalized.Contains("health"))
            {
                player.HealComplete();
                behavior.StopSTT();
                perkStatusText.text = "Health restored to maximum";
                StartCoroutine(resetText());
                return;
            }

            behavior.StopSTT();
            string[] funnyPhrases = new string[]
            {
            "What do you mean by ",
            "Are you sure you want ",
            "I don't think we have ",
            "Nope, not seeing any ",
            "I know nothing about ",
            "Ummm... what is ",
            };


            string randomPhrase = funnyPhrases[Random.Range(0, funnyPhrases.Length)];
            perkStatusText.text = randomPhrase + normalized;
        }
    }

    public void PerkFail(string failReason)
    {
        behavior.StopSTT();
        perkStatusText.text = failReason;
        StartCoroutine(resetText());
    }

    private IEnumerator resetText()
    {
        yield return new WaitForSeconds(2f);
        perkStatusText.text = "Tap to talk";
    }

}
