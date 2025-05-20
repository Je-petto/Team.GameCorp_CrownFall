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
            log.text = "회원정보를 입력해주세요.";
            return;
        }
        if (SQL_Manager.i.SingnUp(id_i.text, Password_i.text, Phonenum_i.text))
        {
            g.SetActive(false);
        }

    }

    public void SelectBTN()
    {
        if (id_i.text.Equals(string.Empty))
        {
            log.text = "사용할 ID를 입력해주세요.";
            return;
        }

        if (SQL_Manager.i.Select(id_i.text))
            log.text = "해당 ID는 사용 가능한 ID 입니다.";
        else
            log.text = "해당 ID는 사용 불가능한 ID 입니다.";
    }
}