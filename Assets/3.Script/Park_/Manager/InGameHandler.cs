using UnityEngine;
using Mirror;
using System;

public class InGameHandler : MonoBehaviour
{   
    public Type type;

    private NetworkManager manager;
    private kcp2k.KcpTransport kcp;
    [SerializeField] private string path;

    private void Awake()
    {
        manager = GetComponent<NetworkManager>();
        kcp = (kcp2k.KcpTransport)manager.transport;

        string[] args = Environment.GetCommandLineArgs();

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
            else if (arg.StartsWith("-uid="))
            {
                InGameSession.uid = arg.Substring("-uid=".Length);
            }
            else if (arg.StartsWith("-cid="))
            {
                InGameSession.characterId = arg.Substring("-cid=".Length);
            }
        }

        ServerIP = GetLocalIPAddress();             //Test용으로 로컬에서 수행.
        kcp.port = ushort.Parse(Port);
    }

    // 파싱을 하고 데이터를 플레이어에게 전달하기


    private string GetLocalIPAddress()
    {
        string localIP = "127.0.0.1"; // 기본값 (loopback)

        // try
        // {
        //     var host = Dns.GetHostEntry(Dns.GetHostName());
        //     foreach (var ip in host.AddressList)
        //     {
        //         if (ip.AddressFamily == AddressFamily.InterNetwork)
        //         {
        //             localIP = ip.ToString();
        //             break;
        //         }
        //     }
        // }
        // catch (Exception ex)
        // {
        //     Debug.LogWarning($"[IP 검색 실패] {ex.Message}");
        // }

        return localIP;
    }

    public string ServerIP { get; private set; }
    public string Port { get; private set; }

    void Start()
    {
        if (type.Equals(Type.Server))
        {
            StartServer();
        }
        else
        {
            StartClient();
        }
    }

    public void StartClient()
    {
        Debug.Log($"{manager.networkAddress} : Start Client");
        manager.StartClient();

        // NetworkClient.OnConnectedEvent += () =>
        // {
        //     Debug.Log("[Client] Connected. Sending AddPlayer request...");
        //     if (!NetworkClient.ready) NetworkClient.Ready();
        //     if (NetworkClient.localPlayer == null) NetworkClient.AddPlayer();
        // };

        // NetworkClient.OnDisconnectedEvent += () =>
        // {
        //     Debug.Log("[Client] Connected. Sending AddPlayer request...");
        //     if (!NetworkClient.ready) NetworkClient.Disconnect();
        // };
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

    private void OnApplicationQuit()
    {
        if (NetworkClient.isConnected) manager.StopClient();

        if (NetworkServer.active) manager.StopServer();
    }
}