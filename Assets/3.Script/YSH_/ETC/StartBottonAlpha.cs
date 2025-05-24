using System.Collections;
using TMPro;
using UnityEngine;

public class StartBottonAlpha : MonoBehaviour
{
    // 이 스크립트는 시작 버튼에 깜빡이는 효과를 주기 위해
    // Coroutine으로 텍스트의 알파 값을 변경해서 표현해봤습니다.
    
    // 깜빡이는 효과를 적용할 TextMeshProUGUI 컴포넌트를 연결합니다.
    public TextMeshProUGUI text;
    
    void Start()
    {
        StartCoroutine(TextAlpha());
    }

    // TextAlpha() 코루틴은 텍스트의 알파 값을 주기적으로 변경하여 깜빡이는 효과를 생성합니다.
    IEnumerator TextAlpha()
    {
        // 무한 루프를 사용해서 깜빡임 효과를 유지 했습니다.
        while(true)
        {
            // 텍스트의 알파 값을 0으로 설정해 투명화.
            text.alpha = 0;
            // 0.5초 동안 대기.
            yield return new WaitForSeconds(0.5f);
            // 텍스트의 알파 값을 1로 설정해 불투명화.
            text.alpha = 1;
            // 0.5초 동안 대기합니다.
            yield return new WaitForSeconds(0.5f);
        }
    }
}