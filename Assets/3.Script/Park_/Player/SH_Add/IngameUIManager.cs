using System.Collections;
using UnityEngine;
using TMPro;

public class IngameUIManager : MonoBehaviour
{
    public static IngameUIManager Instance;
    public CanvasGroup respawnCanvasGroup;
    public TextMeshProUGUI respawnText;

    void Awake()
    {
        Instance = this;
    }

    public void ShowRespawnUI(float duration)
    {
        respawnCanvasGroup.alpha = 1;
        respawnCanvasGroup.blocksRaycasts = true;
        respawnText.text = $"Respawning in {duration:F0}s...";
    }

    public void UpdateRespawnTime(float timeLeft)
    {
        respawnText.text = $"Respawning in {timeLeft:F0}s...";
    }

    public void HideRespawnUI()
    {
        respawnCanvasGroup.alpha = 0;
        respawnCanvasGroup.blocksRaycasts = false;
    }
}