using UnityEngine;

public class mapGenerator : MonoBehaviour
{
    [Header("맵크기 , 타일")]
    public int width = 50;
    public int height =50;
    public float tileSize =1f;

    [Header("타일프리팹 RGBA")]    
    public GameObject[] tilePrefabs;

    [Header("분포 마스크 텍스처")]  
    public Texture2D tileMask;

    
    void Start()
    {
        GenerateMapByMask();
    }
    
    void GenerateMapByMask()
    {
        if(tilePrefabs.Length < 3 || tileMask == null)
        {
            Debug.LogError("타일 프리팹 4개와 마스크 텍스처를 모두 설정해주세요!");
            return;
        }
        for (int x = 0; x < width; x++)
        {
            for(int z = 0; z < height; z++)
            {
                //마스크에서 UV좌표로 색상 읽기
                float u = (float)x / (width - 1);
                float v = (float)z / (height - 1);
                Color c = tileMask.GetPixelBilinear(u,v);

                //Vector4로 변환(RGBA)
                Vector3 weights = new Vector3(c.r, c.g, c.b);

                //가장 큰 채널 인덱스 찾기
                int tileIndex =0;
                float maxW = weights[0];
                for (int i = 0; i < 3; i++)
                {
                    if(weights[i] > maxW)
                    {
                        maxW = weights[i];
                        tileIndex = i;
                    }
                }

                // 월드 위치 계산
                Vector3 pos = new Vector3(x*tileSize, 0f,z*tileSize);
                //랜덤 Y축 회전
                int rotIndex = Random.Range(0, 4);
                Quaternion rot = Quaternion.Euler(0f, rotIndex * 90f, 0f);

                // 프리팹 생성
                Instantiate(tilePrefabs[tileIndex], pos, rot, transform);
            }
        }
    }
}
