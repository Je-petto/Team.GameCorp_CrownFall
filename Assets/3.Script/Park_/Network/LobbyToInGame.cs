using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 매칭 버튼 클릭시 수행할 메서드
public class LobbyToInGame : MonoBehaviour
{
    public void OnClickTestMatchButton()
    {
        var (inGameServerProcess, port) = GameSpawner.StartGameInstance("testing Match");
    }
}