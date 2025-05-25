using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : BehaviourSingleton<UIManager>
{
    protected override bool IsDontdestroy() => true;

    [SerializeField] private GameObject gameEndPanel;
    [SerializeField] private TextMeshProUGUI winnerTeamText;

    [Header("Player UI")]
    [SerializeField] private GameObject inGameUIPanel;
    [SerializeField] private TextMeshProUGUI atkText;
    [SerializeField] private Slider hpSlider;

    private PlayerController playerController;

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            hpSlider.maxValue = playerController.data.hp;
            hpSlider.value = playerController.data.hp;
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

    private void Update()
    {
        if (playerController != null)
        {
            hpSlider.value = playerController.currentStat.hp;
        }
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
            hpSlider.maxValue = playerController.data.hp;
            hpSlider.value = playerController.currentStat.hp;
            atkText.text = playerController.data.name;
        }
    }
}

// using UnityEngine;
// using UnityEngine.UI;
// using CustomInspector;
// using TMPro;

// public class UIManager : MonoBehaviour
// {
//     [ReadOnly] public PlayerController playerController;

//     public TextMeshProUGUI atk;
//     public Slider hpSlider;

//     void Start()
//     {
//         playerController = FindObjectOfType<PlayerController>();

//         hpSlider.maxValue = playerController.data.hp;
//         hpSlider.value = playerController.data.hp;
//         atk.text = playerController.data.name.ToString();
//     }

//     private void Update()
//     {
//         hpSlider.value = playerController.currentHp;
//     }
// }