using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SingnControl : MonoBehaviour
{
    public InputField id_i;
    public InputField Password_i;
    public InputField Phonenum_i;
    [SerializeField] private Text log;
    public GameObject loginPanel;

    public void JoinBTN(GameObject g)
    {
        if (id_i.text.Equals(string.Empty) || Password_i.text.Equals(string.Empty) || Phonenum_i.text.Equals(string.Empty))
        {
            log.text = "ȸ�������� �Է����ּ���.";
            return;
        }
        if (SQL_Manager.I.SingnUp(id_i.text, Password_i.text, Phonenum_i.text))
        {
            g.SetActive(false);
        }

    }

    public void SelectBTN()
    {
        if (id_i.text.Equals(string.Empty))
        {
            log.text = "����� ID�� �Է����ּ���.";
            return;
        }

        if (SQL_Manager.I.Select(id_i.text))
            log.text = "�ش� ID�� ��� ������ ID �Դϴ�.";
        else
            log.text = "�ش� ID�� ��� �Ұ����� ID �Դϴ�.";
    }
}