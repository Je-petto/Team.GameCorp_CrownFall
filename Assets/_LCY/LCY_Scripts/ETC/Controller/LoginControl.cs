using UnityEngine;
using UnityEngine.UI;

public class LoginControl : MonoBehaviour
{
    public InputField id_i;
    public InputField Password_i;
    [SerializeField] private Text log;

    public void LoginBTN(GameObject g)
    {
        if (id_i.text.Equals(string.Empty) || Password_i.text.Equals(string.Empty))
        {
            log.text = "�α��� ������ �Է����ּ���.";
            return;
        }
        if (SQL_Manager.I.Login(id_i.text, Password_i.text))
        {
            User_info info = SQL_Manager.I.info;

            g.SetActive(false);
        }
        else
            log.text = "�α��� ������ Ȯ�����ּ���.";
    }

    public void LoginAfter(GameObject g)
    {
        if (id_i.text.Equals(string.Empty) || Password_i.text.Equals(string.Empty))
            return;
        if (SQL_Manager.I.Login(id_i.text, Password_i.text))
        {
            User_info info = SQL_Manager.I.info;

            g.SetActive(true);
        }
    }
}