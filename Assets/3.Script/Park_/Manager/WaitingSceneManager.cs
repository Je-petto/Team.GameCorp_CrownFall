using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mirror;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class WaitingSceneManager : BehaviourSingleton<WaitingSceneManager>
{
    protected override bool IsDontdestroy() => false;

    public NetworkManager networkManager; // Inspector에서 할당
    
    string serverIP = "127.0.0.1";                // 로컬 테스트용.
    public int _testCount;

    private List<NetworkPlayer> networkPlayers;
    [SerializeField] Transform nicknamePanParent;
    [SerializeField] PlayerDataPanel playerPanBase;

    List<PlayerDataPanel> playerList = new();

    void Start()
    {
        networkPlayers = FindObjectsOfType<NetworkPlayer>().ToList();
        networkManager = NetworkManager.singleton;
    }

    public void SetMatchedPlayers(List<UserAuth> matchedUserList)
    {
        foreach (var user in matchedUserList)
        {
            PlayerDataPanel playerPan = Instantiate(playerPanBase, nicknamePanParent);
            playerPan.Init(user);
            playerList.Add(playerPan);
        }
    }

    //캐릭터 선택 리스트 갱신.
    public void UpdateSelectWindow(string uid, string c_id)
    {
        PlayerDataPanel p = playerList.Find(a => a.userInfo.uid == uid);
        if (p == null) return;

        //리스트에서 sprite 찾기
        CharacterInfo data = characterList.Find(data => data.cid == c_id);
        if (data == null) return;

        p.SetCharacterImage(data.face);
    }

    public void UpdatePlayerReady(string uid, PlayerMatchState state)
    {
        PlayerDataPanel p = playerList.Find(a => a.userInfo.uid == uid);
        if (p == null) return;

        p.SetReadyState(state);
    }

    [SerializeField] List<CharacterInfo> characterList = new();
    [SerializeField] Transform buttonParent;

    public UnityAction<string> OnChangeSelectedCharacter;
    public UnityAction<PlayerMatchState> OnChangeMatchState;

    //버튼 클릭 이벤트
    public void OnClickSelectCharacter(int index)
    {
        Debug.Log($"Select char Index : {index}");

        CharacterInfo selectedCharacter = characterList[index];

        // 패킷을 만들고 Matchmanager에게 보내기
        OnChangeSelectedCharacter?.Invoke(selectedCharacter.cid);
    }

    public void OnClickReadyButton()
    {
        NetworkPlayer myPlayer = networkPlayers.Find(a => a.isLocalPlayer);
        PlayerMatchState state = myPlayer.matchState;

        if (state.Equals(PlayerMatchState.Ready))
            state = PlayerMatchState.Matched;
        else
            state = PlayerMatchState.Ready;

        OnChangeMatchState?.Invoke(state);
    }


    void StartNewClient(int port, string uid, string cid)
    {
        string args = $"-inGame -ip={serverIP} -port={port} -uid={uid} -cid={cid}"; // 예시: 매치 ID도 넘길 수 있음
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


}