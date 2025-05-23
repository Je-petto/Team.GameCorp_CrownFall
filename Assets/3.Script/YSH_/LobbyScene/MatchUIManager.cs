using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MatchUIManager : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Button startMatchButton;
    [SerializeField] private Button cancelMatchButton;
    [SerializeField] private GameObject matchingFx;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI matchComepleteText;

    [Header("Match Complete UI")]
    [SerializeField] private GameObject matchCompletePanel; // "매칭 완료!" 텍스트와 버튼 포함된 패널
    [SerializeField] private Button goToCharacterSelectButton;

    private bool isMatching = false;
    private bool isMatched = false;

    private void Start()
    {
        startMatchButton.onClick.AddListener(OnStartMatch);
        cancelMatchButton.onClick.AddListener(OnCancelMatch);
        goToCharacterSelectButton.onClick.AddListener(OnGoToCharacterSelect);

        // 초기 상태 설정
        cancelMatchButton.gameObject.SetActive(false);
        matchingFx.SetActive(false);
        matchCompletePanel.SetActive(false);
        statusText.text = "Waiting...";
    }

    /// <summary>
    /// 매칭 시작 버튼 클릭 시 호출
    /// </summary>
    public void OnStartMatch()
    {
        if (isMatching) return;
        isMatching = true;

        startMatchButton.interactable = false;
        cancelMatchButton.gameObject.SetActive(true);
        matchingFx.SetActive(true);
        statusText.text = "Matching...";

        Debug.Log(">> 매칭 시작");
        Debug.Log("[UI] 매칭 FX 활성화, 취소 버튼 표시");

        // 테스트용 매칭 완료 시뮬레이션 (2초 후 매칭 완료)
        
        LobbyManager.I.StartMatching();
    }

    /// <summary>
    /// 매칭 취소 버튼 클릭 시 호출
    /// </summary>
    public void OnCancelMatch()
    {
        if (!isMatching || isMatched) return;

        isMatching = false;
        startMatchButton.interactable = true;
        cancelMatchButton.gameObject.SetActive(false);
        matchingFx.SetActive(false);
        statusText.text = "Match Canceled";

        Debug.Log(">> 매칭 취소");
        Debug.Log("[UI] 매칭 FX 비활성화, 매칭 버튼 표시");

        LobbyManager.I.CancelMatching();
    }

    /// <summary>
    /// 테스트용: 매칭 완료 상태 시뮬레이션
    /// </summary>
    private IEnumerator SimulateMatchComplete()
    {
        yield return new WaitForSeconds(2f);

        if (!isMatching) yield break;

        isMatched = true;
        isMatching = false;

        matchCompletePanel.SetActive(true);
        cancelMatchButton.gameObject.SetActive(false);
        matchingFx.SetActive(false);
        matchComepleteText.text = "Match Complete!";

        Debug.Log(">> 매칭 완료됨 (테스트)");
    }

    /// <summary>
    /// 캐릭터 선택 씬으로 이동
    /// </summary>
    public void OnGoToCharacterSelect()
    {
        Debug.Log(">> 캐릭터 선택 씬으로 이동");
        SceneManager.LoadScene("net.2.SelectScene");
    }
}