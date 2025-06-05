using Mirror;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    [SerializeField] UserAuth user;
    
    public void OnClickGameStart()
    {
        if (UserCache.I.session.uid == "")
        {
            Debug.LogWarning("Login User Inform null....");
            return;
        }
        (NetworkManager.singleton as NetworkLobbyManager).StartClientHandler();
    }
}
