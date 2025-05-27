using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class InGameNetworkManager : NetworkManager
{
    public List<NetworkConnectionToClient> clients = new();         //서버에 접속 중인 클라이언트들.

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Debug.Log("[InGameServer] : New Client In Server...\n");
        base.OnServerAddPlayer(conn);
    }
}