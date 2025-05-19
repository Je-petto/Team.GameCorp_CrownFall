# region using
// .net 라이브러리
using System;
// .net 에서 네트워크 및 소켓통신을 하기 위한 라이브러리
using System.Net;
using System.Net.Sockets;
// 데이터를 읽기 / 쓰기 하기 위한 라이브러리
using System.IO;
using System.Threading; // 멀티스레딩을 하기 위한 라이브러리
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// .net 에서 말하는 패킷의 단위는 stream
#endregion

public class TCP_Manager : MonoBehaviour
{
    public InputField ip;
    public InputField port;
    public InputField messageInput;

    [SerializeField] private Text log;


    StreamReader reader; // 데이터 읽는 놈
    StreamWriter writer; // 데이터 쓰는 놈

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
        // 지속적으로 돌아가야 함
        // 메세지가 들어올때마다 열어줘야함
        // 흐름에 예외처리
        try
        {
            TcpListener tcp = new TcpListener(IPAddress.Parse(ip.text), int.Parse(port.text));
            tcp.Start(); // 서버 시작 -> 서버 오픈
            Log.Enqueue("ServerOpen");

            TcpClient client = tcp.AcceptTcpClient();
            // TcpListener 에 연결될때까지 기다렸다가
            // client가 연결이되면 TcpClient에 할당
            Log.Enqueue(" Client 접속 확인");
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
            // 에러가 생기면 들어오는 곳
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

    private void ClientConnect() // 서버에 접근하는 쪽
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
        string textsum = $"[{SQL_Manager.i.info.User_Name}] : {messageInput.text}";
        // 내가 보낸 메세지도 messagebox에 넣어야 함
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