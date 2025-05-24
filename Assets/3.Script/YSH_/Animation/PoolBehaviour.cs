using UnityEngine;

public class PoolBehaviour : MonoBehaviour
{
    [HideInInspector]
    public PoolManager poolmanager;

    public void Despawn()
    {
        if (poolmanager == null)
        {
            Debug.LogError($"[Despawn 오류] poolmanager가 null입니다. {name} 오브젝트가 PoolManager를 통해 생성되지 않았습니다.");
            gameObject.SetActive(false);
            return;
        }

        poolmanager.Despawn(this);
    }
}