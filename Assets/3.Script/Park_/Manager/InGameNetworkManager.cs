using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class InGameNetworkManager : NetworkManager
{
    public List<NetworkConnectionToClient> clients = new();         //서버에 접속 중인 클라이언트들.

    public List<CharacterInfo> characterInfos = new();

    [Server]
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Debug.Log("[InGameServer] : New Client In Server...\n");
        base.OnServerAddPlayer(conn);
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        // 씬에 있는 NetworkIdentity 오브젝트들 자동 등록
        NetworkServer.SpawnObjects();
    }
}