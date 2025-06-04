using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Collections;

public class InGameNetworkManager : NetworkManager
{
    public List<NetworkConnectionToClient> clients = new();         //서버에 접속 중인 클라이언트들.

    public List<CharacterInfo> characterInfos = new();

    public List<UserAuth> userList = new();

    public void Init(List<UserAuth> userList)
    {
        this.userList = userList;
    }
    
    [Server]
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Debug.Log("[InGameServer] : New Client In Server...\n");
        base.OnServerAddPlayer(conn);
        clients.Add(conn);
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        clients.Remove(conn);
        base.OnServerDisconnect(conn);

        if (clients.Count <= 0)
        {
            Debug.Log("[InGameServer] : All clients disconnected. Shutting down server...");
            Application.Quit();
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        // 씬에 있는 NetworkIdentity 오브젝트들 자동 등록
        NetworkServer.SpawnObjects();
    }

    public void GameOver(GameObject tower, string team)
    {
        StartCoroutine(ShowGameOverPan(tower, team));
    }

    IEnumerator ShowGameOverPan(GameObject tower, string team)
    {
        yield return new WaitForSeconds(0.2f);

        foreach (var c in clients)
        {
            c.identity.GetComponent<PlayerController_Net>().GameOverSet(tower, team);
        }
    }
}