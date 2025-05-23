using UnityEngine;
using UnityEngine.UI;

public class PlayerDataPanel : MonoBehaviour
{
    [SerializeField] private Image characterProfileImg;
    [SerializeField] private Text nickname;
    [SerializeField] private Text readyText;

    public UserAuth userInfo;

    void Awake()
    {
        characterProfileImg = GetComponentInChildren<Image>();
        nickname = transform.Find("NicknameText").GetComponent<Text>();
        readyText = transform.Find("ReadyState").GetComponent<Text>();
    }

    public void Init(UserAuth userInfo)
    {
        this.userInfo = userInfo;
        this.nickname.text = userInfo.nickname;
    }

    public void SetCharacterImage(Sprite sprite)
    {
        characterProfileImg.sprite = sprite;
    }

    public void SetReadyState(PlayerMatchState state)
    {
        if (state.Equals(PlayerMatchState.Ready))
        {
            readyText.text = "Ready";
        }
        else
        {
            readyText.text = "";
        }
    }
}