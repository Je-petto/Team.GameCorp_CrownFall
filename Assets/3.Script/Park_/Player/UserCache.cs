using System.IO;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class UserAuth
{
    public string uid;
    public string nickname;
    public string c_id;
    public int teamCode;
    public UserAuth() { }
    public UserAuth(string uid, string nickname)
    {
        this.uid = uid;
        this.nickname = nickname;
        c_id = "";
        teamCode = 0;
    }
}

[System.Serializable]
public struct ClientSession
{
    public string uid;
    public string nickname;
    public string selected_cid;
    public int teamCode;
}

public class UserCache : BehaviourSingleton<UserCache>
{
    protected override bool IsDontdestroy() => true;

    public ClientSession session;

    public void StartInGameClient(int port)
    {
        string ip = "127.0.0.1"; // 로컬 테스트용. 실제 환경에선 서버에서 전달받거나 DNS 사용.
        string args = $"-inGame -ip={ip} -port={port} -uid={session.uid} -cid={session.selected_cid} -team={session.teamCode}"; // 예시: 매치 ID도 넘길 수 있음
 
        Debug.Log($"[Client] : InGame Client Start!");

        var inGameprocess = new System.Diagnostics.Process();
        string processPath = Path.Combine(Application.dataPath, "InGameClient", "Team.GameCorp_CrownFall.exe");
        
        inGameprocess.StartInfo.FileName = processPath;
        inGameprocess.StartInfo.Arguments = args;
        inGameprocess.StartInfo.UseShellExecute = true;
        inGameprocess.StartInfo.CreateNoWindow = false;
        inGameprocess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;

        inGameprocess.EnableRaisingEvents = true;
        inGameprocess.Exited += (sender, e) =>
        {
            string uid = "uid-test";
            // ReconnectToLobbyServer(uid);
        };

        // 클라이언트 실행.
        inGameprocess.Start();

        //기존 클라이언트는 종료!
#if UNITY_EDITOR
        // 에디터에서는 플레이 모드 종료
        EditorApplication.isPlaying = false;
#else
            // 빌드된 게임에서는 애플리케이션 종료
        Application.Quit();
#endif
    }

    public void StartLobbyClient()
    {

    }
}