using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
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
    public string ServerIP;
    public string Port;

    public Item(string L_index, string IPValue, string port)
    {
        License = L_index;
        ServerIP = IPValue;
        Port = port;
    }
}
public class SeverChecker : MonoBehaviour
{
    public Type type;

    private NetworkManager networkManager;
    private kcp2k.KcpTransport kcp;

    [SerializeField] private string path;

    public string ServerIP { get; private set; }
    public string Port { get; private set; }

    private void OnEnable()
    {
        path = Application.dataPath + "/License";

        // 폴더 검사
        if (!File.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        // 파일 검사
        if (!File.Exists(path + "/License.json"))
        {
            DefaultData(path);
        }
        networkManager = GetComponent<NetworkManager>();
        kcp = (kcp2k.KcpTransport)networkManager.transport;

    }

    private void DefaultData(string path)
    {
        // Json 만드는 작업
        List<Item> item = new List<Item>();
        item.Add(new Item("0", "127.0.0.1", "7777"));

        JsonData data = JsonMapper.ToJson(item);
        File.WriteAllText(path + "/License.json", data.ToString());
    }

    private Type License_type()
    {
        Type t = Type.Empty;

        try
        {
            string jsonstring = File.ReadAllText(path + "/License.json");
            JsonData itemdata = JsonMapper.ToObject(jsonstring);

            string type_s = itemdata[0]["License"].ToString();
            string ip_s = itemdata[0]["ServerIP"].ToString();
            string port_s = itemdata[0]["Port"].ToString();

            ServerIP = ip_s;
            Port = port_s;
            t = (Type)Enum.Parse(typeof(Type), type_s);

            networkManager.networkAddress = ServerIP;
            kcp.port = ushort.Parse(Port);

            return t;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return Type.Empty;
        }
    }
    private void Start()
    {
        type = License_type();

        if (type.Equals(Type.Server))
        {
            Start_Server();
        }
        else
        {
            Start_Client();
        }
    }

    public void Start_Server()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            Debug.Log("webGL cannot be Server");
        }
        else
        {
            networkManager.Start();
            Debug.Log($"{networkManager.networkAddress} Start Server...");
            NetworkServer.OnConnectedEvent += (NetworkConnectionToClient) =>
            {
                Debug.Log($"new client Connect : {NetworkConnectionToClient.address}");
            };
            NetworkServer.OnDisconnectedEvent += (NetworkConnectionToClient) =>
            {
                Debug.Log($"new client Disconnect : {NetworkConnectionToClient.address}");
            };
        }
    }
    public void Start_Client()
    {

        networkManager.StartClient();
        Debug.Log($"{networkManager.networkAddress} : Start Client...");
    }

    private void OnApplicationQuit()
    {
        if (NetworkClient.isConnected)
        {
            networkManager.StopClient();
        }
        if (NetworkServer.active)
        {
            networkManager.StopServer();
        }
    }
}
