using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class NetworkLobbyManager : NetworkRoomManager
{
    // 로비에 로그인 되어있는 유저들. => 인게임 서버에서 로비로 다시 돌아 갈때 사용할 예정
    public List<UserAuth> cachedUser;

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

public class MatchUserListPacket
{
    public List<UserAuth> userList;
    public MatchUserListPacket(List<UserAuth> users) => userList = users;
}