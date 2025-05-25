using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : BehaviourSingleton<GameManager>
{
    [SerializeField] private bool dontDestroyOnLoad = true;

    protected override bool IsDontdestroy()
    {
        return dontDestroyOnLoad;
    }

    public void OnGameWin()
    {
        Debug.Log("게임 승리! 타워 파괴 완료.");
        // 추가 승리 로직이 있다면 여기에 작성
    }
}
