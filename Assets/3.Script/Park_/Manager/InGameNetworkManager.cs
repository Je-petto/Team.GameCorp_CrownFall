using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public class InGameNetworkManager : NetworkManager
{

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Debug.Log("[InGameServer] Add Player...");
        base.OnServerAddPlayer(conn);
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        Debug.Log($"[InGameServer] Client connected: {conn.address}...");
    }

    [Server]
    public override void OnServerSceneChanged(string sceneName)
    {
        Debug.Log($"[InGameServer] Scene Load Complete: {sceneName}...");
    }
}