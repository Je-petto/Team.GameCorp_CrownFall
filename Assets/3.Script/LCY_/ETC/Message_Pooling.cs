# region using
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
# endregion

public class Message_Pooling : MonoBehaviour
{
    [SerializeField] private Text[] messageBox;

    public Action<string> message;

    private string curMessage = string.Empty;
    private string pastMessage;

    private void Start()
    {
        messageBox = transform.GetComponentsInChildren<Text>();
        message = AddingMessage;
        pastMessage = curMessage;
    }
    private void Update()
    {
        if (pastMessage.Equals(curMessage)) return;
        ReadText(curMessage);
        pastMessage = curMessage;
    }
    public void AddingMessage(string m)
    {
        curMessage = m;
    }
    public void ReadText(string m)
    {
        bool isInput = false;
        for (int i = 0; i < messageBox.Length; i++)
        {
            if (messageBox[i].text.Equals(""))
            {
                messageBox[i].text = m;
                isInput = true;
                break;
            }
        }
        if (!isInput)
        {
            for (int i = 1; i < messageBox.Length; i++)
            {
                messageBox[i - 1].text = messageBox[i].text;
            }
            messageBox[messageBox.Length - 1].text = m;
        }
    }
}