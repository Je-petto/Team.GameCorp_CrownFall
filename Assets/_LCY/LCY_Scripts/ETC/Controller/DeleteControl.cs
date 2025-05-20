using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DeleteControl : MonoBehaviour
{
    public InputField id_i;
    [SerializeField] private Text log;

    public void DeleteBTN(GameObject g)
    {
        if (id_i.text.Equals(string.Empty))
        {
            log.text = "삭제할 ID를 입력해주세요.";
            return;
        }
        if (SQL_Manager.i.DeleteID(id_i.text))
            g.SetActive(false);

        else
            log.text = "본인 ID를 입력해주세요.";
    }

    public void DeleteAfter(GameObject g)
    {
        if (id_i.text.Equals(string.Empty))
            return;
        if (SQL_Manager.i.DeleteID(id_i.text))
            g.SetActive(true);
    }
}