using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameNetworkManager : NetworkManager
{
    string ingameSceneName = "net.3.InGameScene";

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        base.OnServerConnect(conn);

        Debug.Log($"[InGameServer] Client connected: {conn.address}");

        // 클라이언트가 1명 이상 접속하면 씬 변경 (혹은 조건을 설정할 수도 있음)
        if (numPlayers >= 1 && SceneManager.GetActiveScene().name != ingameSceneName)
        {
            Debug.Log($"[InGameServer] Changing Scene to: {ingameSceneName}");
            ServerChangeScene(ingameSceneName);
        }
    }
}