using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEffectController : MonoBehaviour
{
    [System.Serializable]
    public class FxEntry
    {
        public string fxName;                   // 애니메이션 이벤트에서 사용할 FX 이름
        public GameObject effectPrefab;         // FX로 사용할 프리팹
        public string[] slotNames;              // FX를 붙일 위치의 후보 본 이름들
        public Vector3 localPositionOffset;     // 위치 미세 조정
        public Vector3 localRotationEuler;      // 회전 미세 조정
        public float lifetime = 2f;             // FX 자동 제거 시간
    }

    public Transform modelRoot;                 // 캐릭터 본 루트
    public List<FxEntry> fxList = new();        // FX 정보 리스트 (인스펙터에서 등록)

    private Dictionary<string, FxEntry> fxDict = new();              // FX 이름 -> FX 정보
    private Dictionary<string, Transform> cachedSpawnPoints = new(); // FX 이름 -> 붙일 위치 Transform

    private Transform footLeft, footRight, handLeft, handRight;

    private Animator animator;

    private void Awake()
    {
        // animator가 존재할 경우, 타이밍 문제 방지를 위해 비활성화할 수 있음
        animator = GetComponent<Animator>();
        // if (animator != null)
        //     animator.enabled = false; // ※ Dictionary 초기화 완료 후 Start에서 다시 켤 예정

        // fxList를 기반으로 Dictionary 생성 (빠른 검색용)
        fxDict = new Dictionary<string, FxEntry>(System.StringComparer.OrdinalIgnoreCase);
        foreach (var fx in fxList)
        {
            if (!fxDict.ContainsKey(fx.fxName))
                fxDict.Add(fx.fxName, fx);
        }

        Debug.Log($"[FX] Dictionary 초기화 완료: {fxDict.Count}개 FX 등록됨");
    }

    private IEnumerator Start()
    {
        // FX 붙일 위치 및 손/발 위치 탐색 코루틴 실행
        yield return StartCoroutine(DelayFindSlots());
        yield return StartCoroutine(DelayFindBodyParts());

        // Animator를 다시 활성화 (AnimationEvent가 fxDict 접근 가능해진 시점 이후)
        if (animator != null)
            animator.enabled = true;
    }

    // FX를 붙일 위치(slot)를 찾는 코루틴
    private IEnumerator DelayFindSlots()
    {
        yield return new WaitForEndOfFrame(); // 본 구조가 완전히 로드될 때까지 대기

        foreach (var fx in fxList)
        {
            Transform slot = modelRoot.FindSlot(fx.slotNames);
            if (slot != null)
            {
                cachedSpawnPoints[fx.fxName] = slot;
            }
            else
            {
                Debug.LogWarning($"[FX] '{fx.fxName}' FX 위치를 찾을 수 없습니다.");
            }
        }

        Debug.Log("[FX] FX 슬롯 위치 탐색 완료");
    }

    // 손/발 위치 찾기 (기타 외부에서도 사용 가능)
    private IEnumerator DelayFindBodyParts()
    {
        yield return new WaitForEndOfFrame();

        footLeft = modelRoot.FindSlot("leftfoot", "Ball_L", "l foot", "Lfoot");
        footRight = modelRoot.FindSlot("rightfoot", "Ball_R", "r foot", "Rfoot");
        handLeft = modelRoot.FindSlot("L Hand", "Hand_L", "LeftHand", "l hand");
        handRight = modelRoot.FindSlot("R Hand", "Hand_R", "RightHand", "r hand");

        Debug.Log("[FX] 손/발 위치 초기화 완료");
    }

    // 애니메이션 이벤트에서 호출되는 FX 재생 함수
    public void PlayFx(string fxName)
    {
        if (fxDict == null || fxDict.Count == 0)
        {
            Debug.LogError("[FX] fxDict가 아직 초기화되지 않았습니다.");
            return;
        }

        if (!fxDict.TryGetValue(fxName, out FxEntry fxEntry))
        {
            Debug.LogWarning($"[FX] '{fxName}' FX 정보 없음. 현재 등록된 FX들: {string.Join(", ", fxDict.Keys)}");
            return;
        }

        if (!cachedSpawnPoints.TryGetValue(fxName, out Transform parent))
        {
            Debug.LogWarning($"[FX] '{fxName}' FX 위치 캐시 없음");
            return;
        }

        // FX 생성
        GameObject fxInstance = Instantiate(fxEntry.effectPrefab, parent);
        fxInstance.transform.localPosition = fxEntry.localPositionOffset;
        fxInstance.transform.localRotation = Quaternion.Euler(fxEntry.localRotationEuler);
        Destroy(fxInstance, fxEntry.lifetime);
    }

    // 외부에서 손/발 위치를 참조할 수 있도록 getter 제공 가능
    // public Transform GetFootLeft() => footLeft;
    // public Transform GetFootRight() => footRight;
    // public Transform GetHandLeft() => handLeft;
    // public Transform GetHandRight() => handRight;
}
