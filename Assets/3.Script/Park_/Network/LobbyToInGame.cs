using System.Collections;
using kcp2k;
using Mirror;
using UnityEngine;

public class LobbyToInGame : MonoBehaviour
{
    public NetworkManager networkManager; // Inspector에서 할당
    public string serverIP = "127.0.0.1";

    void Start()
    {
        networkManager = FindAnyObjectByType<NetworkManager>();    
    }

    public void OnClickTestMatchButton()
    {
        // 1. 인게임 서버 실행
        var (process, port) = GameSpawner.StartGameInstance("testing Match");

        if (process == null)
        {
            Debug.LogError("❌ 인게임 서버 실행 실패");
            return;
        }

        Debug.Log($"[Test] 인게임 서버 시작됨 → 포트: {port}");
        StartCoroutine(WaitForServerAndConnect(serverIP, port));
    }

    IEnumerator WaitForServerAndConnect(string ip, int port)
    {
        Debug.Log("[Test] 서버 포트 오픈 대기 중...");

        yield return new WaitForSeconds(10f);

        Debug.Log("[Test] 포트 열림 → 클라이언트 접속 시도");

        // 2. 기존 네트워크 종료 (로비 연결 끊기)
        if (NetworkClient.isConnected || NetworkServer.active)
        {
            Debug.Log(" 로비와의 접속을 종료하고 인게임 서버로 넘어갑니다.");
            
            Destroy(NetworkManager.singleton.gameObject);
            networkManager.StopClient();
        }

        // 3. 클라이언트 설정 후 접속
        networkManager.networkAddress = ip;
        var transport = networkManager.GetComponent<KcpTransport>();
        transport.port = ushort.Parse(port.ToString());

        Debug.Log("인게임 서버로 이동");
        networkManager.StartClient();
    }
}