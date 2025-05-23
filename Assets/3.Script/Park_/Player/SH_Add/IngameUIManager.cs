using UnityEngine;
using TMPro;

public class IngameUIManager : BehaviourSingleton<IngameUIManager>
{
    public CanvasGroup respawnCanvasGroup;
    public TextMeshProUGUI respawnText;

    void Start()
    {
        Init();
    }

    void Init()
    {
        respawnCanvasGroup.alpha = 0;
        respawnCanvasGroup.blocksRaycasts = false;
    }

    public void ShowRespawnUI(float duration)
    {
        respawnCanvasGroup.alpha = 0.3f;
        respawnCanvasGroup.blocksRaycasts = true;
        respawnText.text = $"Respawning in {duration:F0}s...";
    }

    public void UpdateRespawnTime(float timeLeft)
    {
        respawnText.text = $"Respawning in {timeLeft:F0}s...";
    }

    public void HideRespawnUI()
    {
        Debug.Log("ui 숨기기");
        respawnCanvasGroup.alpha = 0;
        respawnCanvasGroup.blocksRaycasts = false;
    }

    protected override bool IsDontdestroy() => false;
}