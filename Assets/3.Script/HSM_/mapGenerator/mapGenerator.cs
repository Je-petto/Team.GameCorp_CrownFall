using UnityEngine;

public class mapGenerator : MonoBehaviour
{
    [Header("R 채널: 벽 타일 Prefabs")]
    public GameObject[] redPrefabs;
    [Header("G 채널: 안개 Prefabs")]
    public GameObject[] greenPrefabs;
    [Header("B 채널: 바닥 타일 Prefabs")]
    public GameObject[] bluePrefabs;

    [Header("마스크 텍스처")]
    public Texture2D tileMask;

    [Header("맵 크기 & 타일 크기")]
    public int width    = 50;
    public int height   = 50;
    public float tileSize = 1f;

    [Header("안개 높이 (G 채널)")]
    public float fogHeight = 3f;
    [Tooltip("얼마나 안쪽으로 밀어넣을지 (타일 크기 비율)")]
    [Range(0f, 1f)]
    public float insetFactor = 0.5f; // 0.5타일 만큼 안으로

    void Start()
    {
        GenerateMapByMask();
    }

    void GenerateMapByMask()
    {
        if (tileMask == null) { Debug.LogError("마스크 텍스처를 설정해주세요!"); return; }

        // 1) 맵 중앙 좌표 계산
        Vector3 mapCenter = new Vector3(
            (width  - 1) * tileSize * 0.5f,
            0f,
            (height - 1) * tileSize * 0.5f
        );

        // 2) 이전 생성물 삭제
        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);

        // 3) 타일 + 안개 생성
        for (int x = 0; x < width; x++)
        for (int z = 0; z < height; z++)
        {
            // 마스크 샘플링
            float u = (float)x / (width  - 1);
            float v = (float)z / (height - 1);
            Color c = tileMask.GetPixelBilinear(u, v);

            // R/G/B 중 최대 채널 인덱스
            float[] w = { c.r, c.g, c.b };
            int ch = 0;
            for (int i = 1; i < 3; i++)
                if (w[i] > w[ch]) ch = i;

            // 풀 선택
            GameObject[] pool = ch switch {
                0 => redPrefabs,
                1 => greenPrefabs,
                2 => bluePrefabs,
                _ => null
            };
            if (pool == null || pool.Length == 0) continue;

            // 타일 기본 위치
            Vector3 tilePos = new Vector3(x * tileSize, 0f, z * tileSize);

            if (ch == 1)
            {
                // G 채널 → 안개
                // 중앙 방향 벡터 계산
                Vector3 dir = (mapCenter - tilePos);
                dir.y = 0f;
                dir.Normalize();

                // insetFactor 만큼 안쪽으로 오프셋
                float inset = tileSize * insetFactor;
                Vector3 fogPos = tilePos
                    + dir * inset                     // 안쪽으로 이동
                    + Vector3.up * (fogHeight * 0.5f); // 높이 중앙

                // 인스턴트 & 스케일
                GameObject fog = Instantiate(
                    pool[Random.Range(0, pool.Length)],
                    fogPos,
                    Quaternion.identity,
                    transform
                );
                // X/Z 스케일을 tileSize + inset*2 로 줘서 겹침이 부드럽게
                float sx = tileSize + inset * 2f;
                fog.transform.localScale = new Vector3(sx, fogHeight, sx);
            }
            else
            {
                // R/B 채널 → 일반 타일
                GameObject tile = Instantiate(
                    pool[Random.Range(0, pool.Length)],
                    tilePos,
                    Quaternion.Euler(0f, 90f * Random.Range(0, 4), 0f),
                    transform
                );
            }
        }
    }
}
