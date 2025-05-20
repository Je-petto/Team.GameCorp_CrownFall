# region using
using UnityEngine;
using UnityEngine.SceneManagement;
#endregion

public class SelectSceneControl : MonoBehaviour
{
    public void SceneLoad(string name)
    {
        if (SQL_Manager.i.info.User_Name == null) return;
        SceneManager.LoadScene(name);
    }
}