using UnityEngine;

public class mapGenerator : MonoBehaviour
{
    [Header("R 채널: 벽 타일 Prefabs")]
    public GameObject[] redPrefabs;
    [Header("B 채널: 바닥 타일 Prefabs")]
    public GameObject[] bluePrefabs;

    [Header("마스크 텍스처")]
    public Texture2D tileMask;

    [Header("맵 크기 & 타일 크기")]
    public int width = 50;
    public int height = 50;
    public float tileSize = 1f;

    void Start()
    {
        GenerateMapByMask();
    }

    void GenerateMapByMask()
    {
        if (tileMask == null)
        {
            Debug.LogError("마스크 텍스처를 설정해주세요!");
            return;
        }

        // 맵 중앙 좌표 계산
        Vector3 mapCenter = new Vector3(
            (width - 1) * tileSize * 0.5f,
            0f,
            (height - 1) * tileSize * 0.5f
        );

        // 기존 자식 오브젝트 제거
        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);

        // 타일 생성
        for (int x = 0; x < width; x++)
        for (int z = 0; z < height; z++)
        {
            // 마스크 색상 샘플링
            float u = (float)x / (width - 1);
            float v = (float)z / (height - 1);
            Color c = tileMask.GetPixelBilinear(u, v);

            // R/B 중 더 큰 값을 선택 (G 제거)
            float[] w = { c.r, c.b };
            int ch = w[0] >= w[1] ? 0 : 1;

            // 풀 선택
            GameObject[] pool = ch switch
            {
                0 => redPrefabs,
                1 => bluePrefabs,
                _ => null
            };
            if (pool == null || pool.Length == 0) continue;

            // 위치 계산 및 타일 생성
            Vector3 tilePos = new Vector3(x * tileSize, 0f, z * tileSize);
            GameObject tile = Instantiate(
                pool[Random.Range(0, pool.Length)],
                tilePos,
                Quaternion.identity,
                transform
            );
        }
    }
}
