using System;
using System.Collections.Generic;
using System.IO;
using Mirror;
using UnityEngine;

public class InGameNetworkManager : NetworkManager
{
    public List<UserAuth> inGameUsers = new();

    public void Init(List<UserAuth> users)
    {
        this.inGameUsers = users;
    }

    public UserAuth GetUser(string uid)
    {
        UserAuth user = inGameUsers.Find(u => u.uid == uid);
        if (user == null)
        {
            Debug.Log("user is null");
            return null;
        }

        return user;
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
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
    
    public void LoadMatchData(Guid matchId)
    {
        string path = $"./MatchData/{matchId}.json";

        if (!File.Exists(path))
        {
            Debug.LogError($"[InGameServer] Match data not found: {path}");
            return;
        }

        string json = File.ReadAllText(path);
        MatchUserListPacket userListPacket = JsonUtility.FromJson<MatchUserListPacket>(json);

        foreach (var user in userListPacket.userList)
        {
            inGameUsers.Add(user);
            Debug.Log($"Loaded user: {user.uid} - {user.nickname}, char: {user.c_id}, team: {user.teamCode}");
        }
    }
}