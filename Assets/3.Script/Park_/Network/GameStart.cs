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

        (NetworkManager.singleton as NetworkLobbyManager).clientSession = new(user.uid, user.nickname, "");
        (NetworkManager.singleton as NetworkLobbyManager).StartClientHandler();
    }
}
