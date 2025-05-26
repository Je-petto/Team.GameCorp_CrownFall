using System;
using System.Collections.Generic;
using System.IO;
using Mirror;
using UnityEngine;

public class InGameNetworkManager : NetworkManager
{
    public List<UserAuth> inGameUsers = new();

    public List<NetworkConnectionToClient> clients = new();         //서버에 접속 중인 클라이언트들.

    public void Init(List<UserAuth> users)
    {
        this.inGameUsers = users;
    }

    
    public UserAuth GetUser(NetworkConnectionToClient conn, string uid)
    {
        UserAuth user = inGameUsers.Find(u => u.uid == uid);
        if (user == null)
        {
            Debug.Log("user is null");
            return null;
        }

        Debug.Log($"uid :{uid} set Character...");

        //클라이언트 들에게 모두 전달하기
        foreach (var c in clients)
        {
            if (c.Equals(conn)) continue;               //보낸 사람이면 넘어가기.
            c.identity.GetComponent<PlayerController>().RecieveCharacterData(user);
        }

        return user;
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        clients.Add(conn);
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