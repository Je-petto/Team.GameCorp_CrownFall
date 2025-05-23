using System.Collections;
using System.IO;
using kcp2k;
using Mirror;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class LobbyToInGame : MonoBehaviour
{
    public NetworkManager networkManager; // Inspector에서 할당
    public string serverIP = "127.0.0.1";

    void Start()
    {
        networkManager = NetworkManager.singleton;
    }

    public void OnClickTestMatchButton()
    {
        // 1. 인게임 서버 실행
        var (process, port) = GameSpawner.StartInGameServer("testing Match");

        if (process == null)
        {
            Debug.LogError("❌ 인게임 서버 실행");
            return;
        }

        Debug.Log($"[Test] 인게임 서버 시작됨 → 포트: {port}");
        StartCoroutine(WaitForServerAndConnect(serverIP, port));
    }

    IEnumerator WaitForServerAndConnect(string ip, int port)
    {
        Debug.Log($"[Test] 서버[{port}] 포트 오픈 대기 중...");

        yield return new WaitForSeconds(10f);

        Debug.Log("[Test] 포트 열림 → 클라이언트 접속 시도");

        StartNewClient(port, "dsf", "sdfa");

        // 2. 기존 네트워크 종료 (로비 연결 끊기)
        if (NetworkClient.isConnected || NetworkServer.active)
        {
            Debug.Log("[Client] 종료...");
            networkManager.StopClient();
        }
    }


    void StartNewClient(int port, string uid, string cid)
    {
        string ip = "127.0.0.1"; // 로컬 테스트용. 실제 환경에선 서버에서 전달받거나 DNS 사용.
        string args = $"-inGame -ip={ip} -port={port} -uid={uid} -cid={cid}"; // 예시: 매치 ID도 넘길 수 있음
        var process = new System.Diagnostics.Process();
        process.StartInfo.FileName = "D:/Project/Team.GameCorp_CrownFall/Builds/InGameClient/Team.GameCorp_CrownFall.exe";
        
        // 인게임 클라이언트
        if (!File.Exists(process.StartInfo.FileName))
        {
            Debug.LogError($"InGame Client executable not found at: {process.StartInfo.FileName}");
            return;
        }

        process.StartInfo.Arguments = args;
        process.StartInfo.UseShellExecute = true;
        process.StartInfo.CreateNoWindow = false;       // 콘솔 띄우기.
        process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;

        // 클라이언트 실행.
        process.Start();
        

        //기존 클라이언트는 종료!
        #if UNITY_EDITOR
        // 에디터에서는 플레이 모드 종료
        EditorApplication.isPlaying = false;
        #else
            // 빌드된 게임에서는 애플리케이션 종료
            Application.Quit();
        #endif
    }
}