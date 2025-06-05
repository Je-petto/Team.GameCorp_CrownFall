using DG.Tweening;
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
            log.text = "ID와 패스워드를 입력해주세요";
            return;
        }
        
        // if (SQL_Manager.I.Login(id_i.text, Password_i.text))
        // {
        //     User_info info = SQL_Manager.I.info;

        //     g.SetActive(false);
        // }
        // else
        //     log.text = "ID와 패스워드를 확인해주세요.";

        UserCache.I.session.uid = id_i.text;
        UserCache.I.session.nickname = "tester";
        UserCache.I.session.selected_cid = "";

        log.text = $"{id_i.text}님 어서오세요.";

        DOVirtual.DelayedCall(1f, () => g.SetActive(false));
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