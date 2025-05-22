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
            log.text = "������ ID�� �Է����ּ���.";
            return;
        }
        if (SQL_Manager.I.DeleteID(id_i.text))
            g.SetActive(false);

        else
            log.text = "���� ID�� �Է����ּ���.";
    }

    public void DeleteAfter(GameObject g)
    {
        if (id_i.text.Equals(string.Empty))
            return;
        if (SQL_Manager.I.DeleteID(id_i.text))
            g.SetActive(true);
    }
}