using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInfo : MonoBehaviour
{
    public Text userName_T;
    public Text userRecord_T;

    private void Update()
    {
        userName_T.text = SQL_Manager.i.info.User_Name;
    }
}