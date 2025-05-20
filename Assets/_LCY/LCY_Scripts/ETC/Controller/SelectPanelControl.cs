# region using
using UnityEngine;
using UnityEngine.SceneManagement;
#endregion

public class SelectPanelControl : MonoBehaviour
{
    public void Open(GameObject g)
    {
        g.SetActive(true);
    }

    public void Close(GameObject g)
    {
        g.SetActive(false);
    }

    public void GameStart()
    {
        
    }
}