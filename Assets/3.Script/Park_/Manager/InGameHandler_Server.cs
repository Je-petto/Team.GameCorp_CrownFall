using UnityEngine;
using Mirror;
using System;
using System.Collections.Generic;
using System.IO;
using LitJson;

public class InGameHandler_Server : MonoBehaviour
{
    public Type type;

    private InGameNetworkManager manager;
    private kcp2k.KcpTransport kcp;
    [SerializeField] private string path;

    private void Awake()
    {
        // #region Test
        //     if (path.Equals(string.Empty))
        //     {
        //         path = Application.dataPath + "/License";
        //     }

        //     // 폴더 검사
        //     if (!File.Exists(path))
        //     {
        //         Directory.CreateDirectory(path);
        //     }

        //     //파일 검사
        //     if (!File.Exists(path + "/License.json"))
        //     {
        //         DefaultData(path);
        //     }

        //     manager = GetComponent<InGameNetworkManager>();
        //     kcp = (kcp2k.KcpTransport)manager.transport;
        // #endregion Test

        if (path.Equals(string.Empty))
        {
            path = Application.dataPath + "/License";
        }

        manager = GetComponent<InGameNetworkManager>();
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
                Server_IP = arg.Substring("-ip=".Length);
            }
            else if (arg.StartsWith("-jsonPath"))
            {
                MatchPath = arg.Substring("-jsonPath=".Length);
            }
        }

        Server_IP = GetLocalIPAddress();             //Test용으로 로컬에서 수행.
        kcp.port = ushort.Parse(Port);
    }

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

    public string Server_IP { get; private set; }
    public string Port { get; private set; }
    public string MatchPath { get; private set; }

    void Start()
    {
        StartServer();
    }

    public void StartClient()
    {
        Debug.Log($"{manager.networkAddress} : Start Client");

        manager.StartClient();

        string[] args = Environment.GetCommandLineArgs();

        foreach (var arg in args)
        {
            if (arg.StartsWith("-uid="))
            {
                InGameSession.uid = arg.Substring("-uid=".Length);
            }
        }

        InGameSession.isInit = true;
    }

    public void StartServer()
    {
        // 서버의 경로 WebGL로 빌드 불가
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            Debug.LogWarning("WebGL cannot be Server");
            return;
        }

        manager.StartServer();

        string[] args = Environment.GetCommandLineArgs();
        string matchId = "";

        foreach (var arg in args)
        {
            if (arg.StartsWith("-matchId="))
            {
                matchId = arg.Substring("-matchId=".Length);
            }
        }

        if (string.IsNullOrEmpty(matchId))
        {
            Debug.LogError("[Server] matchId 인자를 찾을 수 없습니다.");
            return;
        }

        List<UserAuth> userList = LoadMatchDataFromJson(matchId);

        (NetworkManager.singleton as InGameNetworkManager).Init(userList);

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

    private List<UserAuth> LoadMatchDataFromJson(string matchId)
    {
        if (!File.Exists(MatchPath))
        {
            Debug.LogError($"[MatchData] Match data file is null... : {MatchPath}");
            return new List<UserAuth>();
        }

        try
        {
            string json = File.ReadAllText(MatchPath);
            MatchUserListPacket data = JsonUtility.FromJson<MatchUserListPacket>(json);

            if (data == null || data.userList == null)
            {
                Debug.LogError("1 -- [MatchData] JSON Parsing Fail...");
                return new List<UserAuth>();
            }

            Debug.Log($"2 -- [MatchData] {data.userList.Count} Load Datas.");
            return data.userList;
        }
        catch (Exception ex)
        {
            Debug.LogError($"3 --[MatchData] JSON 파일 파싱 중 예외 발생: {ex.Message}");
            return new List<UserAuth>();
        }
    }


    private void OnApplicationQuit()
    {
        if (NetworkClient.isConnected) manager.StopClient();

        if (NetworkServer.active) manager.StopServer();
    }


    private void DefaultData(string path)
    {
        // Json을 만드는 작업
        List<Item> item = new List<Item>();
        item.Add(new Item("0", "127.0.0.1", "7777"));

        JsonData data = JsonMapper.ToJson(item);

        File.WriteAllText(path + "/License.json", data.ToString());
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

            Server_IP = ip_s;
            Port = port_s;
            type = (Type)Enum.Parse(typeof(Type), type_s);

            manager.networkAddress = Server_IP;
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