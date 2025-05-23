using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public class InGameNetworkManager : NetworkManager
{
    string ingameSceneName = "net.3.InGameScene";
    // public override void Awake()
    // {
    //     base.Awake();
    //     onlineScene = ingameSceneName;
    // }

    public override void Awake()
    {
        base.Awake();
        InGameSessionStore.LoadFromArgs();
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Debug.Log("[InGameServer] Add Player...");
        GameObject playerObj = Instantiate(playerPrefab);
        NetworkServer.AddPlayerForConnection(conn, playerObj);
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        Debug.Log($"[InGameServer] Client connected: {conn.address}");

        Debug.Log($"[InGameServer] Changing Scene to: {ingameSceneName}");
        ServerChangeScene(ingameSceneName);                                     // ✅ 이게 호출돼야 씬 이동함
    }

    [Server]
    public override void OnServerSceneChanged(string sceneName)
    {
        Debug.Log($"[InGameServer] Scene Load Complete: {sceneName}");
    }
}

public static class InGameSessionStore
{
    public static Dictionary<string, ClientSession> sessionDict = new();

    public static void LoadFromArgs()
    {
        string[] args = Environment.GetCommandLineArgs();
        string sessionData = args.FirstOrDefault(a => a.StartsWith("-session="))?.Split("=")[1];

        if (!string.IsNullOrEmpty(sessionData))
        {
            string json = Uri.UnescapeDataString(sessionData);
            var wrapper = JsonUtility.FromJson<PlayerSessionList>(json);
            foreach (var p in wrapper.conns)
            {
                sessionDict[p.uid] = p;
            }

            Debug.Log($"[세션] {sessionDict.Count}명 등록됨");
        }
        else
        {
            Debug.LogWarning("[세션] 세션 데이터가 전달되지 않음");
        }
    }
}