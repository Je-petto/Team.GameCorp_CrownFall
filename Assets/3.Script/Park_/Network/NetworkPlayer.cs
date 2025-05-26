using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//해당 컴포넌트는 [로비, 게임 대기화면]에서 까지 유효하다.
public class NetworkPlayer : NetworkRoomPlayer
{
    [SyncVar] public PlayerMatchState matchState = PlayerMatchState.NotMatched;

    public UserAuth userAuth;

    public override void OnStartClient()
    {
        base.OnStartClient();

        userAuth = new(UserCache.I.session.uid, UserCache.I.session.nickname);
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

        WaitingSceneManager.I.OnChangeSelectedCharacter -= CmdSendPlayerSelectedCharacterData;
        WaitingSceneManager.I.OnChangeSelectedCharacter += CmdSendPlayerSelectedCharacterData;

        WaitingSceneManager.I.OnChangeMatchState -= CmdSendPlayerReadyState;
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
    void CmdSendPlayerReadyState(PlayerMatchState state)
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
    public void SetTeam(int teamCode)
    {
        Debug.Log($"Team Data Get : {teamCode}");
        userAuth.teamCode = teamCode;

        if (UserCache.I != null)
        {
            UserCache.I.session.teamCode = teamCode;
            Debug.Log($"[Client] UserCache에 팀 정보 반영 완료: {teamCode}");
        }
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

        UserCache.I.StartInGameClient(port);

        // Application.Quit(); // 또는 로비 UI 종료 처리
    }
}