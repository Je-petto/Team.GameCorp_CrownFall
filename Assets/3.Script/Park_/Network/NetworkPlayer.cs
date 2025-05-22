using kcp2k;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using Telepathy;
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

    // public CharacterData selectedCharacter;
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
        if (scene.name == "GameWaitingScene")
        {
            // StartCoroutine(BindEvent());
        }
    }

    // IEnumerator BindEvent()
    // {
    //     yield return new WaitUntil(() => WaitingManager.I != null);
    //     WaitingManager.I.OnChangeSelectedCharacter += CmdSendPlayerSelectedCharacterData;
    //     WaitingManager.I.OnChangeMatchState += CmdSendPlayerReadyState;
    // }

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

        MatchPlayerCharacterDataPacket packet = new MatchPlayerCharacterDataPacket(userAuth.uid, userAuth.nickname, c_id);
        ((NetworkLobbyManager)NetworkManager.singleton).matchManager.ReceiveCharacterSelection(connectionToClient, packet);
    }

    [TargetRpc]
    public void ReceivePlayerCharacterData(MatchPlayerCharacterDataPacket packet)
    {
        // if (WaitingManager.I == null) return;
        // WaitingManager.I.UpdateSelectWindow(packet.uid, packet.dynamicData);
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
        // if (WaitingManager.I == null) return;
        // WaitingManager.I.UpdatePlayerReady(packet.uid, packet.dynamicData);
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

    // IEnumerator GetMatchedMemberList_Co(List<UserAuth> matchedUserList)
    // {
    //     yield return new WaitUntil(() => WaitingManager.I != null);
    //     WaitingManager.I.SetMatchedPlayers(matchedUserList);
    // }



    //InGame 서버로 이동한다.           ==============================>>> 로비 서버와 인게임 서버가 따로 있다.
    [TargetRpc] // 서버에서 특정 클라이언트에게 호출되는 RPC
    public void TargetConnectToInGame(NetworkConnection target, int port)
    {
        Debug.Log($"Connecting to InGame Server on port {port}");
        Debug.Log("게임 시작!!!");

        // 접속할 서버의 IP 주소 설정 (테스트용은 localhost, 실제론 공인 IP 또는 도메인)
        NetworkManager.singleton.networkAddress = "127.0.0.1";                  //즉, 서버 ip : 

        // Mirror에서 사용하는 TelepathyTransport의 포트를 설정
        NetworkManager.singleton.GetComponent<KcpTransport>().port = (ushort)port;

        // 클라이언트 시작 (지정된 포트에 있는 인게임 서버로 접속)
        NetworkManager.singleton.StartClient();
    }
}