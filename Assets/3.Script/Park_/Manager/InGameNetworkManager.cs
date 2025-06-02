using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class InGameNetworkManager : NetworkManager
{
    public List<NetworkConnectionToClient> clients = new();         //서버에 접속 중인 클라이언트들.

    public List<CharacterInfo> characterInfos = new();
    
    [Server]
    public GameObject GetProjection(string cid)
    {
        CharacterInfo info = characterInfos.Find(c => c.cid == cid);

        if (info == null) return null;


        return info.projection;
    }


    [Server]
    public GameObject GetModel(string cid)
    {
        CharacterInfo info = characterInfos.Find(c => c.cid == cid);

        if (info == null) return null;

        return info.inGameModel;
    }

    [Server]
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Debug.Log("[InGameServer] : New Client In Server...\n");
        base.OnServerAddPlayer(conn);
    }
}