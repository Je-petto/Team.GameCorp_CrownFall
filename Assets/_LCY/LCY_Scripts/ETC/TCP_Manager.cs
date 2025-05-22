# region using
// .net ���̺귯��
using System;
// .net ���� ��Ʈ��ũ �� ��������� �ϱ� ���� ���̺귯��
using System.Net;
using System.Net.Sockets;
// �����͸� �б� / ���� �ϱ� ���� ���̺귯��
using System.IO;
using System.Threading; // ��Ƽ�������� �ϱ� ���� ���̺귯��
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// .net ���� ���ϴ� ��Ŷ�� ������ stream
#endregion

public class TCP_Manager : MonoBehaviour
{
    public InputField ip;
    public InputField port;
    public InputField messageInput;

    [SerializeField] private Text log;


    StreamReader reader; // ������ �д� ��
    StreamWriter writer; // ������ ���� ��

    private Message_Pooling message;

    private Queue<string> Log = new Queue<string>();

    private void Update()
    {
        LogMessage();
    }
    private void LogMessage()
    {
        if (Log.Count > 0)
            log.text = Log.Dequeue();
    }
    #region Server
    public void ServerOpen()
    {
        message = FindAnyObjectByType<Message_Pooling>();
        Thread th = new Thread(ServerConnect);
        th.IsBackground = true;
        th.Start();
    }

    private void ServerConnect()
    {
        // ���������� ���ư��� ��
        // �޼����� ���ö����� ���������
        // �帧�� ����ó��
        try
        {
            TcpListener tcp = new TcpListener(IPAddress.Parse(ip.text), int.Parse(port.text));
            tcp.Start(); // ���� ���� -> ���� ����
            Log.Enqueue("ServerOpen");

            TcpClient client = tcp.AcceptTcpClient();
            // TcpListener �� ����ɶ����� ��ٷȴٰ�
            // client�� �����̵Ǹ� TcpClient�� �Ҵ�
            Log.Enqueue(" Client ���� Ȯ��");
            reader = new StreamReader(client.GetStream());
            writer = new StreamWriter(client.GetStream());
            writer.AutoFlush = true;

            while (client.Connected)
            {
                string readDate = reader.ReadLine();
                message.message(readDate);
            }
        }
        catch (Exception e)
        {
            // ������ ����� ������ ��
            Log.Enqueue(e.Message);
        }
    }
    #endregion
    #region Client
    public void _ClientConnect()
    {
        message = FindAnyObjectByType<Message_Pooling>();
        Log.Enqueue("Client Connect");
        Thread thread = new Thread(ClientConnect);
        thread.IsBackground = true;
        thread.Start();
    }

    private void ClientConnect() // ������ �����ϴ� ��
    {
        try
        {
            TcpClient client = new TcpClient();
            IPEndPoint ipEnd = new IPEndPoint(IPAddress.Parse(ip.text), int.Parse(port.text));
            client.Connect(ipEnd);
            Log.Enqueue("Server Connect Compelete");
            reader = new StreamReader(client.GetStream());
            writer = new StreamWriter(client.GetStream());
            writer.AutoFlush = true;

            while (client.Connected)
            {
                string readDate = reader.ReadLine();
                message.message(readDate);
            }
        }
        catch (Exception e)
        {
            Log.Enqueue(e.Message);
        }
    }
    #endregion
    #region Sending
    public void SendingBTN()
    {
        string textsum = $"[{SQL_Manager.I.info.User_Name}] : {messageInput.text}";
        // ���� ���� �޼����� messagebox�� �־�� ��
        if (SendingMessage(textsum))
        {
            message.message(textsum);
            messageInput.text = string.Empty;
        }
    }
    private bool SendingMessage(string m)
    {
        if (writer != null)
        {
            writer.WriteLine(m);
            return true;
        }
        else
        {
            Log.Enqueue("Writer Null");
            return false;
        }
    }
    #endregion
}