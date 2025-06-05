using UnityEngine;
using Mirror;
using System;
using System.Collections.Generic;
using System.IO;
using LitJson;



public enum Type
{
    Empty = 0,
    Client,
    Server
}

public class Item
{
    public string License;
    public string Server_IP;
    public string Port;

    public Item(string L_index, string IPValue, string port)
    {
        License = L_index;
        Server_IP = IPValue;
        Port = port;
    }
}

public static class InGameSession
{
    public static bool isInit = false;
    public static string uid;
    public static string characterId;
    public static int teamCode;
}

public class InGameHandler : MonoBehaviour
{
    public Type type;

    private InGameNetworkManager manager;
    private kcp2k.KcpTransport kcp;
    [SerializeField] private string path;

    private void Awake()
    {
        manager = GetComponent<InGameNetworkManager>();
        if (manager == null)
        {
            Debug.Log("manager is null...");
        }

        kcp = (kcp2k.KcpTransport)manager.transport;
        if (kcp == null)
        {
            Debug.Log("kcp is null...");
        }

        string[] args = Environment.GetCommandLineArgs();

        foreach (var arg in args)
        {
            Debug.Log($"[ARG] {arg}");
        }

        foreach (var arg in args)
        {
            if (arg.StartsWith("-port="))
            {
                Port = arg.Substring("-port=".Length);
            }
            else if (arg.StartsWith("-ip="))
            {
                ServerIP = arg.Substring("-ip=".Length);
            }
            else if (arg.StartsWith("-jsonPath"))
            {
                MatchPath = arg.Substring("-jsonPath=".Length);
            }
        }
    }

    public string ServerIP { get; private set; }
    public string Port { get; private set; }
    public string MatchPath { get; private set; }

    void Start()
    {
        manager.networkAddress = ServerIP;
        kcp.port = ushort.Parse(Port);

        Debug.Log($"ServerIp:{ServerIP}, Port : {Port}");
        StartClient();
    }

    public void StartClient()
    {
        Debug.Log($"{manager.networkAddress} : Start Client");


        string[] args = Environment.GetCommandLineArgs();

        foreach (var arg in args)
        {
            if (arg.StartsWith("-uid="))
            {
                InGameSession.uid = arg.Substring("-uid=".Length);
            }
        }

        Debug.Log($"------- fore Init = {InGameSession.isInit}");
        InGameSession.isInit = true;
        Debug.Log($"------- post Init = {InGameSession.isInit}");
        
        manager.StartClient();
    }

    private void OnApplicationQuit()
    {
        if (NetworkClient.isConnected) manager.StopClient();

        if (NetworkServer.active) manager.StopServer();
    }


    #region Test
    private Type License_type()
    {
        Type type = Type.Empty;
        try
        {
            string jsonString = File.ReadAllText(path + "/License.json");

            JsonData itemData = JsonMapper.ToObject(jsonString);

            string type_s = itemData[0]["License"].ToString();
            string ip_s = itemData[0]["Server_IP"].ToString();
            string port_s = itemData[0]["Port"].ToString();

            ServerIP = ip_s;
            Port = port_s;
            type = (Type)Enum.Parse(typeof(Type), type_s);

            manager.networkAddress = ServerIP;
            kcp.port = ushort.Parse(Port);
            return type;
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            return Type.Empty;
        }
    }


    #endregion
}