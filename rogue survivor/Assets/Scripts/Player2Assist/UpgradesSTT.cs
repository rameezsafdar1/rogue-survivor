using player2_sdk;
using UnityEngine;

public class UpgradesSTT : MonoBehaviour
{
    [SerializeField] private Player2STT behavior;
    [SerializeField] private GameObject upgradesPanel;
    [SerializeField] private upgradesManager upgradesManager;
    [SerializeField] private playerStats player;

    public void CallUpgrade(string speech)
    {
        if (!upgradesPanel.activeSelf)
        {
            return;
        }

        string normalized = speech.ToLower().Trim().Trim('.', '!', '?');
        Debug.LogWarning("Called with speech: " + normalized);

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

        if (normalized.Contains("returner"))
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
}
