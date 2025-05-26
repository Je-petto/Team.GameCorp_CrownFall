using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using DG.Tweening;

public class UIManager : BehaviourSingleton<UIManager>
{
    protected override bool IsDontdestroy() => true;

    [SerializeField] private GameObject gameEndPanel;
    [SerializeField] private TextMeshProUGUI winnerTeamText;

    [Header("Player UI")]
    [SerializeField] private GameObject inGameUIPanel;
    [SerializeField] private TextMeshProUGUI atkText;
    [SerializeField] private Image hpBar;

    [Header("Character Profile")]
    [SerializeField] private Image face;
    [SerializeField] private Image skillIcon;

    private PlayerController playerController;

    private void Start()
    {
        playerController = FindObjectsOfType<PlayerController>().ToList().Find(p => p.isLocalPlayer);

        if (playerController != null)
        {
            atkText.text = playerController.data.name;
        }
        else
        {
            Debug.LogWarning("PlayerController not found!");
        }

        gameEndPanel.SetActive(false);
        if (inGameUIPanel != null)
            inGameUIPanel.SetActive(true); // 게임 시작 시 InGameUI 활성화
    }

    public void ShowGameEndPanel(string winnerTeamName)
    {
        if (winnerTeamText != null)
            winnerTeamText.text = $"{winnerTeamName} TEAM WINS!";

        if (inGameUIPanel != null)
            inGameUIPanel.SetActive(false); // 게임 종료 시 InGameUI 비활성화

        gameEndPanel.SetActive(true);
    }

    // 필요하면 플레이어를 동적으로 바꾸는 함수 추가 가능
    public void SetPlayerController(PlayerController player)
    {
        playerController = player;
        if (playerController != null)
        {
            atkText.text = playerController.data.name;
        }

        face.sprite = player.data.face;
        skillIcon.sprite = player.data.SkillIcon;
    }

    [SerializeField] TextMeshProUGUI timeText;

    public void StartCoolDown(float duration)
    {
        float remainingTime = duration;

        DOTween.To(() => remainingTime, x =>
        {
            remainingTime = x;

            int displayTime = Mathf.CeilToInt(remainingTime); // 올림 (1.9 → 2)
            timeText.text = displayTime.ToString();
        }, 0f, duration)
        .SetEase(Ease.Linear)
        .OnComplete(() => timeText.text = ""); // 끝나면 텍스트 비우기
    }

    public void UpdateHpBar(float percent)
    {
        hpBar.fillAmount = (float)playerController.data.hp / playerController.syncedHp;
    }
}