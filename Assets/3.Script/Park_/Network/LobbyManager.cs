using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    #region Matching Event.
    public void OnClickMatchingBtn() => StartMatching();
    public void OnClickCancelBtn() => CancelMatching();

    [SerializeField] GameObject matchingLog;
    NetworkPlayer networkPlayer;

    [SerializeField] GameObject matchingStateComponent;

    void Awake()
    {
        NetworkClient.RegisterHandler<SceneMessage>(OnSceneMessageReceived, false);
        matchingLog.SetActive(false);
    }

    public void StartMatching()
    {
        // 로비에서는 유일한 컴포넌트이다.
        networkPlayer = NetworkClient.connection.identity.GetComponent<NetworkPlayer>();
        matchingLog.gameObject.SetActive(true);
        networkPlayer.CmdRequestStartMatching(true);
    }

    public void CancelMatching()
    {
        networkPlayer = NetworkClient.connection.identity.GetComponent<NetworkPlayer>();
        matchingLog.gameObject.SetActive(false);
        networkPlayer.CmdRequestStartMatching(false);
    }
    #endregion
    
    private void OnSceneMessageReceived(SceneMessage msg)
    {
        Debug.Log($"[Client] Custom scene load: {msg.sceneName}");
        networkPlayer.matchState = PlayerMatchState.Matching;
        SceneManager.LoadScene(msg.sceneName, LoadSceneMode.Single);
    }
}