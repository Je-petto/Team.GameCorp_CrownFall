using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;


//랜덤으로 스폰되는 방향으로 변경
public class ItemSpawner : MonoBehaviour
{
    [Header("pooling setting")]
    [Tooltip(" 스폰할 아이템 프리팹")]
    public GameObject itemPrefab;
    [Tooltip("풀에 미리 생성할 아이템 개수")]
    public int poolSize = 10;
    [Tooltip("아이템 자동 회수 시간")]
    public float returnDelay = 5f;


    [Header("랜덤 스폰 영역")]
    [Tooltip("최소 스폰 영역")]
    public Vector2 SpawnAreaMin = new Vector2(-10f, -10f);
    [Tooltip("최대 스폰 영역")]
    public Vector2 SpawnAreaMax = new Vector2(10f, 10f);
    [Tooltip("레이 시작 높이")]
    public float spawnHeight = 70f;
    [Tooltip("스폰간격")]
    public float spawnInterval = 3f;

    [Header("레이캐스트 설정")]
    [Tooltip("스폰할 레이어 마스크")]
    public LayerMask spawnLayer;
    [Tooltip("레이 최대 거리")]
    public float maxDistance = 100f;

    [Tooltip("아이템 스폰 y값")]
    public float spawnPoint = 1f;

    private Queue<GameObject> pool;

    void Awake()
    {
        // 풀 초기화
        pool = new Queue<GameObject>();
//Debug.Log($"풀 초기화 : poolSize = {poolSize}");
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(itemPrefab, transform);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
//Debug.Log($"풀 초기화 완료 : 실제 enqueued 개수 = {pool.Count}");
    }
  

    void Start()
    {
        //일정 간격으로 랜덤 스폰
        InvokeRepeating(nameof(RandomSpawn), 1f, spawnInterval);
    }

    void RandomSpawn()
    {
        Debug.Log("RandomSpawn 호출됨");

        // 랜덤 선택
        float x = Random.Range(SpawnAreaMin.x, SpawnAreaMax.x);
        float z = Random.Range(SpawnAreaMin.y, SpawnAreaMax.y);
        Vector3 rayOrigin = new Vector3(x, spawnHeight, z);

//Debug.Log($"Raycast 발사: origin={rayOrigin}, dir=Vector3.down, maxDist={maxDistance}");

        

        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, maxDistance, spawnLayer))
        {
            Debug.Log($"히트: {hit.collider.name} at {hit.point}");
            Vector3 point = hit.point;
            point.y = spawnPoint;
            SpawnAt(point, hit.normal);
        }
        //실패시 그냥 넘어간다
        else
        {
            Debug.Log("히트 실패");
        }
    }

    void SpawnAt(Vector3 position, Vector3 normal)
    {
        if (pool.Count == 0)
        {
            Debug.LogWarning("풀에 남은 아이템이 없습니다.");
            return;
        }
        //pool에서 꺼내서 배치
        GameObject item = pool.Dequeue();
        item.transform.position = position;
        //원하는 경우, 표면 법선 방향으로 살짝 띄어서 배치
        item.transform.rotation = Quaternion.LookRotation(normal) * Quaternion.Euler(90, 0, 0);
        item.SetActive(true);


        //일정 시간 후 자동 회수
        StartCoroutine(ReturnToPoolAfterDelay(item, returnDelay));
    }
    private IEnumerator ReturnToPoolAfterDelay(GameObject item, float delay)
    {
        yield return new WaitForSeconds(delay);
        item.SetActive(false);
        pool.Enqueue(item);
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 center = new Vector3((SpawnAreaMin.x + SpawnAreaMax.x) * 0.5f, spawnHeight, (SpawnAreaMin.y + SpawnAreaMax.y) * 0.5f);
        Vector3 size = new Vector3(Mathf.Abs(SpawnAreaMax.x - SpawnAreaMin.x), 0f, Mathf.Abs(SpawnAreaMax.y - SpawnAreaMin.y));

        Gizmos.DrawWireCube(transform.position, new Vector3(SpawnAreaMin.x * -1 + SpawnAreaMax.x, 5f, SpawnAreaMin.y * -1 + SpawnAreaMax.y));
    }
}
