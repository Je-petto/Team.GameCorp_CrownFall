using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    #region Matching Event.
    public void OnClickMatchingBtn() => StartMatching();
    public void OnClickCancelBtn() => CancelMatching();

    [SerializeField] Text matchingLog;
    NetworkPlayer networkPlayer;

    void Awake()
    {
        NetworkClient.RegisterHandler<SceneMessage>(OnSceneMessageReceived, false);
    }

    public void StartMatching()
    {
        networkPlayer = NetworkClient.connection.identity.GetComponent<NetworkPlayer>();
        matchingLog.text = "Matching...";
        networkPlayer.CmdRequestStartMatching(true);
    }

    public void CancelMatching()
    {
        networkPlayer = NetworkClient.connection.identity.GetComponent<NetworkPlayer>();
        matchingLog.text = "Not-Matching...";
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
