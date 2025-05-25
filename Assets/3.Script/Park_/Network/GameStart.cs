using Mirror;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    [SerializeField] UserAuth user;
    public void OnClickGameStart()
    {
        // if (SQL_Manager.I.info == null)
        // {
        //     Debug.LogWarning("Login User Inform null....");
        //     return;
        // }
        
        (NetworkManager.singleton as NetworkLobbyManager).clientSession.uid = user.uid;
        (NetworkManager.singleton as NetworkLobbyManager).clientSession.nickname =  user.nickname;
        (NetworkManager.singleton as NetworkLobbyManager).clientSession.selected_cid = "";
        (NetworkManager.singleton as NetworkLobbyManager).StartClientHandler();
    }
}
