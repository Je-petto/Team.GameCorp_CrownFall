using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 캐릭터 프리팹에 붙는 스크립트
public class Character : MonoBehaviour
{
    public CharacterInfo characterInfo; // 이 캐릭터의 정보

    // 필요하다면 이곳에서 데이터 초기화도 가능해요
    void Start()
    {
        // 예시: 이름을 로그로 출력해보기
        if (characterInfo != null)
        {
            Debug.Log("캐릭터 이름: " + characterInfo.characterName);
        }
    }
}

