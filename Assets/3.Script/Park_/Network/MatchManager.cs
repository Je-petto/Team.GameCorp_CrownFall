using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using System.Collections;
using System.Linq;
using System.IO;

public enum PlayerMatchState
{
    NotMatched,
    Matching,
    Matched,
    Ready
}

public class MatchManager : NetworkBehaviour
{
    private class MatchGroup
    {
        public Guid matchId;
        public List<NetworkConnectionToClient> players = new();
        public Dictionary<int, List<NetworkConnectionToClient>> teamData = new();
        public int readyCount = 0;
        public MatchGroup(Guid guid) { this.matchId = guid; }
    }

    private readonly List<MatchGroup> matchGroups = new();
    public int maxPlayerCount = 2;

    [Server]
    public void AddToMatchList(NetworkConnectionToClient conn)
    {
        NetworkMatch matchConn = conn.identity.GetComponent<NetworkMatch>();

        UserAuth uInfo = conn.identity.GetComponent<NetworkPlayer>().userAuth;

        MatchGroup group = matchGroups.Find(g => g.players.Count < maxPlayerCount);

        if (group == null)
        {
            // 매치가 없으면 새로 생성
            group = new(Guid.NewGuid());
            matchGroups.Add(group);
        }

        matchConn.matchId = group.matchId;

        if (group.players.Contains(conn)) return;
        group.players.Add(conn);

        Debug.Log("=========== Match Data ============");
        Debug.Log($"Match Players : {group.players.Count}");

        foreach (var player in group.players)
        {
            Debug.Log($"Player Info : {player.identity.GetComponent<NetworkPlayer>().userAuth.nickname}");
        }

        Debug.Log("===================================");

        // 그룹에 player가 최대로 차있다면 해당 클라이언트들 씬 이동 (로비 -> 게임 대기 씬)
        if (group.players.Count >= maxPlayerCount)
        {
            Debug.Log("[Match Complete] : Go to 'GameWatingScene'");
            MakeGameRoom(group);
        }
    }

    // ServerChangeScene() 는 서버의 모든 클라이언트들에게 씬을 변경할 것을 요구한다.
    // 따라서  new SceneMessage로 각각의 클라이언트들에게 전달이 필요
    [Server]
    public void RemoveToMatchList(NetworkConnectionToClient conn)
    {
        NetworkMatch matConn = conn.identity.GetComponent<NetworkMatch>();

        //해당 플레이어가 포함되어있는 그룹 찾기
        MatchGroup group = matchGroups.Find(g => g.players.Contains(conn));
        if (group == null) return;

        matConn.matchId = Guid.Empty;
        group.players.Remove(conn);
    }


    // 매칭이 완료된 후 대기방으로 이동하는 코드.
    [Server]
    private void MakeGameRoom(MatchGroup group)
    {
        Debug.Log("Match Complete...");
        string sceneName = "net.2.SelectorScene";

        SetTeam(group);

        // 클라이언트들에게 WaitingScene으로 이동하도록 한다.
        foreach (var conn in group.players)
        {
            conn.Send(new SceneMessage()
            {
                sceneName = sceneName,
                sceneOperation = SceneOperation.Normal,
                customHandling = true
            });
        }

        // 매칭된 유저들의 정보 리스트 [생성]
        List<UserAuth> users = new();

        foreach (var player in group.players)
        {
            var info = player.identity.GetComponent<NetworkPlayer>().userAuth;
            UserAuth i = new UserAuth(info.uid, info.nickname);
            users.Add(i);
        }

        // 매칭된 유저들의 정보 리스트 [전달]
        foreach (var conn in group.players)
        {
            var player = conn.identity.GetComponent<NetworkPlayer>();
            player.ReceiveMatchInfo(users);
        }

        // 서버 측에서도 해당 Scene 로드 필요 (안 하면 클라이언트 쪽에 씬이 미적용된다.) => 테스트 필요.....
        if (!SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
    }

    [Server]
    private void SetTeam(MatchGroup group)
    {
        group.teamData.Add(0, new());
        group.teamData.Add(1, new());

        // con
        foreach (var p in group.players)
        {
            if (group.teamData[0].Count > group.teamData[1].Count)
            {
                group.teamData[1].Add(p);
                p.identity.GetComponent<NetworkPlayer>().SetTeam(1);
                p.identity.GetComponent<NetworkPlayer>().userAuth.teamCode = 1;
                Debug.Log($"== team {p.identity.GetComponent<NetworkPlayer>().userAuth.nickname} : {1}");
            }
            else
            {
                group.teamData[0].Add(p);
                p.identity.GetComponent<NetworkPlayer>().SetTeam(0);
                p.identity.GetComponent<NetworkPlayer>().userAuth.teamCode = 0;
                Debug.Log($"== team {p.identity.GetComponent<NetworkPlayer>().userAuth.nickname} : {0}");
            }
        }
    }

    private IEnumerator GameStart_Co(MatchGroup group)
    {
        // 1. 모든 유저 정보 저장
        var userList = group.players.Select(conn => conn.identity.GetComponent<NetworkPlayer>().userAuth).ToList();
        var (inGameServerProcess, port) = GameSpawner.StartInGameServer(group.matchId, userList);

        Debug.Log(">> All Ready Complete => Game Start after 5 seconds");
        yield return new WaitForSeconds(5f);

        foreach (var conn in group.players)
        {
            conn.identity.GetComponent<NetworkPlayer>().TargetConnectToInGame(conn, port);
        }
    }

    [Server]
    public void ReceiveCharacterSelection(NetworkConnectionToClient conn, MatchPlayerCharacterDataPacket packet)
    {
        MatchGroup group = matchGroups.Find(g => g.players.Contains(conn));

        if (group == null) return;

        foreach (var player in group.players)
        {
            player.identity.GetComponent<NetworkPlayer>().ReceivePlayerCharacterData(packet);
        }
    }

    [Server]
    public void ReceivePlayerReadyState(NetworkConnectionToClient conn, MatchPlayerReadyDataPacket packet)
    {
        MatchGroup group = matchGroups.Find(g => g.players.Contains(conn));

        if (group == null) return;

        if (packet.dynamicData.Equals(PlayerMatchState.Ready))
        {
            group.readyCount++;

            // 게임 시작 여부 확인하기
            if (CheckAllReady(group)) StartCoroutine(GameStart_Co(group));
        }
        else
        {
            group.readyCount--;
        }

        Debug.Log($"Current Ready State : {group.readyCount}");
        foreach (var player in group.players)
        {
            player.identity.GetComponent<NetworkPlayer>().ReceivePlayerReadyState(packet);
        }
    }

    private bool CheckAllReady(MatchGroup group) => group.readyCount >= maxPlayerCount;
}


public class GameSpawner
{
    private static int basePort = 8000;
    public static (System.Diagnostics.Process, int) StartInGameServer(Guid matchId, List<UserAuth> users)
    {
        Debug.Log("\n\n");
        Debug.Log($"user Count : {users.Count}");
        Debug.Log("\n+++++++++++++++ New Ingame Server! ++++++++++++++++++\n");

        int port = GetAvailablePort();

        string jsonDirectory = Path.Combine(Application.dataPath, "MatchData");
        if (!Directory.Exists(jsonDirectory))
        {
            Directory.CreateDirectory(jsonDirectory);
        }

        string jsonPath = Path.Combine(jsonDirectory, $"{matchId}.json");

        string json = JsonUtility.ToJson(new MatchUserListPacket(users));
        File.WriteAllText(jsonPath, json);

        var process = new System.Diagnostics.Process();
        process.StartInfo.FileName = "D:/Project/Team.GameCorp_CrownFall/Builds/InGameServer/Team.GameCorp_CrownFall.exe";
        process.StartInfo.Arguments = $"-port={port} -matchId={matchId} -jsonPath={jsonPath}";
        process.StartInfo.UseShellExecute = true;
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