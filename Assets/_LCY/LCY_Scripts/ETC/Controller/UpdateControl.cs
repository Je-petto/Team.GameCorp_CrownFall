using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UpdateControl : MonoBehaviour
{
    public InputField id_i;
    public InputField Password_i;
    [SerializeField] private Text log;

    public void UpdateBTN(GameObject g)
    {
        if (id_i.text.Equals(string.Empty))
        {
            log.text = "ȸ�������� �Է����ּ���.";
            return;
        }
        if (SQL_Manager.I.UpdatePW(id_i.text, Password_i.text))
            g.SetActive(false);

        else
            log.text = "ȸ�������� Ȯ�����ּ���.";
    }

    public void UpdateAfter(GameObject g)
    {
        if (id_i.text.Equals(string.Empty)) return;
        if (SQL_Manager.I.UpdatePW(id_i.text, Password_i.text))
            g.SetActive(true);
    }
}