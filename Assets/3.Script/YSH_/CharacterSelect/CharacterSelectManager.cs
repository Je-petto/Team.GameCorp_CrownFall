using Mirror;
using UnityEngine;

public class CharacterSelectManager : MonoBehaviour
{
    [SerializeField] private CharacterInfoDatabase characterDatabase;
    [SerializeField] private CharacterInfoUI characterInfoUI;
    [SerializeField] private Transform spawnPoint;

    private string selected_cid;
    private GameObject currentCharacterModel;

    public CharacterInfo SelectedCharacterInfo { get; private set; }

    void OnEnable()
    {
        Debug.Log($"[Client] {"CharacterSelectManager"} : Enable!");
        selected_cid = characterDatabase.characterInfos[0].cid;
        SelectedCharacterInfo = characterDatabase.characterInfos[0];
        SelectCharacterByCID(selected_cid);
    }

    public void SelectCharacterByCID(string cid)
    {
        var info = characterDatabase.GetCharacterByCID(cid);
        if (info == null)
        {
            Debug.LogError($"해당 CID({cid})를 가진 캐릭터를 찾을 수 없습니다.");
            return;
        }

        SelectedCharacterInfo = info;

        if (currentCharacterModel != null)
            Destroy(currentCharacterModel);

        currentCharacterModel = Instantiate(info.model, spawnPoint.position, spawnPoint.rotation);
        WaitingSceneManager.I.SelectCharacter(info);
        characterInfoUI?.SetCharacterInfo(info);
    }

    public void OnClickReadyButton()
    {
        (NetworkManager.singleton as NetworkLobbyManager).clientSession.selected_cid = "test1";
        WaitingSceneManager.I.SetReadyState();
    }
}