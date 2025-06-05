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
    [SerializeField] PlayerDataPanel teamMatePan;

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
            PlayerDataPanel playerPan = Instantiate(teamMatePan, nicknamePanParent);
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
    public void SelectCharacter(CharacterInfo selectedCharacter)
    {
        Debug.Log($"Select character : {selectedCharacter.name}");

        NetworkPlayer myPlayer = networkPlayers.Find(a => a.isLocalPlayer);
        myPlayer.userAuth.c_id = selectedCharacter.cid;
        
        OnChangeSelectedCharacter?.Invoke(selectedCharacter.cid);
    }

    public void SetReadyState()
    {
        NetworkPlayer myPlayer = networkPlayers.Find(a => a.isLocalPlayer);
    
        PlayerMatchState state = myPlayer.matchState;

        if (state.Equals(PlayerMatchState.Ready))
            state = PlayerMatchState.Matched;
        else
            state = PlayerMatchState.Ready;

        OnChangeMatchState?.Invoke(state);
    }
}