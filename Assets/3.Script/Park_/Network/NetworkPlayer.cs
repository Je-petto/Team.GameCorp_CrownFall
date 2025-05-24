using kcp2k;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Telepathy;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct UserAuth
{
    public string uid;
    public string nickname;
    public string c_id;
    public UserAuth(string uid, string nickname)
    {
        this.uid = uid;
        this.nickname = nickname;
        c_id = null;
    }
}

//해당 컴포넌트는 [로비, 게임 대기화면]에서 까지 유효하다. 
public class NetworkPlayer : NetworkRoomPlayer
{
    [SyncVar] public PlayerMatchState matchState = PlayerMatchState.NotMatched;

    public UserAuth userAuth;

    public TeamComponent myTeamData = null;
    public string selectedCharacter_Id = "";

    public override void OnStartClient()
    {
        base.OnStartClient();

        ClientSession session = (NetworkManager.singleton as NetworkLobbyManager).clientSession;

        userAuth = new(session.uid, session.nickname);
        CmdSendUserInfo(userAuth.uid, userAuth.nickname);

        //씬 전환 이벤트 생성
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        Debug.Log("Stop Client...");
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "net.2.SelectorScene")
        {
            matchState = PlayerMatchState.Matched;
            StartCoroutine(BindEvent());
        }
    }

    IEnumerator BindEvent()
    {
        yield return new WaitUntil(() => WaitingSceneManager.I != null);
        WaitingSceneManager.I.OnChangeSelectedCharacter += CmdSendPlayerSelectedCharacterData;
        WaitingSceneManager.I.OnChangeMatchState += CmdSendPlayerReadyState;
    }

    [Command]
    public void CmdRequestStartMatching(bool on)
    {
        NetworkLobbyManager manager = (NetworkLobbyManager)NetworkManager.singleton;
        manager.StartMatching(connectionToClient, on);
    }

    [Command]
    void CmdSendUserInfo(string uid, string nick)
    {
        userAuth = new UserAuth(uid, nick);
    }

    // 캐릭터 선택 정보 갱신
    [Command]
    void CmdSendPlayerSelectedCharacterData(string c_id) //CharacterData data
    {
        // selectedCharacter = data;
        Debug.Log($"Client U_ID[{userAuth.uid}] : Select Character : {c_id}");

        selectedCharacter_Id = c_id;
        userAuth.c_id = c_id;
        MatchPlayerCharacterDataPacket packet = new MatchPlayerCharacterDataPacket(userAuth.uid, userAuth.nickname, userAuth.c_id);
        ((NetworkLobbyManager)NetworkManager.singleton).matchManager.ReceiveCharacterSelection(connectionToClient, packet);
    }

    [TargetRpc]
    public void ReceivePlayerCharacterData(MatchPlayerCharacterDataPacket packet)
    {
        if (WaitingSceneManager.I == null) return;
        WaitingSceneManager.I.UpdateSelectWindow(packet.uid, packet.dynamicData);
        Debug.Log("Get Character Data!");
    }


    // 플레이어 준비 상태 갱신.
    [Command]
    void CmdSendPlayerReadyState(PlayerMatchState state) //CharacterData data
    {
        Debug.Log($"Client U_ID[{userAuth.uid}] : Ready State Change : {state}");

        this.matchState = state;

        MatchPlayerReadyDataPacket packet = new MatchPlayerReadyDataPacket(userAuth.uid, userAuth.nickname, matchState);
        ((NetworkLobbyManager)NetworkManager.singleton).matchManager.ReceivePlayerReadyState(connectionToClient, packet);
    }

    [TargetRpc]
    public void ReceivePlayerReadyState(MatchPlayerReadyDataPacket packet)
    {
        if (WaitingSceneManager.I == null) return;
        WaitingSceneManager.I.UpdatePlayerReady(packet.uid, packet.dynamicData);
    }

    [TargetRpc]
    public void ReceiveMatchInfo(List<UserAuth> matchedUserList)
    {
        Debug.Log("[Client] Match Info Received:");
        if (matchedUserList == null) return;

        foreach (var user in matchedUserList)
        {
            Debug.Log($"▶️ UID: {user.uid}, Nickname: {user.nickname}");
        }

        // StartCoroutine(GetMatchedMemberList_Co(matchedUserList));
    }

    [TargetRpc]
    public void SetTeam(TeamType team)
    {
        Debug.Log($"Team Data Get : {team}");
        myTeamData = new(team);
    }

    IEnumerator GetMatchedMemberList_Co(List<UserAuth> matchedUserList)
    {
        yield return new WaitUntil(() => WaitingSceneManager.I != null);
        WaitingSceneManager.I.SetMatchedPlayers(matchedUserList);
    }

    //InGame 서버로 이동한다.           ==============================>>> 로비 서버와 인게임 서버가 따로 있다.
    [TargetRpc] // 서버에서 특정 클라이언트에게 호출되는 RPC
    public void TargetConnectToInGame(NetworkConnection target, int port)
    {
        Debug.Log($"Connecting to InGame Server on port {port}");
        Debug.Log("게임 시작!!!");

        StartInGameClient(target, port);

        Application.Quit(); // 또는 로비 UI 종료 처리
    }

    void StartInGameClient(NetworkConnection target, int port)
    {
        string ip = "127.0.0.1"; // 로컬 테스트용. 실제 환경에선 서버에서 전달받거나 DNS 사용.
        string args = $"-inGame -ip={ip} -port={port} -uid={userAuth.uid} -cid={userAuth.c_id}"; // 예시: 매치 ID도 넘길 수 있음
        string ingameClientPath = "D:/Project/Team.GameCorp_CrownFall/Builds/InGameClient/Team.GameCorp_CrownFall.exe";

        // 인게임 클라이언트
        if (!File.Exists(ingameClientPath))
        {
            Debug.LogError($"InGame Client executable not found at: {ingameClientPath}");
            return;
        }

        Debug.Log($"[Client] : InGame Client Start!");

        var process = new System.Diagnostics.Process();
        process.StartInfo.FileName = ingameClientPath;
        process.StartInfo.Arguments = args;
        process.StartInfo.UseShellExecute = true;
        process.StartInfo.CreateNoWindow = false;       // 콘솔 띄우기.
        process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;

        // 클라이언트 실행.
        process.Start();


        //기존 클라이언트는 종료!
#if UNITY_EDITOR
        // 에디터에서는 플레이 모드 종료
        EditorApplication.isPlaying = false;
#else
            // 빌드된 게임에서는 애플리케이션 종료
            Application.Quit();
#endif
    }

}