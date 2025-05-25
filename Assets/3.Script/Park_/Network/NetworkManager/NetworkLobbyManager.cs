using System;
using System.Collections.Generic;
using System.IO;
using Mirror;
using UnityEngine;

public struct ClientSession
{
    public string uid;
    public string nickname;
    public string selected_cid;
}

public class NetworkLobbyManager : NetworkRoomManager
{
    // 로비에 로그인 되어있는 유저들. => 인게임 서버에서 로비로 다시 돌아 갈때 사용할 예정
    public List<UserAuth> cachedUser;

    public ClientSession clientSession;

    #region Sub-Component
    public MatchManager matchManager;
    public NetworkHandler networkHandler;
    #endregion

    public override void OnStartServer()
    {
        Debug.Log("Server Start");
        base.OnStartServer();

        matchManager = GetComponentInChildren<MatchManager>();
        networkHandler = GetComponentInChildren<NetworkHandler>();

        if (matchManager == null)
        {
            Debug.Log("matchManager is null..");
        }
    }

    public void StartClientHandler()
    {
        networkHandler.StartClient();
    }

    [Server]
    public override void OnRoomServerAddPlayer(NetworkConnectionToClient conn)
    {
        // RoomPlayer -> 여기서는 NetworkPlayer 가 생성되는 부분
        Debug.Log("[Server] : new Client On Server!");
        base.OnRoomServerAddPlayer(conn);

        var player = conn.identity.GetComponent<NetworkPlayer>();
    }

    [Server]
    public void StartMatching(NetworkConnectionToClient conn, bool on)
    {
        Debug.Log("[Receive] : Matching Request");

        if (matchManager == null)
        {
            Debug.Log("matchManager is null..");
            return;
        }

        if (on)
        {
            Debug.Log($"conn {conn.address} Enqueue Match");
            matchManager.AddToMatchList(conn);
        }
        else
        {
            Debug.Log($"conn {conn.address} Dequeue Match");
            matchManager.RemoveToMatchList(conn);
        }
    }

    public override void OnRoomServerSceneChanged(string sceneName)
    {
        Debug.Log($"[Server] Scene changed to: {sceneName}");
    }
}

public class GameSpawner
{
    private static int basePort = 8000;

    public static (System.Diagnostics.Process process, int port) StartInGameServer(Guid matchId)
    {
        int port = GetAvailablePort(); 

        var process = new System.Diagnostics.Process();
        process.StartInfo.FileName = "D:/Project/LocalGit/Team.GameCorp_CrownFall/Builds/InGameServer/Team.GameCorp_CrownFall.exe"; // 빌드된 서버 실행파일

        if (!File.Exists(process.StartInfo.FileName))
        {
            Debug.LogError($"Game Server Is Not Exist : {process.StartInfo.FileName}");
            return (null, port);
        }

        Debug.Log("\n+++++++++++++++++ New Server ++++++++++++++++++++\n");

        process.StartInfo.Arguments = $"-batchmode -nographics -port={port} -matchId={matchId}";
        process.StartInfo.UseShellExecute = true;
        process.StartInfo.CreateNoWindow = false;       // 콘솔 띄우기.
        process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;               // 일반 창

        // 게임 서버 실행
        process.Start();

        return (process, port);
    }

    //Test Game instance.
    public static (System.Diagnostics.Process process, int port) StartInGameServer(string matchId)
    {
        int port = GetAvailablePort(); // 사용 가능한 포트 확보

        var process = new System.Diagnostics.Process();
        process.StartInfo.FileName = "D:/Project/LocalGit/Team.GameCorp_CrownFall/Builds/InGameServer/Team.GameCorp_CrownFall.exe"; // 빌드된 서버 실행파일

        if (!File.Exists(process.StartInfo.FileName))
        {
            Debug.LogError($"Game Server Is Not Exist : {process.StartInfo.FileName}");
            return (null, port);
        }

        Debug.Log("\n+++++++++++++++++ New Server ++++++++++++++++++++\n");

        process.StartInfo.Arguments = $"-batchmode -nographics -port={port} -matchId={matchId}";    // 포트와 매치 ID 전달
        process.StartInfo.UseShellExecute = true;
        process.StartInfo.CreateNoWindow = false;       // 콘솔 띄우기.
        process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;               // 일반 창

        // 게임 서버 실행
        process.Start();

        return (process, port);
    }

    // 실제 사용 가능한 포트를 반환 (기초 구현)
    private static int GetAvailablePort()
    {
        // 실제 환경에서는 충돌 체크 필요
        return basePort++;
    }
}