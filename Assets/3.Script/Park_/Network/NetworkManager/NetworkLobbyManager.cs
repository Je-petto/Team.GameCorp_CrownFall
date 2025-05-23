using System;
using System.Collections.Generic;
using System.IO;
using Mirror;
using UnityEngine;

public class ClientSession
{
    public string uid;
    public string nickname;
    public string selected_cid;

    public ClientSession(string uid, string nickname, string selected_cid)
    {
        this.uid = uid;
        this.nickname = nickname;
        this.selected_cid = selected_cid;
    }
}

[System.Serializable]
public class PlayerSessionList
{
    public List<ClientSession> conns = new();
}


public class NetworkLobbyManager : NetworkRoomManager
{
    public ClientSession clientSession = null;

    #region Sub-Component
    public MatchManager matchManager;
    public NetworkHandler networkHandler;
    #endregion

    public List<NetworkPlayer> matchedPlayerList = new();

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
    public override GameObject OnRoomServerCreateGamePlayer(NetworkConnectionToClient conn, GameObject roomPlayerObj)
    {
        NetworkPlayer networkPlayer = roomPlayerObj.GetComponent<NetworkPlayer>();
        PlayerController gameplayer = Instantiate(playerPrefab).GetComponent<PlayerController>();

        //유저 정보, 선택한 캐릭터 id, 팀 데이터 전달
        // gameplayer.SetNetworkData(networkPlayer._userInfo, networkPlayer.selectedCharacter_Id, networkPlayer.myTeamData);

        return gameplayer.gameObject;
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
    private static int basePort = 8000; // 포트 시작점 (순차 증가용)

    public static (System.Diagnostics.Process process, int port) StartInGameServer(Guid matchId)
    {
        int port = GetAvailablePort(); // 사용 가능한 포트 확보

        var process = new System.Diagnostics.Process();
        process.StartInfo.FileName = "D:/Server_Path"; // 빌드된 서버 실행파일

        if (!File.Exists(process.StartInfo.FileName))
        {
            Debug.LogError($"게임 서버 실행 파일을 찾을 수 없습니다: {process.StartInfo.FileName}");
            return (null, port);
        }

        // string jsonData = JsonUtility.ToJson(matchPlayers);
        // string encoded = Uri.EscapeDataString(jsonData); // 안전하게 변환

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
        process.StartInfo.FileName = "D:/Project/Team.GameCorp_CrownFall/Builds/InGameServer/Team.GameCorp_CrownFall.exe"; // 빌드된 서버 실행파일

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