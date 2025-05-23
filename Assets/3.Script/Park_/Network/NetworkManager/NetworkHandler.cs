using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using LitJson;

// public enum Type
// {

//     Empty = 0,
//     Client,                 // 1
//     Server                  // 2
// }

// public class Item
// {
//     public string License;
//     public string Server_IP;
//     public string Port;

//     public Item(string L_index, string IPValue, string port)
//     {
//         License = L_index;
//         Server_IP = IPValue;
//         this.Port = port;
//     }
// }

public class NetworkHandler : MonoBehaviour
{
    public Type type;

    private NetworkLobbyManager manager;
    private kcp2k.KcpTransport kcp;
    [SerializeField] private string path;

    private void Awake()
    {
        if (path.Equals(string.Empty))
        {
            path = Application.dataPath + "/License";
        }

        // 폴더 검사
        if (!File.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        //파일 검사
        if (!File.Exists(path + "/License.json"))
        {
            DefaultData(path);
        }

        manager = GetComponent<NetworkLobbyManager>();
        kcp = (kcp2k.KcpTransport)manager.transport;
    }

    private void DefaultData(string path)
    {
        // Json을 만드는 작업
        List<Item> item = new List<Item>();
        item.Add(new Item("0", "127.0.0.1", "7777"));

        JsonData data = JsonMapper.ToJson(item);

        File.WriteAllText(path + "/License.json", data.ToString());
    }

    public string ServerIP { get; private set; }
    public string Port { get; private set; }

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

    void Start()
    {
        type = License_type();

        if (type.Equals(Type.Server))
        {
            StartServer();
        }
    }

    public void StartServer()
    {
        // 서버의 경로 WebGL로 빌드 불가
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            Debug.LogWarning("WebGL cannot be Server");
        }
        else
        {
            manager.StartServer();
            Debug.Log($"{manager.networkAddress} start server...");

            NetworkServer.OnConnectedEvent += (NetworkConnectionToClient) =>
            {
                Debug.Log($"new Client : {NetworkConnectionToClient.address}");
            };
            NetworkServer.OnDisconnectedEvent += (NetworkConnectionToClient) =>
            {
                Debug.Log($"new Client Disconnect : {NetworkConnectionToClient.address}");
            };
        }
    }

    public void SetServer()
    {
        
    }

    public void StartClient()
    {
        Debug.Log($"{manager.networkAddress} : Start Client");
        manager.StartClient();
    }

    private void OnApplicationQuit()
    {
        if (NetworkClient.isConnected) manager.StopClient();

        if (NetworkServer.active) manager.StopServer();
    }
}
