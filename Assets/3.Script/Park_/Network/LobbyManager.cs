using System.Collections;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : BehaviourSingleton<LobbyManager>
{
    protected override bool IsDontdestroy() => false;

    [SerializeField] GameObject matchingLog;
    NetworkPlayer networkPlayer;

    void Start()
    {
        NetworkClient.RegisterHandler<SceneMessage>(OnSceneMessageReceived, false);
    }

    public void StartMatching()
    {
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

    private void OnSceneMessageReceived(SceneMessage msg)
    {
        Debug.Log($"[Client] Custom scene load: {msg.sceneName}");
        networkPlayer.matchState = PlayerMatchState.Matching;
        StartCoroutine(LoadWaitingScene(msg));
    }

    IEnumerator LoadWaitingScene(SceneMessage msg)
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(msg.sceneName, LoadSceneMode.Single);
    }
}