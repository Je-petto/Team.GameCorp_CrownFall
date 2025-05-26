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
        
        UserCache.I.session.uid = user.uid;
        UserCache.I.session.nickname =  user.nickname;
        UserCache.I.session.selected_cid = "";
        (NetworkManager.singleton as NetworkLobbyManager).StartClientHandler();
    }
}
